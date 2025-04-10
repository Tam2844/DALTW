
using Microsoft.AspNetCore.Mvc;
using DALTW.Models;
using DALTW.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace DALTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class GradeController : Controller
    {
        private readonly IGradeRepository _repository;

        public GradeController(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var grades = await _repository.GetAllAsync();
            return View(grades);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grade grade)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(grade);
                return RedirectToAction(nameof(Index));
            }
            return View(grade);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var grade = await _repository.GetByIdAsync(id);
            if (grade == null)
                return NotFound();

            return View(grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Grade grade)
        {
            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(grade);
                return RedirectToAction(nameof(Index));
            }
            return View(grade);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var grade = await _repository.GetByIdAsync(id);
            if (grade == null)
                return NotFound();

            return View(grade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var grade = await _repository.GetByIdAsync(id);
            if (grade == null)
                return NotFound();

            return View(grade);
        }
    }
}