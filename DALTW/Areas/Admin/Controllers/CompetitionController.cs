using Microsoft.AspNetCore.Mvc;
using DALTW.Models;
using DALTW.Repositories;

namespace DALTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompetitionController : Controller
    {
        private readonly ICompetitionRepository _repository;

        public CompetitionController(ICompetitionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var competitions = await _repository.GetAllAsync();
            return View(competitions);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Competition competition)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(competition);
                return RedirectToAction(nameof(Index));
            }
            return View(competition);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var competition = await _repository.GetByIdAsync(id);
            if (competition == null)
                return NotFound();

            return View(competition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Competition competition)
        {
            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(competition);
                return RedirectToAction(nameof(Index));
            }
            return View(competition);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var competition = await _repository.GetByIdAsync(id);
            if (competition == null)
                return NotFound();

            return View(competition);
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
            var competition = await _repository.GetByIdAsync(id);
            if (competition == null)
                return NotFound();

            return View(competition);
        }
    }
}
