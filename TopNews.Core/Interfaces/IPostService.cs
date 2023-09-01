using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.Post;

namespace TopNews.Core.Interfaces
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAll();
        Task<PostDto?> Get(int id);
        Task Create(PostDto model);
        Task Update(PostDto model);
        Task Delete(int id);
        Task<List<PostDto>> GetByCategory(int id);
        Task<PostDto> GetById(int id);
        Task<List<PostDto>> Search(string searchString);
    }
}
