
using System.Diagnostics;

namespace Hospital_Management.Middleware
{
    public class LoggingMiddleware
    {
        //private readonly RequestDelegate _next;

        //public LoggingMiddleware(RequestDelegate next)
        //{
        //    _next = next;
        //}
        //public async Task InvokeAsync(HttpContext context, ILogger<LoggingMiddleware> logger)
        //{
        //    logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
        //    await _next(context);
        //    logger.LogInformation("Finished handling request.");
        //}

        //public Task InvokeAsync(HttpContext context, RequestDelegate next)
        //{
        //    throw new NotImplementedException();
        //}

        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;


        }
        public async Task Invoke(HttpContext context)
        {
            var guid = Guid.NewGuid();

            var stopwatch = new Stopwatch();
            stopwatch.Start();      
            _logger.LogInformation($"handling request {context.Request.Method} {context.Request.Path} GUID {guid} {DateTime.Now} ");
            //try
            //{

                await _next(context);

            //}catch(Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}
            _logger.LogInformation($"finished handling request {context.Request.Method} {context.Request.Path} with status code {context.Response.StatusCode}  GUID {guid} {DateTime.Now}  {stopwatch.ElapsedMilliseconds}");

        }
    }   
   
}
