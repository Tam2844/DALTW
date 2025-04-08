using Microsoft.AspNetCore.Mvc;
using DALTW.Models;
using DALTW.Repositories;

namespace DALTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TopicController : Controller
    {
        private readonly ITopicRepository _repository;

        public TopicController(ITopicRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var topics = await _repository.GetAllAsync();
            return View(topics);
        }

        public async Task<IActionResult> Details(int id)
        {
            var topic = await _repository.GetByIdAsync(id);
            if (topic == null)
                return NotFound();
            return View(topic);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TopicName")] Topic topic)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Để xem lỗi
                }
                return View(topic);
            }

            await _repository.AddAsync(topic);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var topic = await _repository.GetByIdAsync(id);
            if (topic == null)
                return NotFound();
            return View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Topic topic)
        {
            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(topic);
                return RedirectToAction(nameof(Index));
            }
            return View(topic);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var topic = await _repository.GetByIdAsync(id);
            if (topic == null)
                return NotFound();
            return View(topic);
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
