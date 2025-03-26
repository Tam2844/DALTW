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
    
    public class DocumentManagerController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly ICompetitionRepository _competitionRepository;

        public DocumentManagerController(
            IWebHostEnvironment webHostEnvironment,
            IDocumentRepository documentRepository,
            ICategoryRepository categoryRepository,
            IGradeRepository gradeRepository,
            ITopicRepository topicRepository,
            ISemesterRepository semesterRepository,
            ICompetitionRepository competitionRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _documentRepository = documentRepository;
            _categoryRepository = categoryRepository;
            _gradeRepository = gradeRepository;
            _topicRepository = topicRepository;
            _semesterRepository = semesterRepository;
            _competitionRepository = competitionRepository;
        }

        [Authorize(Roles = "Admin, Employee")]
        //  Hiển thị danh sách tài liệu
        public async Task<IActionResult> Index(int? topicId, int? gradeId, int? categoryId, int? semesterID, int? competitionID, string keyword)
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
            if (semesterID.HasValue)
            {
                documents = documents.Where(d => d.SemesterID == semesterID.Value).ToList();
            }
            if (competitionID.HasValue)
            {
                documents = documents.Where(d => d.CompetitionID == competitionID.Value).ToList();
            }
            await LoadSelectLists();
            // ✅ Đổ dữ liệu vào ViewBag để hiển thị danh sách chọn
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DocumentList", documents);
            }

            return View(documents);
        }



        [Authorize(Roles = "Admin,Employee")]
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
        [Authorize(Roles = "Admin, Employee")]
        // 🔄 Chuyển đổi Word sang PDF
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
            ViewBag.DocumentID = document.DocumentID;
            string pdfUrl = "/" + Path.GetRelativePath(_webHostEnvironment.WebRootPath, pdfPath).Replace("\\", "/");
            return View("ViewPdf", pdfUrl);
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
            ViewBag.Semesters = new SelectList(await _semesterRepository.GetAllAsync(), "SemesterID", "Name");
            ViewBag.Competitions = new SelectList(await _competitionRepository.GetAllAsync(), "CompetitionID", "Name");
        }
        [Authorize(Roles = "Admin, Employee")]
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
            var semesters = await _semesterRepository.GetAllAsync();
            var competitions = await _competitionRepository.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "Name");
            ViewBag.Grades = new SelectList(grades, "GradeID", "Name");
            ViewBag.Topics = new SelectList(topics, "TopicID", "TopicName");
            ViewBag.Semesters = new SelectList(await _semesterRepository.GetAllAsync(), "SemesterID", "Name");
            ViewBag.Competitions = new SelectList(await _competitionRepository.GetAllAsync(), "CompetitionID", "Name");

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
            var semesters = await _semesterRepository.GetAllAsync();
            var competitions = await _competitionRepository.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "Name");
            ViewBag.Grades = new SelectList(grades, "GradeID", "Name");
            ViewBag.Topics = new SelectList(topics, "TopicID", "TopicName");
            ViewBag.Semesters = new SelectList(await _semesterRepository.GetAllAsync(), "SemesterID", "Name");
            ViewBag.Competitions = new SelectList(await _competitionRepository.GetAllAsync(), "CompetitionID", "Name");

            return View(document);
        }
        [Authorize(Roles = "Admin")]
        //  Xử lý cập nhật tài liệu
        [HttpGet]     
        // ❌ Hiển thị trang xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound();

            return View(document);
        }
        [Authorize(Roles = "Admin")]
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
