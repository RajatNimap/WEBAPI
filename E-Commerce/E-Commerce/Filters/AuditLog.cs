using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_Commerce.Filters
{
    public class AuditLog :Attribute, IActionFilter
    {
        private readonly ILogger<AuditLog> _logger;
        public AuditLog(ILogger<AuditLog> logger)
        {
            _logger = logger;   
        }

        private Stopwatch _stopwatch;


        public void OnActionExecuting(ActionExecutingContext context)
        {

            _stopwatch = Stopwatch.StartNew();
            var actionName = context.ActionDescriptor.DisplayName;
            var controller = context.Controller.GetType().Name;
            // throw new NotImplementedException();
            _logger.LogInformation($" method name : {actionName}, controllername : {controller}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            var actionName = context.ActionDescriptor.DisplayName;
            var controller = context.Controller.GetType().Name;
            var elapsed = _stopwatch.ElapsedMilliseconds;
            _logger.LogInformation($" method name : {actionName}, controllername : {controller} {elapsed}");

        }
    }
}
