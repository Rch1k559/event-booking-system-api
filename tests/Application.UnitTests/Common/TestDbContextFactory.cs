using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Common
{
    public static class TestDbContextFactory
    {
        public static AppDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
