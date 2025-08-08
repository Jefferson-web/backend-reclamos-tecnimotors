using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Features.Dashboard.Dtos;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Queries
{
    public class DashboardQueries : IDashboardQueries
    {
        private readonly IDbContext _context;

        public DashboardQueries(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<EstadisticaCardDto>> GetEstadisticasGeneralesAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            var result = new List<EstadisticaCardDto>();

            // Obtener fecha del mes anterior para comparación
            var fechaActual = DateTime.Now;
            var inicioPeriodoActual = new DateTime(fechaActual.Year, fechaActual.Month, 1);
            var inicioPeriodoAnterior = inicioPeriodoActual.AddMonths(-1);
            var finPeriodoAnterior = inicioPeriodoActual.AddDays(-1);

            // Parámetros para filtrar por fecha
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde ?? inicioPeriodoAnterior);
            parameters.Add("@FechaHasta", fechaHasta ?? fechaActual);
            parameters.Add("@InicioPeriodoActual", inicioPeriodoActual);
            parameters.Add("@InicioPeriodoAnterior", inicioPeriodoAnterior);
            parameters.Add("@FinPeriodoAnterior", finPeriodoAnterior);

            // 1. Total de Reclamos
            const string sqlTotalReclamos = @"
                WITH total_actual AS (
                    SELECT COUNT(*) AS total
                    FROM reclamos
                    WHERE fecha_creacion >= @InicioPeriodoActual
                ),
                total_anterior AS (
                    SELECT COUNT(*) AS total
                    FROM reclamos
                    WHERE fecha_creacion >= @InicioPeriodoAnterior
                    AND fecha_creacion < @InicioPeriodoActual
                )
                SELECT 
                    (SELECT total FROM total_actual) AS total_actual,
                    (SELECT total FROM total_anterior) AS total_anterior";

            var totalReclamos = await _context.Connection.QueryFirstOrDefaultAsync<(int totalActual, int totalAnterior)>(
                sqlTotalReclamos,
                parameters,
                transaction: _context.Transaction);

            decimal cambioPorcentualTotal = 0;
            if (totalReclamos.totalAnterior > 0)
            {
                cambioPorcentualTotal = Math.Round((decimal)(totalReclamos.totalActual - totalReclamos.totalAnterior) * 100 / totalReclamos.totalAnterior, 1);
            }

            result.Add(new EstadisticaCardDto
            {
                Icono = "bi-clipboard-data",
                Valor = totalReclamos.totalActual,
                Titulo = "Total de Reclamos",
                CambioPorcentual = cambioPorcentualTotal,
                PeriodoTiempo = "el mes pasado",
                Sufijo = "",
                EsDecimal = false
            });

            // 2. Reclamos Resueltos
            const string sqlReclamosResueltos = @"
                WITH resueltos_actual AS (
                    SELECT COUNT(*) AS total
                    FROM reclamos
                    WHERE (estado = 'Cerrado' OR estado = 'Rechazado')
                    AND fecha_cierre >= @InicioPeriodoActual
                ),
                resueltos_anterior AS (
                    SELECT COUNT(*) AS total
                    FROM reclamos
                    WHERE (estado = 'Cerrado' OR estado = 'Rechazado')
                    AND fecha_cierre >= @InicioPeriodoAnterior
                    AND fecha_cierre < @InicioPeriodoActual
                )
                SELECT 
                    (SELECT total FROM resueltos_actual) AS total_actual,
                    (SELECT total FROM resueltos_anterior) AS total_anterior";

            var reclamosResueltos = await _context.Connection.QueryFirstOrDefaultAsync<(int totalActual, int totalAnterior)>(
                sqlReclamosResueltos,
                parameters,
                transaction: _context.Transaction);

            decimal cambioPorcentualResueltos = 0;
            if (reclamosResueltos.totalAnterior > 0)
            {
                cambioPorcentualResueltos = Math.Round((decimal)(reclamosResueltos.totalActual - reclamosResueltos.totalAnterior) * 100 / reclamosResueltos.totalAnterior, 1);
            }

            result.Add(new EstadisticaCardDto
            {
                Icono = "bi-check-circle",
                Valor = reclamosResueltos.totalActual,
                Titulo = "Resueltos",
                CambioPorcentual = cambioPorcentualResueltos,
                PeriodoTiempo = "el mes pasado",
                Sufijo = "",
                EsDecimal = false
            });

            // 3. Tiempo de Respuesta Promedio
            const string sqlTiempoRespuesta = @"
                WITH tiempo_actual AS (
                    SELECT AVG(EXTRACT(EPOCH FROM (fecha_cierre - fecha_creacion))/3600) AS promedio
                    FROM reclamos
                    WHERE (estado = 'Cerrado' OR estado = 'Rechazado')
                    AND fecha_cierre >= @InicioPeriodoActual
                ),
                tiempo_anterior AS (
                    SELECT AVG(EXTRACT(EPOCH FROM (fecha_cierre - fecha_creacion))/3600) AS promedio
                    FROM reclamos
                    WHERE (estado = 'Cerrado' OR estado = 'Rechazado')
                    AND fecha_cierre >= @InicioPeriodoAnterior
                    AND fecha_cierre < @InicioPeriodoActual
                )
                SELECT 
                    (SELECT promedio FROM tiempo_actual) AS promedio_actual,
                    (SELECT promedio FROM tiempo_anterior) AS promedio_anterior";

            var tiempoRespuesta = await _context.Connection.QueryFirstOrDefaultAsync<(decimal? promedioActual, decimal? promedioAnterior)>(
                sqlTiempoRespuesta,
                parameters,
                transaction: _context.Transaction);

            decimal tiempoRespuestaActual = tiempoRespuesta.promedioActual.HasValue ? Math.Round(tiempoRespuesta.promedioActual.Value, 1) : 0;
            decimal cambioPorcentualTiempo = 0;
            if (tiempoRespuesta.promedioAnterior.HasValue && tiempoRespuesta.promedioAnterior.Value > 0)
            {
                cambioPorcentualTiempo = Math.Round((decimal)(tiempoRespuestaActual - tiempoRespuesta.promedioAnterior.Value) * 100 / tiempoRespuesta.promedioAnterior.Value, 1);
            }

            result.Add(new EstadisticaCardDto
            {
                Icono = "bi-alarm",
                Valor = tiempoRespuestaActual,
                Titulo = "Tiempo de Respuesta Promedio",
                CambioPorcentual = cambioPorcentualTiempo,
                PeriodoTiempo = "el mes pasado",
                Sufijo = "h",
                EsDecimal = true
            });

            // 4. Satisfacción del Cliente - CON DATOS REALES DE ENCUESTAS
            const string sqlSatisfaccion = @"
    WITH isg_actual AS (
        SELECT AVG(re.valor_likert::numeric) AS promedio
        FROM encuestas_enviadas ee
        INNER JOIN respuestas_encuesta re ON ee.id = re.encuesta_enviada_id
        INNER JOIN preguntas_encuesta pe ON re.pregunta_id = pe.id
        WHERE ee.estado_encuesta = 'RESPONDIDA'
            AND pe.codigo = 'P10'
            AND ee.fecha_respuesta >= @InicioPeriodoActual
    ),
    isg_anterior AS (
        SELECT AVG(re.valor_likert::numeric) AS promedio
        FROM encuestas_enviadas ee
        INNER JOIN respuestas_encuesta re ON ee.id = re.encuesta_enviada_id
        INNER JOIN preguntas_encuesta pe ON re.pregunta_id = pe.id
        WHERE ee.estado_encuesta = 'RESPONDIDA'
            AND pe.codigo = 'P10'
            AND ee.fecha_respuesta >= @InicioPeriodoAnterior
            AND ee.fecha_respuesta < @InicioPeriodoActual
    )
    SELECT 
        COALESCE((SELECT promedio FROM isg_actual), 0) AS isg_actual,
        COALESCE((SELECT promedio FROM isg_anterior), 0) AS isg_anterior";

            var satisfaccion = await _context.Connection.QueryFirstOrDefaultAsync<(decimal isgActual, decimal isgAnterior)>(
                sqlSatisfaccion,
                parameters,
                transaction: _context.Transaction);

            decimal satisfaccionActual = Math.Round(satisfaccion.isgActual, 1);
            decimal cambioPorcentualSatisfaccion = 0;

            if (satisfaccion.isgAnterior > 0)
            {
                cambioPorcentualSatisfaccion = Math.Round((satisfaccionActual - satisfaccion.isgAnterior) * 100 / satisfaccion.isgAnterior, 1);
            }

            result.Add(new EstadisticaCardDto
            {
                Icono = "bi-emoji-smile",
                Valor = satisfaccionActual,
                Titulo = "Satisfacción del Cliente",
                CambioPorcentual = cambioPorcentualSatisfaccion,
                PeriodoTiempo = "el mes pasado",
                Sufijo = "/5",
                EsDecimal = true
            });

            return result;
        }

        public async Task<DistribucionEstadosDto> GetDistribucionEstadosAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            string sql = @"
        SELECT 
            estado AS Estado, 
            COUNT(*) AS Cantidad
        FROM reclamos
        WHERE 1=1
            AND (@FechaDesde::timestamp IS NULL OR fecha_creacion >= @FechaDesde)
            AND (@FechaHasta::timestamp IS NULL OR fecha_creacion <= @FechaHasta)
        GROUP BY estado
        ORDER BY COUNT(*) DESC";

            // Crear un DynamicParameters para especificar tipos explícitamente
            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@FechaHasta", fechaHasta, DbType.DateTime, ParameterDirection.Input);

            var distribucion = await _context.Connection.QueryAsync<EstadoReclamoDto>(
                sql,
                parameters,
                transaction: _context.Transaction);

            return new DistribucionEstadosDto
            {
                Distribucion = distribucion
            };
        }

        public async Task<AnalisisMotivosParetoDto> GetAnalisisMotivosParetoAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            string sql = @"
                SELECT 
                    m.nombre AS Motivo,
                    COUNT(*) AS Cantidad
                FROM reclamos r
                JOIN motivos m ON r.motivo_id = m.motivo_id
                WHERE 1=1
                    AND (@FechaDesde::timestamp IS NULL OR r.fecha_creacion >= @FechaDesde)
                    AND (@FechaHasta::timestamp IS NULL OR r.fecha_creacion <= @FechaHasta)
                GROUP BY m.nombre
                ORDER BY COUNT(*) DESC";

            var parameters = new DynamicParameters();
            parameters.Add("@FechaDesde", fechaDesde, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@FechaHasta", fechaHasta, DbType.DateTime, ParameterDirection.Input);

            var motivos = await _context.Connection.QueryAsync<MotivoReclamoDto>(
                sql,
                parameters,
                transaction: _context.Transaction);

            // Calcular porcentajes acumulados
            int total = motivos.Sum(m => m.Cantidad);
            decimal acumulado = 0;

            var motivosConPorcentaje = motivos.Select(m =>
            {
                acumulado += m.Cantidad;
                return new MotivoReclamoDto
                {
                    Motivo = m.Motivo,
                    Cantidad = m.Cantidad,
                    PorcentajeAcumulado = Math.Round((decimal)acumulado * 100 / total, 1)
                };
            }).ToList();

            return new AnalisisMotivosParetoDto
            {
                Motivos = motivosConPorcentaje
            };
        }

        public async Task<TendenciaReclamosDto> GetTendenciaReclamosAsync(int anio)
        {
            // Obtener datos para año actual y anterior
            int anioAnterior = anio - 1;

            string sql = @"
                WITH meses AS (
                    SELECT generate_series(1, 12) AS mes
                ),
                reclamos_por_mes_actual AS (
                    SELECT 
                        EXTRACT(MONTH FROM fecha_creacion) AS mes,
                        COUNT(*) AS cantidad
                    FROM reclamos
                    WHERE EXTRACT(YEAR FROM fecha_creacion) = @AnioActual
                    GROUP BY EXTRACT(MONTH FROM fecha_creacion)
                ),
                reclamos_por_mes_anterior AS (
                    SELECT 
                        EXTRACT(MONTH FROM fecha_creacion) AS mes,
                        COUNT(*) AS cantidad
                    FROM reclamos
                    WHERE EXTRACT(YEAR FROM fecha_creacion) = @AnioAnterior
                    GROUP BY EXTRACT(MONTH FROM fecha_creacion)
                )
                SELECT 
                    m.mes,
                    COALESCE(r_actual.cantidad, 0) AS cantidad_actual,
                    COALESCE(r_anterior.cantidad, 0) AS cantidad_anterior
                FROM meses m
                LEFT JOIN reclamos_por_mes_actual r_actual ON m.mes = r_actual.mes
                LEFT JOIN reclamos_por_mes_anterior r_anterior ON m.mes = r_anterior.mes
                ORDER BY m.mes";

            var resultados = await _context.Connection.QueryAsync<(int mes, int cantidadActual, int cantidadAnterior)>(
                sql,
                new { AnioActual = anio, AnioAnterior = anioAnterior },
                transaction: _context.Transaction);

            // Convertir número de mes a nombre
            string[] nombresMeses = { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            var meses = resultados.Select(r => nombresMeses[r.mes - 1]).ToList();

            // Organizar datos por año
            var datosPorAnio = new Dictionary<int, IEnumerable<int>>
            {
                { anio, resultados.Select(r => r.cantidadActual) },
                { anioAnterior, resultados.Select(r => r.cantidadAnterior) }
            };

            return new TendenciaReclamosDto
            {
                Meses = meses,
                DatosPorAnio = datosPorAnio
            };
        }

        public async Task<IEnumerable<EstadisticaISGDto>> GetEstadisticasISGAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
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
                        AND (@FechaInicio::timestamp IS NULL OR ee.fecha_respuesta >= @FechaInicio)
                        AND (@FechaFin::timestamp IS NULL OR ee.fecha_respuesta <= @FechaFin)
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

            var parameters = new DynamicParameters();
            parameters.Add("@FechaInicio", fechaInicio, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@FechaFin", fechaFin, DbType.DateTime, ParameterDirection.Input);

            return await _context.Connection.QueryAsync<EstadisticaISGDto>(
                sql,
                parameters,
                transaction: _context.Transaction);
        }
    }
}
