using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TopNews.Core.DTOs.Post;
using TopNews.Core.Interfaces;
using TopNews.Models;
using X.PagedList;

namespace TopNews.Controllers
{
    public class HomeController : Controller
    {

        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;
        public HomeController(IPostService postService, ICategoryService categoryService)
        {
            _categoryService = categoryService;
            _postService = postService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            List<PostDto> posts = (await _postService.GetAll()).OrderByDescending(p => p.Id).ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View("Index", posts.ToPagedList(pageNumber, pageSize));
        }


        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("NotFound");
                default:
                    return View("Error");
            }
        }
        public async Task<IActionResult> ReadMore(int? id)
        {
            PostDto? post = await _postService.Get(id ?? 0);
            if (post == null)
            {
                return RedirectToAction(nameof(Index));
            }
            post.CategoryName = (await _categoryService.Get(post.CategoryId)).Name;
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([FromForm] string searchString)
        {
            List<PostDto> posts = await _postService.Search(searchString);
            int pageSize = 20;
            int pageNumber = 1;
            return View("Index", posts.ToPagedList(pageNumber, pageSize));
        }
    }
}