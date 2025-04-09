using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DALTW.Models;
using DALTW.Repositories;

namespace DALTW.Controllers;

public class HomeController : Controller
{
    private readonly ITrafficLogRepository _trafficLogRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IWebHostEnvironment _env;

    public HomeController(ITrafficLogRepository trafficLogRepository, IDocumentRepository documentRepository, IWebHostEnvironment env)
    {
        _trafficLogRepository = trafficLogRepository;
        _documentRepository = documentRepository;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        int totalVisits = await _trafficLogRepository.GetTotalVisitsAsync();
        ViewBag.TotalVisits = totalVisits;

        var documents = await _documentRepository.GetNewestDocumentsAsync(6);

        // CH? THÊM ?O?N NÀY:
        foreach (var document in documents)
        {
            if (document.FileURL != null && document.FileURL.EndsWith(".docx"))
            {
                string filePath = Path.Combine(_env.WebRootPath, document.FileURL.TrimStart('/'));
                document.ImageFilePath = ConvertWordToImage(filePath);
            }
        }

        return View(documents);
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

        string directoryPath = Path.Combine(_env.WebRootPath, "images");
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
}
