using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Application.Behaviors;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cf =>
            {
                cf.RegisterServicesFromAssembly(assembly);

                cf.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cf.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
