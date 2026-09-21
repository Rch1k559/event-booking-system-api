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

                // ОПТИМИЗАЦИЯ: Запуск проверки всеми зарегистрированными FluentValidation-валидаторами 
                // параллельно через Task.WhenAll для ускорения выполнения
                var results = await Task.WhenAll(
                    validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                // Собираем ошибки из всех валидаторов в единый плоский список
                var failures = results
                    .SelectMany(r => r.Errors)
                    .Where(f => f is not null)
                    .ToList();

                // Если есть хотя бы одна ошибка, прерываем выполнение пайплайна и выбрасываем исключение
                if (failures.Count > 0)
                {
                    throw new ValidationException(failures);
                }
            }
            // Если валидация прошла успешно, передаем управление следующему шагу (или Handler)
            return await next();
        }
    }
}
