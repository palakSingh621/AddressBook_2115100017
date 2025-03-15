using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Middleware.GlobalExceptionHandler
{
    public class ExceptionHandler
    {
        public static string HandleException(Exception ex, ILogger logger, out object errorResponse)
        {
            logger.LogError(ex, "An error occurred in the application");

            errorResponse = new
            {
                Success = false,
                Message = "An error occurred",
                Error = ex.Message
            };

            return JsonConvert.SerializeObject(errorResponse);
        }

        public static object CreateErrorResponse(Exception ex, ILogger logger)
        {
            logger.LogError(ex, "An error occurred in the application");

            return new
            {
                Success = false,
                Message = "An error occurred",
                Error = ex.Message
            };
        }
    }
}
