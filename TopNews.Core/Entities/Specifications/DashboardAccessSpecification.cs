using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopNews.Core.Entities.Specifications
{
    public class DashboardAccessSpecification
    {
        public class GetByIpAddress : Specification<DashboardAccess> 
        {
            public GetByIpAddress(string ipAddress)
            {
                Query.Where(da => da.IpAddress == ipAddress);
            }
        }
    }
}
