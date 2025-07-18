
using System.Diagnostics;

namespace E_Commerce
{
    public class CustomMiddleware : IMiddleware
    {
        //private readonly RequestDelegate _next;
         private readonly ILogger<CustomMiddleware> _logger;

        public CustomMiddleware(RequestDelegate next,ILogger<CustomMiddleware> logger)
        {
            //_next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context,RequestDelegate request)
        {
            // await _next(context);
            var _stopwatch = Stopwatch.StartNew();
            _stopwatch.Start();
            var guid = Guid.NewGuid();
            _logger.LogInformation($"{guid} {_stopwatch}  {context.Request.Path} {context.Request.Method} ");
            Console.WriteLine("Custom Middleware Logic Before Next Middleware");
            try
            {
                await request(context); //call for the next Delegate
               // Call the next middleware in the pipeline
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            _stopwatch.Stop();
            _logger.LogInformation($"{guid} {_stopwatch.ElapsedMilliseconds}  {context.Response.StatusCode} ");
            Console.WriteLine("Custom Middleware Logic after the next middleware");

        }
    }

    //public class CustomMiddlewarebyDelegate
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly ILogger<CustomMiddlewarebyDelegate> _logger;
    //    public CustomMiddlewarebyDelegate(RequestDelegate next,ILogger<CustomMiddlewarebyDelegate> logger)
    //    {
    //        _next = next;
    //        _logger = logger;
    //    }
    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        // Custom logic before the next middleware

    //        var _stopwatch = Stopwatch.StartNew();
    //        _stopwatch.Start();

    //        var guid = Guid.NewGuid();  

    //        _logger.LogInformation($"{guid} {_stopwatch}  {context.Request.Path} {context.Request.Method} ");
    //        Console.WriteLine("Custom Middleware Logic Before Next Middleware");
    //        try
    //        {
    //            await _next(context); //call for the next Delegate
    //                                  // Call the next middleware in the pipeline
    //        }
    //        catch (Exception ex) {

    //            Console.WriteLine(ex);
    //        }
    //        _stopwatch.Stop();
    //        _logger.LogInformation($"{guid} {_stopwatch.ElapsedMilliseconds}  {context.Response.StatusCode} ");
    //        Console.WriteLine("Custom Middleware Logic after the next middleware");
    //    }
    //}

}
