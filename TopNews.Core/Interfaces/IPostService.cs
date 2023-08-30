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
        Task<PostDto?> Get(int id);
        Task<List<PostDto>> GetByCategory(int id);
        Task Create(PostDto model);
    }
}
