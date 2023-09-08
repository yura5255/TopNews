using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.Entities;
using TopNews.Core.Entities.Site;
using TopNews.Core.Entities.User;
using TopNews.Infrastructure.Initializers;

namespace TopNews.Infrastructure.Context
{
    internal class AppDbContext : IdentityDbContext
    {
        public AppDbContext() : base() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<DashboardAccess> DashdoardAccesses { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.SeedCategoriesAndPosts();
            builder.SeedDashboardAccesses();

        }
    }
}
