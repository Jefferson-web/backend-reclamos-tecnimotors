using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.LocalidadAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.PlantillaAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Domain.Services;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Queries;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories;
using Tecnimotors.Reclamos.Infrastructure.Services;

namespace Tecnimotors.Reclamos.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IDbContext, PostgreSqlDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHttpContextAccessor();

            services.AddScoped<IMotivoRepository, MotivoRepository>();
            services.AddScoped<IReclamoRepository, ReclamoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUbicacionRepository, UbicacionRepository>();
            services.AddScoped<IArchivoRepository, ArchivoRepository>();
            services.AddScoped<ILocalidadRepository, LocalidadRepository>();
            services.AddScoped<IHistorialEstadoRepository, HistorialEstadoRepository>();
            services.AddScoped<IInteraccionRepository, InteraccionRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IEncuestaRepository, EncuestaRepository>();

            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IReclamosQueries, ReclamosQueries>();
            services.AddScoped<IDashboardQueries, DashboardQueries>();
            services.AddScoped<IEmailTemplateProcessor, EmailTemplateProcessor>();

            return services;
        }
    }
}
