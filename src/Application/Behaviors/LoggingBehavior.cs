using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var correlationId = Guid.NewGuid();

            logger.LogInformation("Handling {RequestName} [{CorrelationId}]", requestName, correlationId);

            var timer = Stopwatch.StartNew();

            var response = await next();

            timer.Stop();

            logger.LogInformation("Handled {RequestName} [{CorrelationId}] in {ElapsedMilliseconds} ms",
                requestName, correlationId, timer.ElapsedMilliseconds);

            return response;
        }
    }
}
