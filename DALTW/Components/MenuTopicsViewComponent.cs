using DALTW.Models;
using Microsoft.AspNetCore.Mvc;

public class MenuTopicsViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    public MenuTopicsViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var topics = _context.Topics.ToList();
        return View(topics);
    }
}
