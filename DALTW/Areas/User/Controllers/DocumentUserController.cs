﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using DALTW.Repositories;
using Aspose.Words;

namespace DALTW.Areas.Admin.Controllers
{
    [Area("User")]
    public class DocumentUserController : Controller
    {
    
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly ICompetitionRepository _competitionRepository;

        public DocumentUserController(
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
        [AllowAnonymous]
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
        [Authorize]
        //  Chuyển đổi Word sang PDF và hiển thị
        public async Task<IActionResult> ViewPdf(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);

            if (document == null || string.IsNullOrEmpty(document.FileURL))
            {
                return NotFound("Tài liệu không tồn tại hoặc không có file Word.");
            }

            string wordPath = Path.Combine(_webHostEnvironment.WebRootPath, document.FileURL.TrimStart('/'));

            if (!System.IO.File.Exists(wordPath))
            {
                return NotFound("File Word không tồn tại.");
            }

            string pdfPath = Path.ChangeExtension(wordPath, ".pdf");

            // Nếu file PDF chưa tồn tại, thực hiện chuyển đổi từ Word sang PDF
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

            // Cập nhật đường dẫn PDF cho View
            document.FileURL = "/" + Path.GetRelativePath(_webHostEnvironment.WebRootPath, pdfPath).Replace("\\", "/");

            return View("ViewPdf", document);
        }

        //  Lưu file vào thư mục wwwroot/DocFile

        private async Task<string> SaveFile(IFormFile file)
        {
            string savePath = Path.Combine(_webHostEnvironment.WebRootPath, "DocFile", file.FileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
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
            ViewBag.Semesters = new SelectList(await _semesterRepository.GetAllAsync(), "SemesterID", "Name");
            ViewBag.Competitions = new SelectList(await _competitionRepository.GetAllAsync(), "CompetitionID", "Name");
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
    }
}
