﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TopNews.Core.DTOs.Category;
using TopNews.Core.DTOs.Post;
using TopNews.Core.Interfaces;
using TopNews.Core.Validation.Post;
using X.PagedList;

namespace TopNews.Web.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IPostService _postService;

        public PostController(ICategoryService categoryService, IPostService postService)
        {
            _categoryService = categoryService;
            _postService = postService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> IndexAsync(int? page)
        {
            List<PostDto> posts = await _postService.GetAll();
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }


        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostDto model)
        {
            var validator = new CreatePostValidation();
            var validationResult = await validator.ValidateAsync(model);

            if (validationResult.IsValid)
            {
                //byte[] fileBytes = Convert.FromBase64String(CroppedImageData);
                //IFormFile croppedImageFile = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "CroppedImage", "cropped-image.jpg");

                // Create an IFormFileCollection and add the cropped image to it
                //var formFiles = new FormFileCollection();
                //formFiles.Add(croppedImageFile);
                var files = HttpContext.Request.Form.Files;
                model.File = files;
                //model.File = formFiles;
                await _postService.Create(model);
                return RedirectToAction("Index", "Post");
            }
            ViewBag.Errors = validationResult.Errors[0];
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var posts = await _postService.Get(id);

            if (posts == null) return NotFound();

            await LoadCategories();
            return View(posts);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostDto model)
        {
            var validator = new CreatePostValidation();
            var validationResult = await validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                model.File = files;
                await _postService.Update(model);
                return RedirectToAction("Index", "Post");
            }
            ViewBag.CreatePostError = validationResult.Errors[0];
            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var postDto = await _postService.Get(id);

            if (postDto == null)
            {
                ViewBag.AuthError = "Post not found.";
                return View();
            }
            return View(postDto);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteById(int id)
        {
            await _postService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PostsByCategory(int id)
        {
            List<PostDto> posts = await _postService.GetByCategory(id);
            int pageSize = 20;
            int pageNumber = 1;
            return View("Index", posts.ToPagedList(pageNumber, pageSize));
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


        private async Task LoadCategories()
        {
            ViewBag.CategoryList = new SelectList(
                await _categoryService.GetAll(),
                nameof(CategoryDto.Id),
                nameof(CategoryDto.Name)
                );
            ;
        }
    }
}
