using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace E_Commerce.Filters
{
    public class ResourceFilter : Attribute, IResourceFilter
    {
        private readonly ILogger<ResourceFilter> _logger;

        public ResourceFilter(ILogger<ResourceFilter> logger)
        {
            _logger = logger;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var cache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
            var key = context.HttpContext.Request.Path.ToString();

            if (cache != null && cache.TryGetValue(key, out IActionResult cachedResult))
            {
                _logger.LogInformation("Serving from cache: {Path}", key);
                context.Result = cachedResult;
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            var cache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
            var key = context.HttpContext.Request.Path.ToString();

            if (cache != null && context.Result != null && !cache.TryGetValue(key, out _))
            {
                cache.Set(key, context.Result, TimeSpan.FromMinutes(10));
                _logger.LogInformation("Cached result for: {Path}", key);
            }
        }
    }
}
