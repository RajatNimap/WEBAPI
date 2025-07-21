using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using System.Net;


namespace E_Commerce
{
        public class AppExceptionalHandler : IExceptionHandler
        {
            public async ValueTask<bool>TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
            {

                var response = new ErrorResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Something went wrong Please try again ",
                    Timestamp = DateTime.UtcNow,    
                };
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    httpContext.Response.ContentType = "application/json";

                    await httpContext.Response.WriteAsJsonAsync(response,cancellationToken);
                    return true;
            }

        
        }
}
