using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TodoApi.Filters
{
    [ExcludeFromCodeCoverage]
    public class RemoveVersionFromParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }
            
            var versionParameter = operation.Parameters.Single(p => p.Name == "version");
            operation.Parameters.Remove(versionParameter);
        }
    }
}