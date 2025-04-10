using Microsoft.AspNetCore.Mvc;
using DALTW.Models;
using DALTW.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace DALTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class SemesterController : Controller
    {
        private readonly ISemesterRepository _repository;

        public SemesterController(ISemesterRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var semesters = await _repository.GetAllAsync();
            return View(semesters);
        }

        public async Task<IActionResult> Details(int id)
        {
            var semester = await _repository.GetByIdAsync(id);
            if (semester == null)
                return NotFound();
            return View(semester);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Semester semester)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(semester);
                return RedirectToAction(nameof(Index));
            }
            return View(semester);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var semester = await _repository.GetByIdAsync(id);
            if (semester == null)
                return NotFound();
            return View(semester);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Semester semester)
        {
            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(semester);
                return RedirectToAction(nameof(Index));
            }
            return View(semester);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var semester = await _repository.GetByIdAsync(id);
            if (semester == null)
                return NotFound();
            return View(semester);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}