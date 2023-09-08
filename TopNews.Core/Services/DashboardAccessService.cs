using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOs.Ip;
using TopNews.Core.Entities;
using TopNews.Core.Entities.Specifications;
using TopNews.Core.Interfaces;

namespace TopNews.Core.Services
{
    internal class DashboardAccessService : IDashboardAccessService
    {
        private readonly IRepository<DashboardAccess> _ipRepo;
        private readonly IMapper _mapper;
        public DashboardAccessService(IRepository<DashboardAccess> ipRepo, IMapper mapper)
        {
            _ipRepo = ipRepo;
            _mapper = mapper;
        }

        public async void Create(DashboardAccessDto model)
        {
            await _ipRepo.Insert(_mapper.Map<DashboardAccess>(model));
        }

        public async void Delete(int id)
        {
            await _ipRepo.Delete(id);
        }

        public async Task<DashboardAccessDto?> Get(string IpAddress)
        {
            return _mapper.Map<DashboardAccessDto>(await _ipRepo.GetItemBySpec(new DashboardAccessSpecification.GetByIpAddress(IpAddress)));
        }

        public async Task<DashboardAccessDto?> Get(int id)
        {
            return _mapper.Map<DashboardAccessDto>(await _ipRepo.GetByID(id));
        }

        public async Task<List<DashboardAccessDto>> GetAll()
        {
            return _mapper.Map<List<DashboardAccessDto>>(_ipRepo.GetAll());
        }

    }
}
