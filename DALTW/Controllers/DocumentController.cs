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

namespace DALTW.Controllers
{
   
    
    public class DocumentController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ITopicRepository _topicRepository;

        public DocumentController(
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

        //  Hiển thị danh sách tài liệu
        public async Task<IActionResult> Index(int? topicId, int? gradeId, int? categoryId)
        {
            var documents = await _documentRepository.GetAllAsync();

            // ✅ Lọc theo Topic
            if (topicId.HasValue)
            {
                documents = documents.Where(d => d.TopicID == topicId.Value).ToList();
            }

            // ✅ Lọc theo Grade
            if (gradeId.HasValue)
            {
                documents = documents.Where(d => d.GradeID == gradeId.Value).ToList();
            }

            // ✅ Lọc theo Category
            if (categoryId.HasValue)
            {
                documents = documents.Where(d => d.CategoryID == categoryId.Value).ToList();
            }

            // ✅ Đổ dữ liệu vào ViewBag để hiển thị danh sách chọn
            ViewBag.Topics = new SelectList(await _topicRepository.GetAllAsync(), "TopicID", "TopicName");
            ViewBag.Grades = new SelectList(await _gradeRepository.GetAllAsync(), "GradeID", "Name");
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "CategoryID", "Name");

            return View(documents);
        }


        //  Trang thêm tài liệu
        public async Task<IActionResult> Add()
        {
            await LoadSelectLists();
            return View();
        }

        //  Xử lý thêm tài liệu (POST)
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

        //  Chuyển đổi Word sang PDF và hiển thị
        public async Task<IActionResult> ViewPdf(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null || string.IsNullOrEmpty(document.FileURL))
            {
                return NotFound("Tài liệu không tồn tại hoặc không có file Word.");
            }

            // Đường dẫn file
            string wordPath = Path.Combine(_webHostEnvironment.WebRootPath, document.FileURL.TrimStart('/'));
            string pdfPath = Path.ChangeExtension(wordPath, ".pdf");

            // Nếu file PDF chưa tồn tại, thực hiện chuyển đổi
            if (!System.IO.File.Exists(pdfPath))
            {
                try
                {
                    new Aspose.Words.Document(wordPath).Save(pdfPath, SaveFormat.Pdf);
                }
                catch (Exception ex)
                {
                    return BadRequest("Lỗi chuyển đổi file: " + ex.Message);
                }
            }

            string pdfUrl = "/" + Path.GetRelativePath(_webHostEnvironment.WebRootPath, pdfPath).Replace("\\", "/");
            return View("ViewPdf", pdfUrl);
        }

        //  Lưu file vào thư mục wwwroot/DocFile
        private async Task<string> SaveFile(IFormFile file)
        {
            string savePath = Path.Combine(_webHostEnvironment.WebRootPath, "DocFile", file.FileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/DocFile/" + file.FileName;
        }

        //  Tải danh sách danh mục, lớp, chủ đề cho View
        private async Task LoadSelectLists()
        {
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "CategoryID", "Name");
            ViewBag.Grades = new SelectList(await _gradeRepository.GetAllAsync(), "GradeID", "Name");
            ViewBag.Topics = new SelectList(await _topicRepository.GetAllAsync(), "TopicID", "TopicName");
        }
        //  Hiển thị trang chỉnh sửa tài liệu
        public async Task<IActionResult> Edit(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            var grades = await _gradeRepository.GetAllAsync();
            var topics = await _topicRepository.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "Name");
            ViewBag.Grades = new SelectList(grades, "GradeID", "Name");
            ViewBag.Topics = new SelectList(topics, "TopicID", "TopicName");

            return View(document);
        }

        //  Xử lý cập nhật tài liệu
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Models.Document document, IFormFile? file)
        {
            if (id != document.DocumentID)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null)
                    {
                        document.FileURL = await SaveFile(file);
                    }

                    await _documentRepository.UpdateAsync(document);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật dữ liệu: " + ex.Message);
            }

            var categories = await _categoryRepository.GetAllAsync();
            var grades = await _gradeRepository.GetAllAsync();
            var topics = await _topicRepository.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "Name");
            ViewBag.Grades = new SelectList(grades, "GradeID", "Name");
            ViewBag.Topics = new SelectList(topics, "TopicID", "TopicName");

            return View(document);
        }
        // Hiển thị trang xác nhận xóa tài liệu
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        // Xử lý xóa tài liệu
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            try
            {
                // Xóa file tài liệu khỏi thư mục (nếu có)
                if (!string.IsNullOrEmpty(document.FileURL))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, document.FileURL.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Xóa khỏi database
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
