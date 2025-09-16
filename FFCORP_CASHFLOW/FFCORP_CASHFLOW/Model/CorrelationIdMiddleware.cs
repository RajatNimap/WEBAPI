namespace FFCORP_CASHFLOW.Model
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Generate or retrieve correlation ID
            var correlationId = Guid.NewGuid().ToString();

            // Store in HttpContext.Items (per-request storage)
            context.Items["CorrelationId"] = correlationId;

            // Log the start of request
            _logger.LogInformation($"[START] Request {context.Request.Method} {context.Request.Path} | CorrelationId: {correlationId}");

            try
            {
                await _next(context); // Continue pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[EXCEPTION] CorrelationId: {correlationId}");

                // You can attach exception to Items too (optional)
                context.Items["Exception"] = ex;

                throw; // Rethrow
            }
            finally
            {
                _logger.LogInformation($"[END] Request CorrelationId: {correlationId}");
            }
        }
    }
}
