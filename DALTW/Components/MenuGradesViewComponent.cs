using Microsoft.AspNetCore.Mvc;
using DALTW.Models;

namespace DALTW.ViewComponents
{
    public class MenuGradesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public MenuGradesViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var grades = _context.Grades.ToList();
            var semesters = _context.Semesters.ToList();
            ViewBag.Grades = grades;
            ViewBag.Semesters = semesters;

            return View(); // mặc định tìm Views/Shared/Components/MenuGrades/Default.cshtml
        }
    }
}
