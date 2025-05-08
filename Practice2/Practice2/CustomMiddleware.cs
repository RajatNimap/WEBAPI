
namespace Practice2
{
    public class CustomMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("I am from the custom middlewar \n");
            await next(context);
            await context.Response.WriteAsync("I am from the backward pipeline from middleware \n");

        }
    }
}
