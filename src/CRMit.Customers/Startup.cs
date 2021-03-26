using CRMit.Customers.Docs;
using CRMit.Customers.Extensions;
using CRMit.Customers.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace CRMit.Customers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CRMit.Customers",
                    Version = "v1",
                    Description = "Customer resource in CRMit API.",
                    Contact = new OpenApiContact
                    {
                        Name = "Daniil Ryzhkov",
                        Email = "drop.sovet@gmail.com",
                        Url = new Uri("https://github.com/dropik/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "The MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

#if DEBUG
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "CRMit.Customers.xml");
                c.IncludeXmlComments(xmlPath);
#endif

                c.OperationFilter<OperationFilter>();
            });

            services.AddDbContext<CustomersDbContext>(optionsBuilder =>
            {
                var server = Configuration["Db:Server"];
                var port = Configuration["Db:Port"];
                var database = Configuration["Db:Database"];
                var user = Configuration["Db:User"];
                var password = Configuration["Db:Password"];
                var connectionString = $"Server={server};Port={port};Database={database};Uid={user};Pwd={password}";
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
                {
                    options.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });

            services.AddStartupTask<DatabaseMigrator>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRMit.Customers v1"));
            }

            if (env.IsStaging() || env.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
