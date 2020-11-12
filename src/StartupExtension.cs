using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi.Filters;

public static class StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddSwaggerSupport(this IServiceCollection services)
    {
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

        return services;
    }
}