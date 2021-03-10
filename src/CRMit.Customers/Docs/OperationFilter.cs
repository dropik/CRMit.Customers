using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CRMit.Customers.Docs
{
    public class OperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.Add("500", new OpenApiResponse()
            {
                Description = "Internal error occured."
            });

            if (operation.Responses.ContainsKey("400"))
            {
                operation.Responses["400"].Description = "Invalid parameters passed.";
            }

            if (operation.Responses.ContainsKey("404"))
            {
                operation.Responses["404"].Description = "Customer not found.";
            }
        }
    }
}
