using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Models;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetReclamosListado;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Queries
{
    public class ReclamosQueries : IReclamosQueries
    {
        private readonly IDbContext _dbContext;
        private readonly string _connectionString;
        public ReclamosQueries(IDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("La cadena de conexión 'DefaultConnection' no está configurada");
        }

        public async Task<ReclamoDetalleCompletoDto> GetReclamoDetalleCompletoAsync(string ticketId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"
    SELECT 
        r.ticket_id AS TicketId,
        r.cliente AS Cliente,
        r.nombres AS Nombres,
        r.apellidos AS Apellidos,
        r.telefono AS Telefono,
        r.correo AS Correo,
        r.detalle AS Detalle,
        r.estado AS Estado,
        r.prioridad AS Prioridad,
        r.motivo_id AS MotivoId,
        r.motivo_rechazo AS MotivoRechazo,
        m.nombre AS MotivoNombre,
        m.descripcion AS MotivoDescripcion,
        r.usuario_id AS UsuarioId,
        u.nombre AS UsuarioNombre,
        u.apellidos AS UsuarioApellidos,
        u.email AS UsuarioEmail,
        u.rol_id AS UsuarioRolId,
        rol.nombre AS UsuarioRolNombre,
        r.departamento_id AS DepartamentoId,
        d.nombre AS DepartamentoNombre,
        r.provincia_id AS ProvinciaId,
        p.nombre AS ProvinciaNombre,
        r.distrito_id AS DistritoId,
        di.nombre AS DistritoNombre,
        r.fecha_creacion AS FechaCreacion,
        r.fecha_cierre AS FechaCierre,
        r.ultima_modificacion AS UltimaModificacion,
        CASE
            WHEN r.fecha_cierre IS NULL THEN EXTRACT(DAY FROM NOW() - r.fecha_creacion)::int
            ELSE EXTRACT(DAY FROM r.fecha_cierre - r.fecha_creacion)::int
        END AS DiasAbierto
    FROM reclamos r
    JOIN motivos m ON r.motivo_id = m.motivo_id
    JOIN usuarios u ON r.usuario_id = u.usuario_id
    JOIN roles rol ON u.rol_id = rol.rol_id
    JOIN departamentos d ON r.departamento_id = d.id
    JOIN provincias p ON r.provincia_id = p.id
    JOIN distritos di ON r.distrito_id = di.id
    WHERE r.ticket_id = @TicketId";

                    var reclamo = await connection.QueryFirstOrDefaultAsync<ReclamoDetalleCompletoDto>(
                        sql,
                        new { TicketId = ticketId });

                    if (reclamo == null)
                        return null;

                    sql = @"
    SELECT 
        a.ticket_id AS TicketId,
        a.usuario_id AS UsuarioId,
        u.nombre AS UsuarioNombre,
        u.apellidos AS UsuarioApellidos,
        u.email AS UsuarioEmail,
        u.rol_id AS UsuarioRolId,
        r.nombre AS UsuarioRolNombre
    FROM asignaciones a
    JOIN usuarios u ON a.usuario_id = u.usuario_id
    JOIN roles r ON u.rol_id = r.rol_id
    WHERE a.ticket_id = @TicketId";

                    var asignaciones = await connection.QueryAsync<AsignacionDetalleDto>(
                        sql,
                        new { TicketId = ticketId });

                    reclamo.Asignaciones = asignaciones.ToList();

                    sql = @"
    SELECT 
        a.archivo_id AS ArchivoId,
        a.nombre_original AS NombreOriginal,
        a.nombre_sistema AS NombreSistema,
        a.extension AS Extension,
        a.tipo_mime AS TipoMime,
        a.ruta_almacenamiento AS RutaAlmacenamiento,
        a.tamano_bytes AS TamanoByte,
        a.fecha_subida AS FechaSubida
    FROM reclamo_archivos ra
    JOIN archivos a ON ra.archivo_id = a.archivo_id
    WHERE ra.ticket_id = @TicketId
    ORDER BY a.fecha_subida DESC";

                    var archivos = await connection.QueryAsync<ArchivoDetalleDto>(
                        sql,
                        new { TicketId = ticketId });

                    reclamo.Archivos = archivos.ToList();

                    sql = @"
                SELECT 
                    i.interaccion_id AS InteraccionId,
                    i.ticket_id AS ReclamoTicket,
                    i.usuario_id AS UsuarioId,
                    u.nombre AS UsuarioNombre,
                    u.apellidos AS UsuarioApellidos,
                    r.nombre AS UsuarioRolNombre,
                    i.mensaje AS Mensaje,
                    i.fecha_registro AS FechaRegistro,
                    i.fecha_modificacion AS FechaModificacion
                FROM interacciones i
                JOIN usuarios u ON i.usuario_id = u.usuario_id::varchar
                JOIN roles r ON u.rol_id = r.rol_id
                WHERE i.ticket_id = @TicketId
                ORDER BY i.fecha_registro DESC";

                    var interacciones = await connection.QueryAsync<InteraccionDetalleDto>(
                        sql,
                        new { TicketId = ticketId });

                    reclamo.Interacciones = interacciones.ToList();

                    foreach (var interaccion in reclamo.Interacciones)
                    {
                        sql = @"
                SELECT 
                    a.archivo_id AS ArchivoId,
                    a.nombre_original AS NombreOriginal,
                    a.nombre_sistema AS NombreSistema,
                    a.extension AS Extension,
                    a.tipo_mime AS TipoMime,
                    a.ruta_almacenamiento AS RutaAlmacenamiento,
                    a.tamano_bytes AS TamanoByte,
                    a.fecha_subida AS FechaSubida
                FROM interaccion_archivos ia
                JOIN archivos a ON ia.archivo_id = a.archivo_id
                WHERE ia.interaccion_id = @InteraccionId
                ORDER BY a.fecha_subida DESC";

                        var interaccionArchivos = await connection.QueryAsync<ArchivoDetalleDto>(
                            sql,
                            new { InteraccionId = interaccion.InteraccionId });

                        interaccion.Archivos = interaccionArchivos.ToList();
                    }

                    sql = @"
SELECT 
    he.historial_id AS HistorialId,
    he.ticket_id AS TicketId,
    he.usuario_id AS UsuarioId,
    u.nombre AS UsuarioNombre,
    u.apellidos AS UsuarioApellidos,
    he.estado_anterior AS EstadoAnterior,
    he.estado_nuevo AS EstadoNuevo,
    he.comentario AS Comentario,
    he.fecha_registro AS FechaRegistro
FROM historial_estados he
JOIN usuarios u ON he.usuario_id = u.usuario_id
WHERE he.ticket_id = @TicketId
ORDER BY he.fecha_registro ASC";

                    var historialEstados = await connection.QueryAsync<EstadoHistorialDto>(
                        sql,
                        new { TicketId = ticketId });

                    reclamo.HistorialEstados = historialEstados.ToList();

                    return reclamo;
                }
            }
            catch (Exception ex)
            {
                // Considera agregar un log del error
                throw;
            }
        }

        public async Task<PaginatedList<ReclamoListadoDto>> GetReclamosPaginadosAsync(
    int pageNumber,
    int pageSize,
    string ticketId = "",
    DateTime? fechaDesde = null,
    DateTime? fechaHasta = null,
    string estado = "",
    string prioridad = "",
    Usuario usuarioActual = null)  // Agregar el parámetro de usuario actual
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Skip", (pageNumber - 1) * pageSize);
            parameters.Add("@Take", pageSize);

            var whereClause = new StringBuilder();
            var joinClause = new StringBuilder(" JOIN motivos m ON r.motivo_id = m.motivo_id ");
            var needsGroupBy = false;

            if (!string.IsNullOrEmpty(ticketId))
            {
                whereClause.Append(" AND r.ticket_id = @TicketId ");
                parameters.Add("@TicketId", ticketId); // Coincidencia exacta
            }

            if (fechaDesde.HasValue)
            {
                whereClause.Append(" AND r.fecha_creacion >= @FechaDesde ");
                parameters.Add("@FechaDesde", fechaDesde.Value.Date);
            }

            if (fechaHasta.HasValue)
            {
                whereClause.Append(" AND r.fecha_creacion <= @FechaHasta ");
                parameters.Add("@FechaHasta", fechaHasta.Value.Date.AddDays(1).AddSeconds(-1));
            }

            if (!string.IsNullOrEmpty(estado))
            {
                whereClause.Append(" AND r.estado = @Estado ");
                parameters.Add("@Estado", estado);
            }

            if (!string.IsNullOrEmpty(prioridad))
            {
                whereClause.Append(" AND r.prioridad = @Prioridad ");
                parameters.Add("@Prioridad", prioridad);
            }

            // Filtrar por asignación del Jefe de Área si corresponde
            if (usuarioActual != null && usuarioActual.NombreRol == "JefeArea")
            {
                joinClause.Append(" JOIN asignaciones a ON r.ticket_id = a.ticket_id ");
                whereClause.Append(" AND a.usuario_id = @UsuarioId ");
                parameters.Add("@UsuarioId", usuarioActual.UsuarioId);
                needsGroupBy = true;
            }

            var groupByClause = needsGroupBy
                ? " GROUP BY r.ticket_id, r.cliente, r.nombres, r.apellidos, r.fecha_creacion, r.estado, r.prioridad, m.nombre, r.fecha_cierre, r.ultima_modificacion "
                : "";

            // Consulta para obtener los reclamos - ordenada por fecha de creación descendiente por defecto
            var query = $@"
SELECT 
    r.ticket_id AS TicketId,
    r.cliente AS Cliente,
    r.nombres AS Nombres,
    r.apellidos AS Apellidos,
    CONCAT(r.nombres, ' ', r.apellidos) AS NombreCompleto,
    r.fecha_creacion AS FechaCreacion,
    r.estado AS Estado,
    r.prioridad AS Prioridad,
    m.nombre AS MotivoNombre,
    CASE 
        WHEN r.fecha_cierre IS NULL THEN EXTRACT(DAY FROM NOW() - r.fecha_creacion)::int
        ELSE EXTRACT(DAY FROM r.fecha_cierre - r.fecha_creacion)::int
    END AS DiasAbierto,
    r.ultima_modificacion AS UltimaModificacion
FROM reclamos r
{joinClause}
WHERE 1=1 {whereClause}
{groupByClause}
ORDER BY r.fecha_creacion DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY";

            // Consulta para contar el total de registros
            var countQuery = $@"
SELECT COUNT(DISTINCT r.ticket_id)
FROM reclamos r
{joinClause}
WHERE 1=1 {whereClause}";

            var connection = _dbContext.Connection;

            // Obtener los reclamos con tipo fuerte
            var reclamos = await connection.QueryAsync<ReclamoListadoDto>(
                query,
                parameters,
                transaction: _dbContext.Transaction);

            var totalCount = await connection.ExecuteScalarAsync<int>(
                countQuery,
                parameters,
                transaction: _dbContext.Transaction);

            var reclamosList = reclamos.ToList();

            // Obtener las asignaciones para cada reclamo
            if (reclamosList.Any())
            {
                var ticketIds = reclamosList.Select(r => r.TicketId).ToList();

                // Consulta para obtener asignaciones con tipo fuerte
                var asignacionesQuery = @"
SELECT
    a.ticket_id AS TicketId,
    a.usuario_id AS UsuarioId,
    u.nombre AS Nombre,
    u.apellidos AS Apellidos,
    r.nombre AS RolNombre,
    CONCAT(u.nombre, ' ', u.apellidos) AS NombreCompleto
FROM asignaciones a
JOIN usuarios u ON a.usuario_id = u.usuario_id
JOIN roles r ON u.rol_id = r.rol_id
WHERE a.ticket_id = ANY(@TicketIds)";

                // Ejecutar la consulta con mapeo a tipo fuerte
                var asignaciones = await connection.QueryAsync<AsignacionDto>(
                    asignacionesQuery,
                    new { TicketIds = ticketIds.ToArray() },
                    transaction: _dbContext.Transaction);

                // Agrupar por ticket con tipos fuertes
                var asignacionesPorTicket = asignaciones
                    .GroupBy(a => a.TicketId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(a => new AsignacionResumenDto
                        {
                            UsuarioId = a.UsuarioId,
                            Nombre = a.Nombre,
                            Apellidos = a.Apellidos,
                            RolNombre = a.RolNombre
                        }).ToList()
                    );

                // Asignar las asignaciones a cada reclamo
                foreach (var reclamo in reclamosList)
                {
                    if (asignacionesPorTicket.ContainsKey(reclamo.TicketId))
                    {
                        reclamo.Asignaciones = asignacionesPorTicket[reclamo.TicketId];
                    }
                    else
                    {
                        // Inicializar con lista vacía si no hay asignaciones
                        reclamo.Asignaciones = new List<AsignacionResumenDto>();
                    }
                }
            }

            return new PaginatedList<ReclamoListadoDto>(
                reclamosList,
                totalCount,
                pageNumber,
                pageSize);
        }
    }
}
