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

        public PostService(IRepository<Post> postRepo, IMapper mapper, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
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
                var fileName = Guid.NewGuid().ToString();
                string extansions = Path.GetExtension(files[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extansions), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                model.ImagePath = fileName + extansions;
            }
            else
            {
                model.ImagePath = "Default.png";
            }

            DateTime currentDate = DateTime.Today;
            string formatedDate = currentDate.ToString("d");
            model.PublishDate = formatedDate;
            await _postRepo.Insert(_mapper.Map<Post>(model));
            await _postRepo.Save();
        }

        public async Task<PostDto?> Get(int id)
        {
            if (id < 0) return null; // exception handling

            var post = await _postRepo.GetByID(id);

            if (post == null) return null; // exception handling

            return _mapper.Map<PostDto>(post);
        }

        public async Task<List<PostDto>> GetByCategory(int id)
        {
            var result = await _postRepo.GetListBySpec(new Posts.ByCategory(id));
            return _mapper.Map<List<PostDto>>(result);
        }
    }
}
