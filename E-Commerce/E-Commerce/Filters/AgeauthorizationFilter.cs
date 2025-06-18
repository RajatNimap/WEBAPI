using System.Security.Claims;
using E_Commerce.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace E_Commerce.Filters
{
    public class AgeauthorizationFilter : Attribute,IAuthorizationFilter
    {
        private readonly int Minage;
        public AgeauthorizationFilter(int Minage)
        {
            this.Minage = Minage;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user=context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                //  context.Result = new UnauthorizedResult();
                context.Result = new JsonResult(new
                {
                    message = $"access denied due to the unauthorization",
                    statusCodes=StatusCodes.Status403Forbidden,

                });
               
                return;
            }

            var data=user.Claims.FirstOrDefault(x=>x.Type == "Age" );
            
            if(data == null || !int.TryParse(data.Value,out int userAge)){
                context.Result = new ForbidResult();
                return;
            }
            if(userAge < Minage)
            {
                context.Result = new ContentResult
                {
                     StatusCode = StatusCodes.Status403Forbidden,
                     Content=$"access denied your age must be greater than {Minage}",
                     ContentType="text/plain"
                };    
            }
        }
    }
}
