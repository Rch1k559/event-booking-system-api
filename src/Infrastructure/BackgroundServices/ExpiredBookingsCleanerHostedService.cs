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
                    // ВАЖНО: BackgroundService зарегистрирован как Singleton (живет всё время работы приложения). 
                    // DbContext и обработчики MediatR зарегистрированы как Scoped. 
                    // Внедрять Scoped-сервисы напрямую в конструктор Singleton нельзя (Captive Dependency). 
                    // Поэтому на каждой итерации цикла вручную создается новый Scope и извлекается IMediator
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
