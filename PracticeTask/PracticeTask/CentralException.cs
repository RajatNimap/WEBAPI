using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PracticeTask.Model;

namespace PracticeTask
{
    public class CentralException : IExceptionHandler
    {
        

        public async ValueTask<bool>TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
        {

            var response = new Excetiton_Model
            {
                    statuscode=StatusCodes.Status500InternalServerError,
                    message="Something went wrong try again Later",
                    Timestamp=DateTime.UtcNow, 
            };
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";
             httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }

      
    }
}
