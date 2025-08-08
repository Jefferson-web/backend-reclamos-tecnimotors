using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Reflection;
using Tecnimotors.Reclamos.Application.Common.Behaviors;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Application.Jobs;
using Tecnimotors.Reclamos.Application.Services;
using Tecnimotors.Reclamos.Domain.Events;
using Tecnimotors.Reclamos.Domain.Services;

namespace Tecnimotors.Reclamos.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.RegisterServicesFromAssembly(typeof(ReclamoRegistradoEvent).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(NuevaInteraccionEvent).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(ReclamoRechazadoEvent).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(ReclamoCerradoEvent).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(ReclamoAtendidoEvent).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(EnviarEncuestaEvent).Assembly);
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUsuarioService, UsuarioService>();

            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("CerrarReclamosJob");
                q.AddJob<CerrarReclamosJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("CerrarReclamosJob-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInHours(1)
                        .RepeatForever())
                );
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }
    }
}
