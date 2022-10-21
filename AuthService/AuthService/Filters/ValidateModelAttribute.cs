using AuthService.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Filters
{
    public class ValidateModelAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var response = new ErrorModel()
                {
                    success = false,
                    error = "One or more validation errors occurred."
                };

                context.HttpContext.Response.ContentType = "application/json";
                context.Result = new ObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

            context.HttpContext.Response.ContentType = "application/json";
        }
    }

}
