using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Middleware.GlobalExceptionHandler
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            // Use the ExceptionHandler utility to create the error response
            var errorResponse = ExceptionHandler.CreateErrorResponse(context.Exception, _logger);

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = 500 // Internal Server Error
            };

            context.ExceptionHandled = true; // Mark the exception as handled
        }
    }
}
