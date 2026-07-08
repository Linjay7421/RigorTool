using MediatR;

namespace Web.Public.Common.Behaviors
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
            // Log the request being handled.
            _logger.LogInformation($"Processing request of type {typeof(TRequest).Name}");
            Console.WriteLine($"Processing request of type {typeof(TRequest).Name}");
            // Pass control to next handler or behavior in the pipeline.
            var reponse = await next();

            // Log the response after processing.
            _logger.LogInformation($"Completed handling request, response type: {typeof(TResponse).Name}");
            Console.WriteLine($"Completed handling request, response type: {typeof(TResponse).Name}");
            return reponse;
        }
    }
}
