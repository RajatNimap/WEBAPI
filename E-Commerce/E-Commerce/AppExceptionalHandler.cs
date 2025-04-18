using Microsoft.AspNetCore.Diagnostics;

namespace E_Commerce
{
    public class AppExceptionalHandler : IExceptionHandler
    {
        public async ValueTask<bool>TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {


           
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync("something went wrong");
            return true;
        }

        
    }
}
