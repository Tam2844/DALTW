﻿using Aspose.Words;
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
using DALTW.Helper;

namespace DALTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class DocumentManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly ICompetitionRepository _competitionRepository;

        public DocumentManagerController(
             ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            IDocumentRepository documentRepository,
            ICategoryRepository categoryRepository,
            IGradeRepository gradeRepository,
            ITopicRepository topicRepository,
            ISemesterRepository semesterRepository,
            ICompetitionRepository competitionRepository)
        {
            _context = context;
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
            foreach (var document in documents)
            {
                if (document.FileURL != null && document.FileURL.EndsWith(".docx"))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FileURL.TrimStart('/'));

                    // Chuyển trang đầu tiên của tài liệu Word thành ảnh
                    var image = ConvertWordToImage(filePath);
                    document.ImageFilePath = image; // Lưu ảnh vào đối tượng tài liệu
                }
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
                document.Slug = SlugHelper.GenerateSlug(document.Name);//slug 
                await _documentRepository.AddAsync(document);
                return RedirectToAction("Index", "DocumentManager", new { area = "Admin" });
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

        [Authorize]
        public async Task<IActionResult> ViewPdf(int id, string? slug)
        {
            var document = await _documentRepository.GetByIdAsync(id);

            if (document == null || string.IsNullOrEmpty(document.FileURL))
                return NotFound("Tài liệu không tồn tại hoặc không có file Word.");

            var correctSlug = SlugHelper.GenerateSlug(document.Name);
            if (string.IsNullOrEmpty(slug) || slug != correctSlug)
            {
                return RedirectToRoute("document_slug", new { id = id, slug = correctSlug });
            }

            string viewedKey = $"viewed_doc_{id}";
            if (HttpContext.Session.GetString(viewedKey) == null)
            {
                document.ViewCount += 1;
                await _documentRepository.UpdateAsync(document);
                HttpContext.Session.SetString(viewedKey, "true");
            }

            string wordPath = Path.Combine(_webHostEnvironment.WebRootPath, document.FileURL.TrimStart('/'));
            if (!System.IO.File.Exists(wordPath))
                return NotFound("File Word không tồn tại.");

            string pdfPath = Path.ChangeExtension(wordPath, ".pdf");
            if (!System.IO.File.Exists(pdfPath))
            {
                try
                {
                    var wordDocument = new Aspose.Words.Document(wordPath);
                    wordDocument.Save(pdfPath, Aspose.Words.SaveFormat.Pdf);
                }
                catch (Exception ex)
                {
                    return BadRequest("Lỗi chuyển đổi file: " + ex.Message);
                }
            }

            document.FileURL = "/" + Path.GetRelativePath(_webHostEnvironment.WebRootPath, pdfPath).Replace("\\", "/");
            return View("ViewPdf", document);
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

            var oldDocument = await _documentRepository.GetByIdAsync(id);
            if (oldDocument == null)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    oldDocument.Name = document.Name;
                    oldDocument.Content = document.Content;
                    oldDocument.Price = document.Price;
                    oldDocument.CategoryID = document.CategoryID;
                    oldDocument.TopicID = document.TopicID;
                    oldDocument.GradeID = document.GradeID;
                    oldDocument.SemesterID = document.SemesterID;
                    oldDocument.CompetitionID = document.CompetitionID;
                    oldDocument.Slug = SlugHelper.GenerateSlug(document.Name);//slug

                    if (file != null && file.Length > 0)
                    {
                        oldDocument.FileURL = await SaveFile(file);

                        if (file.FileName.EndsWith(".docx"))
                        {
                            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, oldDocument.FileURL.TrimStart('/'));
                            oldDocument.ImageFilePath = ConvertWordToImage(fullPath);
                        }
                    }

                    await _documentRepository.UpdateAsync(oldDocument);
                    return RedirectToAction("Index", "DocumentManager", new { area = "Admin" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
                ModelState.AddModelError("", "Lỗi khi cập nhật dữ liệu: " + ex.Message);
            }

            await LoadSelectLists();
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

        private string GetWordContent(string fileURL)
        {
            // Sử dụng Aspose.Words để đọc nội dung từ file Word
            string wordPath = Path.Combine(_webHostEnvironment.WebRootPath, fileURL.TrimStart('/'));
            var doc = new Aspose.Words.Document(wordPath);
            return doc.GetText();
        }

        private string ConvertWordToImage(string filePath)
        {
            var doc = new Aspose.Words.Document(filePath);

            // Lấy trang đầu tiên
            var imageStream = new MemoryStream();
            var options = new Aspose.Words.Saving.ImageSaveOptions(Aspose.Words.SaveFormat.Png)
            {
                PageSet = new Aspose.Words.Saving.PageSet(0) // Lấy trang đầu tiên
            };
            doc.Save(imageStream, options);

            // Đảm bảo thư mục images đã tồn tại
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);  // Tạo thư mục nếu không tồn tại
            }

            // Lưu hình ảnh vào thư mục wwwroot/images/
            string imagePath = "/images/" + Guid.NewGuid() + ".png";  // Đảm bảo đường dẫn bắt đầu bằng '/'
            string savePath = Path.Combine(directoryPath, Path.GetFileName(imagePath));

            using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                imageStream.WriteTo(fileStream);
            }

            return imagePath; // Trả về đường dẫn hình ảnh bắt đầu với '/'
        }


        public static string ConvertDocxToPdf(string docxPath)
        {
            try
            {
                string pdfPath = Path.ChangeExtension(docxPath, ".pdf");

                Aspose.Words.Document doc = new Aspose.Words.Document(docxPath);
                doc.Save(pdfPath, SaveFormat.Pdf);

                return pdfPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi chuyển đổi: " + ex.Message);
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSuggestions(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Json(new List<object>());

            var documents = await _documentRepository.GetAllAsync();

            var results = documents
                .Where(d => d.Name.ToLower().Contains(keyword.ToLower()))
                .Select(d => new
                {
                    id = d.DocumentID,
                    name = d.Name
                })
                .Take(10) // Giới hạn 10 kết quả gợi ý
                .ToList();

            return Json(results);
        }
    }
}
