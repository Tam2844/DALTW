using Microsoft.AspNetCore.Hosting;
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
using DALTW.Models;
using DALTW.Helper;

namespace DALTW.Controllers
{
    public class DocumentUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly ICompetitionRepository _competitionRepository;

        public DocumentUserController(
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
<<<<<<< HEAD
=======

>>>>>>> f7c9db261833552c33682b6acf1ce1ac22a26129
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? topicId, int? gradeId, int? categoryId, int? semesterID, int? competitionID, string keyword)
        {
            var documents = await _documentRepository.GetAllAsync();

            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                documents = documents.Where(d =>
                    d.Name.ToLower().Contains(keyword) ||
                    d.Content.ToLower().Contains(keyword)
                ).ToList();
            }

<<<<<<< HEAD
            // Lọc theo topicId, gradeId, categoryId, semesterID, competitionID
=======
>>>>>>> f7c9db261833552c33682b6acf1ce1ac22a26129
            if (topicId.HasValue)
                documents = documents.Where(d => d.TopicID == topicId.Value).ToList();
<<<<<<< HEAD
            }

=======
>>>>>>> f7c9db261833552c33682b6acf1ce1ac22a26129
            if (gradeId.HasValue)
                documents = documents.Where(d => d.GradeID == gradeId.Value).ToList();
<<<<<<< HEAD
            }

=======
>>>>>>> f7c9db261833552c33682b6acf1ce1ac22a26129
            if (categoryId.HasValue)
                documents = documents.Where(d => d.CategoryID == categoryId.Value).ToList();
<<<<<<< HEAD
            }

=======
>>>>>>> f7c9db261833552c33682b6acf1ce1ac22a26129
            if (semesterID.HasValue)
                documents = documents.Where(d => d.SemesterID == semesterID.Value).ToList();
<<<<<<< HEAD

                // Thêm tên học kỳ vào ViewBag
                ViewBag.SemesterName = _context.Semesters.FirstOrDefault(s => s.SemesterID == semesterID)?.Name;
            }

=======
>>>>>>> f7c9db261833552c33682b6acf1ce1ac22a26129
            if (competitionID.HasValue)
                documents = documents.Where(d => d.CompetitionID == competitionID.Value).ToList();
<<<<<<< HEAD
            }

            // Load lại các select lists
            await LoadSelectLists();

            // Xử lý ảnh Word
=======

            await LoadSelectLists();

>>>>>>> f7c9db261833552c33682b6acf1ce1ac22a26129
            foreach (var document in documents)
            {
                if (document.FileURL != null && document.FileURL.EndsWith(".docx"))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FileURL.TrimStart('/'));
                    var image = ConvertWordToImage(filePath);
                    document.ImageFilePath = image;
                }
            }

            return View(documents);
        }
<<<<<<< HEAD




        [Authorize]
=======
>>>>>>> f7c9db261833552c33682b6acf1ce1ac22a26129

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

        public IActionResult ByGradeSemester(int gradeId, int semesterId)
        {
            var documents = _context.Documents
                .Where(d => d.GradeID == gradeId && d.SemesterID == semesterId)
                .ToList();

            ViewBag.GradeName = _context.Grades.FirstOrDefault(g => g.GradeID == gradeId)?.Name;
            ViewBag.SemesterName = _context.Semesters.FirstOrDefault(s => s.SemesterID == semesterId)?.Name;

            return View(documents);
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
                .Take(10)
                .ToList();

            return Json(results);
        }

        private async Task LoadSelectLists()
        {
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "CategoryID", "Name");
            ViewBag.Grades = new SelectList(await _gradeRepository.GetAllAsync(), "GradeID", "Name");
            ViewBag.Topics = new SelectList(await _topicRepository.GetAllAsync(), "TopicID", "TopicName");
            ViewBag.Semesters = new SelectList(await _semesterRepository.GetAllAsync(), "SemesterID", "Name");
            ViewBag.Competitions = new SelectList(await _competitionRepository.GetAllAsync(), "CompetitionID", "Name");
        }

        private string ConvertWordToImage(string filePath)
        {
            var doc = new Aspose.Words.Document(filePath);
            var imageStream = new MemoryStream();
            var options = new Aspose.Words.Saving.ImageSaveOptions(Aspose.Words.SaveFormat.Png)
            {
                PageSet = new Aspose.Words.Saving.PageSet(0)
            };
            doc.Save(imageStream, options);

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string imagePath = "/images/" + Guid.NewGuid() + ".png";
            string savePath = Path.Combine(directoryPath, Path.GetFileName(imagePath));

            using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                imageStream.WriteTo(fileStream);
            }

            return imagePath;
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
