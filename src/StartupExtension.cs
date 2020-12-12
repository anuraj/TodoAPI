using System;
using System.Collections.Generic;
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
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri("https://login.microsoftonline.com/7493ef9e-db24-45d8-91b5-9c36018d6d52/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri("https://login.microsoftonline.com/7493ef9e-db24-45d8-91b5-9c36018d6d52/oauth2/v2.0/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "api://fe829e9f-ea47-4178-a0d3-9c07bb70de24/Manage.TodoItems", "Manage Todo Items" }
                        }
                    }
                }
            });
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

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        },
                        Scheme = "oauth2",
                        Name = "oauth2",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
}