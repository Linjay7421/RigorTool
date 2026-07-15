using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Web.Library.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        // When 1 vs 1 handler to validator. or you should use Enumerable.
        private readonly IValidator<TRequest> _validator;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(ILogger<ValidationBehavior<TRequest, TResponse>> logger, IValidator<TRequest>? validator = null)
        {
            _validator = validator;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Check if handler has a validator.
            if (_validator is null)
            {
                return await next();
            }

            // Validate the request using the validator.
            var context = new ValidationContext<TRequest>(request);
            var result = await _validator.ValidateAsync(context, cancellationToken);

            if (!result.IsValid)
            {
                _logger.LogInformation("Request/Command invalid.");
                throw new ValidationException(result.Errors.ToList());
            }

            _logger.LogInformation("Request/Command valid.");
            
            return await next();
        }
    }
}
