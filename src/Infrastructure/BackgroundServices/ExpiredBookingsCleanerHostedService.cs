using Application.Events.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.BackgroundServices
{
    public class ExpiredBookingsCleanerHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredBookingsCleanerHostedService> _logger;

        public ExpiredBookingsCleanerHostedService(IServiceProvider service, ILogger<ExpiredBookingsCleanerHostedService> logger)
        {
            _serviceProvider = service;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background service is started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // CRITICAL: BackgroundService is registered as a Singleton (lives for the application lifetime). 
                    // DbContext and MediatR handlers are registered as Scoped. 
                    // Injecting Scoped services directly into a Singleton constructor causes a Captive Dependency bug. 
                    // Therefore, a new IServiceScope is created manually per iteration to resolve IMediator.
                    _logger.LogInformation("Background service is live");

                    using var scope = _serviceProvider.CreateScope();

                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    // Отправляем команду очистки просроченных броней
                    await mediator.Send(new CancelExpiredBookingsCommand(), stoppingToken);

                    // Задержка выполнения на 1 минуту перед следующей проверкой
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Корректный перехват отмены при остановке приложения
                    break;
                }
            }
        }
    }
}
