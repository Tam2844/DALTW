using Microsoft.AspNetCore.Mvc;
using DALTW.Models;

namespace DALTW.ViewComponents
{
    public class MenuCompetitionsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public MenuCompetitionsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var competitions = _context.Competitions.ToList();
            return View(competitions);
        }
    }
}
