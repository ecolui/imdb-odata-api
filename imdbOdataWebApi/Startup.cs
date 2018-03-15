using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.OData;
using Microsoft.AspNet.OData.Extensions;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;


namespace imdbOdataWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionStr"];

            services.AddDbContext<imdbContext>(optionsAction =>
                optionsAction.UseSqlServer(connectionString));
            
            services.AddOData();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env
                              ,IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // configure OData model
            var builder = new Microsoft.AspNet.OData.Builder.ODataConventionModelBuilder(serviceProvider);

            builder.EntitySet<Media>("Media").EntityType
                .OrderBy(
                    nameof(Media.MediaId),
                    nameof(Media.primaryTitle))
                .Filter(
                    nameof(Media.MediaId));

            builder.EntitySet<Staff>("Staff").EntityType
                .OrderBy(
                    nameof(Staff.StaffId),
                    nameof(Staff.PrimaryName))
                .Filter(
                    nameof(Staff.StaffId));

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
                routeBuilder.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
            });
        }
    }
}
