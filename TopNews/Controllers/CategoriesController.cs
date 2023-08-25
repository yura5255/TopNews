using Microsoft.AspNetCore.Mvc;
using TopNews.Core.DTOs.Category;
using TopNews.Core.DTOs.User;
using TopNews.Core.Interfaces;
using TopNews.Core.Services;
using TopNews.Core.Validation.User;

namespace TopNews.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _categoryService.GetAll();
            return View(result);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto model)
        {
            await _categoryService.Create(model);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.Get(id);
            return View(result);
        }
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoryService.Get(id);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryDto model)
        {
            await _categoryService.Update(model);
            return RedirectToAction(nameof(Index));
        }


    }
}
