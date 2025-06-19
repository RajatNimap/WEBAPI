using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_Commerce.Filters
{
    public class Exception :Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var model = new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "something went wrong",
                Timestamp = DateTime.UtcNow 

            };

            context.Result=new JsonResult(model);

            throw new NotImplementedException();
        }
    }
}
