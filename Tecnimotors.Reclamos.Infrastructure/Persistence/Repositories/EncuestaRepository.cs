using Dapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class EncuestaRepository : IEncuestaRepository
    {
        private readonly IDbContext _context;

        public EncuestaRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracionEncuesta> GetConfiguracionActivaAsync(string tipoEncuesta)
        {
            const string sql = @"
                SELECT 
                    id AS Id,
                    nombre AS Nombre,
                    descripcion AS Descripcion,
                    tipo_encuesta AS TipoEncuesta,
                    dias_espera_envio AS DiasEsperaEnvio,
                    dias_vigencia AS DiasVigencia,
                    activa AS Activa,
                    fecha_registro AS FechaRegistro
                FROM configuracion_encuesta
                WHERE tipo_encuesta = @TipoEncuesta 
                    AND activa = true
                LIMIT 1";

            return await _context.Connection.QueryFirstOrDefaultAsync<ConfiguracionEncuesta>(
                sql,
                new { TipoEncuesta = tipoEncuesta },
                transaction: _context.Transaction);
        }

        public async Task<int> CreateConfiguracionAsync(ConfiguracionEncuesta configuracion)
        {
            const string sql = @"
                INSERT INTO configuracion_encuesta(
                    nombre,
                    descripcion,
                    tipo_encuesta,
                    dias_espera_envio,
                    dias_vigencia,
                    activa,
                    fecha_registro
                )
                VALUES(
                    @Nombre,
                    @Descripcion,
                    @TipoEncuesta,
                    @DiasEsperaEnvio,
                    @DiasVigencia,
                    @Activa,
                    @FechaRegistro
                )
                RETURNING id";

            return await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                configuracion,
                transaction: _context.Transaction);
        }

        public async Task<EncuestasEnviadas> GetEncuestaByTokenAsync(Guid token)
        {
            const string sql = @"
                SELECT 
                    e.id AS Id,
                    e.ticket_id AS TicketId,
                    e.configuracion_encuesta_id AS ConfiguracionEncuestaId,
                    e.token_acceso AS TokenAcceso,
                    e.fecha_envio AS FechaEnvio,
                    e.fecha_vencimiento AS FechaVencimiento,
                    e.fecha_respuesta AS FechaRespuesta,
                    e.estado_encuesta AS EstadoEncuesta,
                    e.email_enviado AS EmailEnviado,
                    e.ip_respuesta AS IPRespuesta,
                    e.dispositivo_respuesta AS DispositivoRespuesta
                FROM encuestas_enviadas e
                WHERE e.token_acceso = @Token";

            return await _context.Connection.QueryFirstOrDefaultAsync<EncuestasEnviadas>(
                sql,
                new { Token = token });
        }

        public async Task<int> CreateEncuestaEnviadaAsync(EncuestasEnviadas encuesta)
        {
            const string sql = @"
                INSERT INTO encuestas_enviadas(
                    ticket_id,
                    configuracion_encuesta_id,
                    token_acceso,
                    fecha_envio,
                    fecha_vencimiento,
                    estado_encuesta,
                    email_enviado
                )
                VALUES(
                    @TicketId,
                    @ConfiguracionEncuestaId,
                    @TokenAcceso,
                    @FechaEnvio,
                    @FechaVencimiento,
                    @EstadoEncuesta,
                    @EmailEnviado
                )
                RETURNING id";

            return await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                encuesta,
                transaction: _context.Transaction);
        }

        public async Task<bool> UpdateEstadoEncuestaAsync(int id, string estado, DateTime? fechaRespuesta)
        {
            const string sql = @"
                UPDATE encuestas_enviadas
                SET 
                    estado_encuesta = @Estado,
                    fecha_respuesta = @FechaRespuesta
                WHERE id = @Id";

            int affectedRows = await _context.Connection.ExecuteAsync(
                sql,
                new { Id = id, Estado = estado, FechaRespuesta = fechaRespuesta },
                transaction: _context.Transaction);

            return affectedRows > 0;
        }

        public async Task<bool> ExisteEncuestaPorTicketAsync(string ticketId)
        {
            const string sql = @"
                SELECT COUNT(1) 
                FROM encuestas_enviadas 
                WHERE ticket_id = @TicketId";

            var count = await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                new { TicketId = ticketId },
                transaction: _context.Transaction);

            return count > 0;
        }

        public async Task<IEnumerable<PreguntasEncuesta>> GetPreguntasByConfiguracionAsync(int configuracionId)
        {
            const string sql = @"
                SELECT 
                    id AS Id,
                    configuracion_encuesta_id AS ConfiguracionEncuestaId,
                    codigo AS Codigo,
                    texto_pregunta AS TextoPregunta,
                    categoria AS Categoria,
                    orden AS Orden,
                    obligatoria AS Obligatoria,
                    activa AS Activa
                FROM preguntas_encuesta
                WHERE configuracion_encuesta_id = @ConfiguracionId
                    AND activa = true
                ORDER BY orden";

            return await _context.Connection.QueryAsync<PreguntasEncuesta>(
                sql,
                new { ConfiguracionId = configuracionId },
                transaction: _context.Transaction);
        }

        public async Task<int> CreatePreguntaAsync(PreguntasEncuesta pregunta)
        {
            const string sql = @"
                INSERT INTO preguntas_encuesta(
                    configuracion_encuesta_id,
                    codigo,
                    texto_pregunta,
                    categoria,
                    orden,
                    obligatoria,
                    activa
                )
                VALUES(
                    @ConfiguracionEncuestaId,
                    @Codigo,
                    @TextoPregunta,
                    @Categoria,
                    @Orden,
                    @Obligatoria,
                    @Activa
                )
                RETURNING id";

            return await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                pregunta,
                transaction: _context.Transaction);
        }

        public async Task<int> CreateRespuestaAsync(RespuestasEncuesta respuesta)
        {
            const string sql = @"
                INSERT INTO respuestas_encuesta(
                    encuesta_enviada_id,
                    pregunta_id,
                    valor_likert,
                    fecha_respuesta
                )
                VALUES(
                    @EncuestaEnviadaId,
                    @PreguntaId,
                    @ValorLikert,
                    @FechaRespuesta
                )
                RETURNING id";

            return await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                respuesta,
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<RespuestasEncuesta>> GetRespuestasByEncuestaAsync(int encuestaId)
        {
            const string sql = @"
                SELECT 
                    id AS Id,
                    encuesta_enviada_id AS EncuestaEnviadaId,
                    pregunta_id AS PreguntaId,
                    valor_likert AS ValorLikert,
                    fecha_respuesta AS FechaRespuesta
                FROM respuestas_encuesta
                WHERE encuesta_enviada_id = @EncuestaId";

            return await _context.Connection.QueryAsync<RespuestasEncuesta>(
                sql,
                new { EncuestaId = encuestaId },
                transaction: _context.Transaction);
        }

        public async Task<int> CreateComentarioAsync(ComentariosEncuesta comentario)
        {
            const string sql = @"
                INSERT INTO comentarios_encuesta(
                    encuesta_enviada_id,
                    tipo_comentario,
                    comentario,
                    fecha_comentario
                )
                VALUES(
                    @EncuestaEnviadaId,
                    @TipoComentario,
                    @Comentario,
                    @FechaComentario
                )
                RETURNING id";

            return await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                comentario,
                transaction: _context.Transaction);
        }

        public async Task<decimal> CalcularISGAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            const string sql = @"
                SELECT 
                    COALESCE(AVG(re.valor_likert), 0) as ISG
                FROM encuestas_enviadas ee
                INNER JOIN respuestas_encuesta re ON ee.id = re.encuesta_enviada_id
                INNER JOIN preguntas_encuesta pe ON re.pregunta_id = pe.id
                WHERE ee.estado_encuesta = 'RESPONDIDA'
                    AND ee.fecha_respuesta >= @FechaInicio
                    AND ee.fecha_respuesta <= @FechaFin
                    AND pe.codigo = 'P10'";

            return await _context.Connection.ExecuteScalarAsync<decimal>(
                sql,
                new { FechaInicio = fechaInicio, FechaFin = fechaFin },
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<EstadisticaISGDto>> GetEstadisticasISGAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            const string sql = @"
                WITH estadisticas AS (
                    SELECT 
                        TO_CHAR(ee.fecha_respuesta, 'YYYY-MM') as periodo,
                        COUNT(DISTINCT ee.id) as total_encuestas,
                        AVG(CASE WHEN pe.codigo = 'P10' THEN re.valor_likert END) as isg,
                        COUNT(CASE WHEN pe.codigo = 'P10' AND re.valor_likert = 5 THEN 1 END) as muy_satisfechos,
                        COUNT(CASE WHEN pe.codigo = 'P10' AND re.valor_likert = 4 THEN 1 END) as satisfechos,
                        COUNT(CASE WHEN pe.codigo = 'P10' AND re.valor_likert = 3 THEN 1 END) as neutrales,
                        COUNT(CASE WHEN pe.codigo = 'P10' AND re.valor_likert <= 2 THEN 1 END) as insatisfechos
                    FROM encuestas_enviadas ee
                    INNER JOIN respuestas_encuesta re ON ee.id = re.encuesta_enviada_id
                    INNER JOIN preguntas_encuesta pe ON re.pregunta_id = pe.id
                    WHERE ee.estado_encuesta = 'RESPONDIDA'
                        AND ee.fecha_respuesta >= @FechaInicio
                        AND ee.fecha_respuesta <= @FechaFin
                    GROUP BY TO_CHAR(ee.fecha_respuesta, 'YYYY-MM')
                )
                SELECT 
                    periodo AS Periodo,
                    total_encuestas AS TotalEncuestas,
                    COALESCE(isg, 0) AS ISG,
                    muy_satisfechos AS MuySatisfechos,
                    satisfechos AS Satisfechos,
                    neutrales AS Neutrales,
                    insatisfechos AS Insatisfechos
                FROM estadisticas
                ORDER BY periodo";

            return await _context.Connection.QueryAsync<EstadisticaISGDto>(
                sql,
                new { FechaInicio = fechaInicio, FechaFin = fechaFin },
                transaction: _context.Transaction);
        }
    }

    // DTOs para queries complejas
    public class EstadisticaISGDto
    {
        public string Periodo { get; set; }
        public int TotalEncuestas { get; set; }
        public decimal ISG { get; set; }
        public int MuySatisfechos { get; set; }
        public int Satisfechos { get; set; }
        public int Neutrales { get; set; }
        public int Insatisfechos { get; set; }
    }
}
