using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DALTW.Models;
using DALTW.Repositories;

namespace DALTW.Controllers;

public class HomeController : Controller
{

    private readonly ITrafficLogRepository _trafficLogRepository;

    public HomeController(ITrafficLogRepository trafficLogRepository)
    {
        _trafficLogRepository = trafficLogRepository;
    }

    public async Task<IActionResult> Index()
    {
        int totalVisits = await _trafficLogRepository.GetTotalVisitsAsync();
        ViewBag.TotalVisits = totalVisits;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
