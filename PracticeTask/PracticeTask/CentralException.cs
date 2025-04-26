
using Microsoft.AspNetCore.Diagnostics;
using PracticeTask.Model;

namespace PracticeTask
{
    public class CentralException :IExceptionHandler
    {
       public async ValueTask<bool>TryHandleAsync(HttpContext context,Exception exception,CancellationToken cancellationToken)
        {

            var exdata = new Excetiton_Model
            {
                statuscode = StatusCodes.Status500InternalServerError,
                message = "Server is down please try again letter",
                Timestamp = DateTime.Now,
            };
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType= "application/json";   
            await context.Response.WriteAsJsonAsync(exdata,cancellationToken);
            return true;
       }
    }
}
