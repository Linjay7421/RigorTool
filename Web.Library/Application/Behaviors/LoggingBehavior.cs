using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Web.Library.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestId = Guid.CreateVersion7().ToString();
            var requestName = typeof(TRequest).Name;

            var stopwatch = Stopwatch.StartNew();
            Exception? exception = null;

            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                exception = ex;
                throw; // go to finally.
            }
            finally
            {
                stopwatch.Stop();

                if (exception is null)
                {
                    _logger.LogInformation(
                        "MediatR request completed. RequestName={RequestName}, RequestId={RequestId}, Status={Status}, ElapsedMs={ElapsedMs}",
                        requestName,
                        requestId,
                        "Success",
                        stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogError(
                        exception,
                        "MediatR request completed. RequestName={RequestName}, RequestId={RequestId}, Status={Status}, ElapsedMs={ElapsedMs}",
                        requestName,
                        requestId,
                        "Failed",
                        stopwatch.ElapsedMilliseconds);
                }
            }
        }
    }
}
