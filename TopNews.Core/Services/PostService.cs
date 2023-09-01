using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.Post;
using TopNews.Core.Entities.Site;
using TopNews.Core.Entities.Specifications;
using TopNews.Core.Interfaces;

namespace TopNews.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IRepository<Post> _postRepo;

        public PostService(IConfiguration configuration, IRepository<Post> postRepo, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _postRepo = postRepo;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public async Task Create(PostDto model)
        {
            if (model.File.Count > 0)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + _configuration.GetValue<string>("ImageSettings:ImagePath");
                var files = model.File;
                string fileName = Guid.NewGuid().ToString();
                string extensions = Path.GetExtension(files[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extensions), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                model.ImagePath = fileName + extensions;
            }
            else
            {
                model.ImagePath = "Default.png";
            }

            DateTime currentDate = DateTime.Today;
            string formatedDate = currentDate.ToString("d");
            model.PublishDate = formatedDate;
            Post newpost = _mapper.Map<Post>(model);
            newpost.Category = null;
            await _postRepo.Insert(newpost);
            await _postRepo.Save();
        }

        public async Task Delete(int id)
        {
            var currentPost = await Get(id);

            if (currentPost == null) return; // exception

            string webPathRoot = _webHostEnvironment.WebRootPath;
            string upload = webPathRoot + _configuration.GetValue<string>("ImageSettings:ImagePath");

            string existingFilePath = Path.Combine(upload, currentPost.ImagePath);

            if (File.Exists(existingFilePath) && currentPost.ImagePath != "Default.png")
            {
                File.Delete(existingFilePath);
            }

            await _postRepo.Delete(id);
            await _postRepo.Save();
        }

        public async Task<PostDto?> Get(int id)
        {
            if (id < 0) return null; // exception handling

            var post = await _postRepo.GetByID(id);

            if (post == null) return null; // exception handling

            return _mapper.Map<PostDto>(post);
        }

        public async Task<List<PostDto>> GetAll()
        {
            var result = await _postRepo.GetListBySpec(new Posts.All());//.ToList();
            return _mapper.Map<List<PostDto>>(result);
        }

        public async Task<List<PostDto>> GetByCategory(int id)
        {
            var result = await _postRepo.GetListBySpec(new Posts.ByCategory(id));
            return _mapper.Map<List<PostDto>>(result);
        }

        public async Task<PostDto> GetById(int id)
        {
            if (id < 0) return null; // exception handling

            var post = await _postRepo.GetByID(id);

            if (post == null) return null; // exception handling

            return _mapper.Map<PostDto>(post);
        }

        public async Task<List<PostDto>> Search(string searchString)
        {
            var result = await _postRepo.GetListBySpec(new Posts.Search(searchString));
            return _mapper.Map<List<PostDto>>(result);
        }

        public async Task Update(PostDto model)
        {
            var currentPost = await _postRepo.GetByID(model.Id);
            if (model.File.Count > 0)
            {
                string webPathRoot = _webHostEnvironment.WebRootPath;
                string upload = webPathRoot + _configuration.GetValue<string>("ImageSettings:ImagePath");

                string existingFilePath = Path.Combine(upload, currentPost.ImagePath);

                if (File.Exists(existingFilePath) && model.ImagePath != "Default.png")
                {
                    File.Delete(existingFilePath);
                }

                var files = model.File;

                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                model.ImagePath = fileName + extension;

            }
            else
            {
                model.ImagePath = currentPost.ImagePath;
            }
            Post newpost = _mapper.Map<Post>(model);
            newpost.Category = null;
            await _postRepo.Update(newpost);
            await _postRepo.Save();
        }
    }
}
