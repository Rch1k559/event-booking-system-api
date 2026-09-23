using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class 
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                // OPTIMIZATION: Execute all registered FluentValidation rules concurrently 
                // using Task.WhenAll to minimize request latency.
                var results = await Task.WhenAll(
                    validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                // Aggregate errors from all validators into a single flat list
                var failures = results
                    .SelectMany(r => r.Errors)
                    .Where(f => f is not null)
                    .ToList();

                // Short-circuit the request pipeline and throw exception if validation fails
                if (failures.Count > 0)
                {
                    throw new ValidationException(failures);
                }
            }
            // Proceed to the next behavior or handler if validation succeeds
            return await next();
        }
    }
}
