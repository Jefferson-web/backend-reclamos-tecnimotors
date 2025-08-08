using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.PlantillaAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly IDbContext _dbContext;
        public EmailTemplateRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PlantillaCorreo> GetPlantillaByCodigo(string codigo)
        {
            const string sql = @"
                select 
	                id as Id,
	                codigo as Codigo,
	                nombre as Nombre,
	                asunto as Asunto,
	                contenido_html as ContenidoHtml,
	                activo as Activo,
	                fecha_registro as FechaRegistro,
	                fecha_actualizacion as FechaActualizacion,
	                descripcion as Descripcion
                from plantillas_correo
                where codigo=@Codigo
            ";

            var template = await _dbContext.Connection.QuerySingleOrDefaultAsync<PlantillaCorreo>(sql, new { Codigo = codigo });
            return template;
        }
    }
}
