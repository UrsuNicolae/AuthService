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
                    var response  = new ErrorModel()
                    {
                        Success = false,
                        Error = "One or more validation errors occurred."
                    };
                    
                    context.Result = new ObjectResult(response);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }
        }
    
}
