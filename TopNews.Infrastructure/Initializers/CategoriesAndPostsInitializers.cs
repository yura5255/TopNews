using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.Entities.Site;

namespace TopNews.Infrastructure.Initializers
{
    public static class CategoriesAndPostsInitializers
    {
        public static void SeedCategoriesAndPosts(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(new Category[]
            {
                new Category { Id = 1, Name = "Sport"},
                new Category { Id = 2, Name = "IT"},
                new Category { Id = 3, Name = "Cars"}
            });
        }
    }
}
