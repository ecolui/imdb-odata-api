using System;

//using System.Data.Entity;
using System.IO;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    public class imdbContext: DbContext
    {
        public imdbContext(DbContextOptions<imdbContext> options) : base(options){}

        public DbSet<Media> Media { get; set; }
        public DbSet<MediaRating> MediaRating { get; set; }

        public DbSet<Staff> Staff { get; set; }
        public DbSet<Profession> Profession { get; set; }
        public DbSet<Principal> Principal { get; set; }
       
    }

    public class imdbContextFactory : IDesignTimeDbContextFactory<imdbContext>
    {

        public imdbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()                
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var Configuration = builder.Build();

            var connectionString = Configuration["IMDBConnectionStr"];
        
            var optionsBuilder = new DbContextOptionsBuilder<imdbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new imdbContext(optionsBuilder.Options);            
        }
    }
}
