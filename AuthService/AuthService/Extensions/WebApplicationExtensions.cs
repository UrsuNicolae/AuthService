using AuthService.Models;
using Newtonsoft.Json;
using System.Text;

namespace AuthService.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void UseStatusCodePagesWithCustomResult(this WebApplication app)
        {
            app.UseStatusCodePages(async context => {

                string json = JsonConvert.SerializeObject(new ErrorModel()
                {
                    Success = false,
                    Error = "Route not found."
                });

                var bytes = Encoding.UTF8.GetBytes(json);
                context.HttpContext.Response.StatusCode = 200;
                await context.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            });
        }
    }
}
