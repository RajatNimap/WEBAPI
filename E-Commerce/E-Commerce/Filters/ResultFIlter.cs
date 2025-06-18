using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_Commerce.Filters
{
    public class ResultFIlter :Attribute, IResultFilter
    {
        private readonly ILogger<ResultFIlter> _logger;
        public ResultFIlter(ILogger<ResultFIlter> logger)
        {
            _logger = logger;
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is DateTime dt)
            {
                dt = DateTime.Now;
                var json = JsonSerializer.Serialize(dt);
                _logger.LogInformation($"{json} this is from the json");
            }
            //throw new NotImplementedException();
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var dat=DateTime.Now;
            var actionName = context.ActionDescriptor.DisplayName;

            _logger.LogInformation($"{dat.ToString()} {actionName}");
        }
    }
}
