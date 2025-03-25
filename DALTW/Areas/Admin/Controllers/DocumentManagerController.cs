using Aspose.Words;
using DALTW.Models;
using DALTW.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace DALTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DocumentManagerController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ITopicRepository _topicRepository;

        public DocumentManagerController(
            IWebHostEnvironment webHostEnvironment,
            IDocumentRepository documentRepository,
            ICategoryRepository categoryRepository,
            IGradeRepository gradeRepository,
            ITopicRepository topicRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _documentRepository = documentRepository;
            _categoryRepository = categoryRepository;
            _gradeRepository = gradeRepository;
            _topicRepository = topicRepository;
        }

        // 📝 Hiển thị danh sách tài liệu với bộ lọc
        public async Task<IActionResult> Index(int? topicId, int? gradeId, int? categoryId, string keyword)
        {
            var documents = await _documentRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                documents = documents.Where(d =>
                    d.Name.ToLower().Contains(keyword) ||
                    d.Content.ToLower().Contains(keyword)
                ).ToList();
            }

            if (topicId.HasValue)
                documents = documents.Where(d => d.TopicID == topicId.Value).ToList();

            if (gradeId.HasValue)
                documents = documents.Where(d => d.GradeID == gradeId.Value).ToList();

            if (categoryId.HasValue)
                documents = documents.Where(d => d.CategoryID == categoryId.Value).ToList();

            await LoadSelectLists();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DocumentList", documents);
            }

            return View(documents);
        }

        // 📄 Trang thêm tài liệu
        public async Task<IActionResult> Add()
        {
            await LoadSelectLists();
            return View();
        }

        // ➕ Xử lý thêm tài liệu (POST)
        [HttpPost]
        public async Task<IActionResult> Add(Models.Document document, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                return View(document);
            }

            try
            {
                if (file != null)
                {
                    document.FileURL = await SaveFile(file);
                }

                await _documentRepository.AddAsync(document);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi lưu vào database: " + ex.Message);
                await LoadSelectLists();
                return View(document);
            }
        }

        // 🔄 Chuyển đổi Word sang PDF
        public async Task<IActionResult> ViewPdf(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null || string.IsNullOrEmpty(document.FileURL))
            {
                return NotFound("Tài liệu không tồn tại hoặc không có file Word.");
            }

            // Đường dẫn file Word và PDF
            string wordPath = Path.Combine(_webHostEnvironment.WebRootPath, document.FileURL.TrimStart('/'));
            string pdfPath = Path.ChangeExtension(wordPath, ".pdf");

            // Nếu file PDF tồn tại, xóa nó trước khi tạo mới
            if (System.IO.File.Exists(pdfPath))
            {
                System.IO.File.Delete(pdfPath);

                // 🕒 Chờ 1 giây để đảm bảo file không còn bị khóa
                System.Threading.Thread.Sleep(1000);
            }

            try
            {
                // Chuyển đổi Word -> PDF
                var doc = new Aspose.Words.Document(wordPath);
                doc.Save(pdfPath, SaveFormat.Pdf);
                doc = null;

                // 🕒 Chờ 1 giây để đảm bảo hệ thống giải phóng file
                System.Threading.Thread.Sleep(1000);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi chuyển đổi file: " + ex.Message);
            }

            // Cập nhật đường dẫn PDF trong database
            document.ImageURL = "/" + Path.GetRelativePath(_webHostEnvironment.WebRootPath, pdfPath).Replace("\\", "/");
            await _documentRepository.UpdateAsync(document);

            return View("ViewPdf", document.ImageURL);
        }


        // 📂 Lưu file vào wwwroot/DocFile
        private async Task<string> SaveFile(IFormFile file)
        {
            string savePath = Path.Combine(_webHostEnvironment.WebRootPath, "DocFile", file.FileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/DocFile/" + file.FileName;
        }

        // 🔄 Tải danh sách danh mục, lớp, chủ đề cho View
        private async Task LoadSelectLists()
        {
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "CategoryID", "Name");
            ViewBag.Grades = new SelectList(await _gradeRepository.GetAllAsync(), "GradeID", "Name");
            ViewBag.Topics = new SelectList(await _topicRepository.GetAllAsync(), "TopicID", "TopicName");
        }

        // ✏ Hiển thị trang chỉnh sửa tài liệu
        public async Task<IActionResult> Edit(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound();

            await LoadSelectLists();
            return View(document);
        }

        // 🔄 Xử lý cập nhật tài liệu
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Models.Document document, IFormFile? file)
        {
            if (id != document.DocumentID)
                return BadRequest();

            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null)
                        document.FileURL = await SaveFile(file);

                    await _documentRepository.UpdateAsync(document);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật dữ liệu: " + ex.Message);
            }

            await LoadSelectLists();
            return View(document);
        }

        // ❌ Hiển thị trang xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound();

            return View(document);
        }

        // 🗑 Xử lý xóa tài liệu
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound();

            try
            {
                if (!string.IsNullOrEmpty(document.FileURL))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, document.FileURL.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                await _documentRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi xóa tài liệu: " + ex.Message);
                return View(document);
            }
        }
    }
}
