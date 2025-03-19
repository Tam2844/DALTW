using DALTW.Models;
using DALTW.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DALTW.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly ITopicRepository _topicRepository;
        public DocumentController(IDocumentRepository documentRepository,
ICategoryRepository categoryRepository,
IGradeRepository gradeRepository,
ITopicRepository topicRepository)
        {
            _documentRepository = documentRepository;
            _categoryRepository = categoryRepository;
            _gradeRepository = gradeRepository;
            _topicRepository = topicRepository;
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _documentRepository.GetAllAsync();
            if (documents == null) // Kiểm tra nếu null
            {
                documents = new List<Document>(); // Tránh lỗi NullReferenceException
            }

            return View(documents);
        }
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var grades = await _gradeRepository.GetAllAsync();
            var topics = await _topicRepository.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "Name");
            ViewBag.Grades = new SelectList(grades, "GradeID", "Name");
            ViewBag.Topics = new SelectList(topics, "TopicID", "TopicName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Document document, IFormFile file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null)
                    {
                        document.FileURL = await SaveFile(file);
                    }

                    await _documentRepository.AddAsync(document); // Lưu vào DB
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi lưu vào database: " + ex.Message);
            }

            var categories = await _categoryRepository.GetAllAsync();
            var grades = await _gradeRepository.GetAllAsync();
            var topics = await _topicRepository.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "Name");
            ViewBag.Grades = new SelectList(grades, "GradeID", "Name");
            ViewBag.Topics = new SelectList(topics, "TopicID", "TopicName");

            return View(document);
        }
        private async Task<string> SaveFile(IFormFile file)
        {
            var savePath = Path.Combine("wwwroot/DocFile", file.FileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            { 
                await file.CopyToAsync(fileStream);
            }

            return "/DocFile/" + file.FileName; 
        }


    }
}
