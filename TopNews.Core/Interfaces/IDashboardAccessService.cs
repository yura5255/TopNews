using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.Ip;

namespace TopNews.Core.Interfaces
{
    public interface IDashboardAccessService
    {
        Task<List<DashboardAccessDto>> GetAll();
        void Create(DashboardAccessDto model);
        void Delete(int id);
        Task<DashboardAccessDto?> Get(string IpAddress);
        Task<DashboardAccessDto?> Get(int id);
    }
}
