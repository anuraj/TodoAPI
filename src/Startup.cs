using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TodoApi.Models;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Linq;

namespace TodoApi
{
    [ExcludeFromCodeCoverage]
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
            var connectionString = Configuration.GetConnectionString("TodoDbConnection");

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("1.0", new OpenApiInfo
                {
                    Version = "1.0",
                    Title = "ToDo API",
                    Description = "Todo Application API",
                    Contact = new OpenApiContact
                    {
                        Name = "anuraj",
                        Email = "anuraj@dotnetthoughts.net",
                        Url = new Uri("https://twitter.com/anuraj"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://anuraj.mit-license.org/"),
                    }
                });

                options.SwaggerDoc("2.0", new OpenApiInfo
                {
                    Version = "2.0",
                    Title = "ToDo API",
                    Description = "Todo Application API",
                    Contact = new OpenApiContact
                    {
                        Name = "anuraj",
                        Email = "anuraj@dotnetthoughts.net",
                        Url = new Uri("https://twitter.com/anuraj"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://anuraj.mit-license.org/"),
                    }
                });

                options.SwaggerDoc("3.0", new OpenApiInfo
                {
                    Version = "3.0",
                    Title = "ToDo API",
                    Description = "Todo Application API",
                    Contact = new OpenApiContact
                    {
                        Name = "anuraj",
                        Email = "anuraj@dotnetthoughts.net",
                        Url = new Uri("https://twitter.com/anuraj"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://anuraj.mit-license.org/"),
                    }
                });

                options.OperationFilter<RemoveVersionFromParameter>();
                options.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                options.DocInclusionPredicate((version, desc) =>
                {
                    if (!desc.TryGetMethodInfo(out MethodInfo methodInfo))
                    {
                        return false;
                    }

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    var maps = methodInfo
                        .GetCustomAttributes(true)
                        .OfType<MapToApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions)
                        .ToArray();

                    return versions.Any(v => v.ToString() == version)
                           && (!maps.Any() || maps.Any(v => v.ToString() == version));
                });
            });

            services.AddDbContext<TodoApiDbContext>(options => options.UseSqlServer(connectionString));
            services.AddHealthChecks().AddDbContextCheck<TodoApiDbContext>();
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/1.0/swagger.json", $"1.0");
                c.SwaggerEndpoint($"/swagger/2.0/swagger.json", $"2.0");
                c.SwaggerEndpoint($"/swagger/3.0/swagger.json", $"3.0");
                c.RoutePrefix = string.Empty;
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
