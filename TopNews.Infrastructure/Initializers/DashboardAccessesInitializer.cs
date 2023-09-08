using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.Entities;

namespace TopNews.Infrastructure.Initializers
{
    internal static class DashboardAccessesInitializer
    {
        public static void SeedDashboardAccesses(this ModelBuilder model)
        {
            model.Entity<DashboardAccess>().HasData(
                new DashboardAccess()
                {
                    Id = 1,
                    IpAddress = "0.0.0.0",
                }
            );
        }
    }
}
