using Dapper;
using DocumentFormat.OpenXml.Office.Word.Y2020.OEmbed;
using Microsoft.AspNetCore.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class ReclamoRepository : IReclamoRepository
    {
        private readonly IDbContext _context;
        public ReclamoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Reclamo reclamo)
        {
            try
            {
                try
                {
                    const string sqlReclamo = @"
                        INSERT INTO reclamos (
                            ticket_id, usuario_id, cliente, nombres, apellidos, telefono, correo, detalle,
                            motivo_id, estado, departamento_id, provincia_id, distrito_id,
                            fecha_creacion, fecha_cierre, ultima_modificacion, prioridad
                        ) VALUES (
                            @TicketId, @UsuarioId, @Cliente, @Nombres, @Apellidos, @Telefono, @Correo, @Detalle,
                            @MotivoId, @Estado, @DepartamentoId, @ProvinciaId, @DistritoId,
                            @FechaCreacion, @FechaCierre, @UltimaModificacion, @Prioridad
                        )";

                    await _context.Connection.ExecuteAsync(
                        sqlReclamo,
                        new
                        {
                            reclamo.TicketId,
                            reclamo.UsuarioId,
                            reclamo.Cliente,
                            reclamo.Nombres,
                            reclamo.Apellidos,
                            reclamo.Telefono,
                            reclamo.Correo,
                            reclamo.Detalle,
                            reclamo.MotivoId,
                            reclamo.Estado,
                            reclamo.DepartamentoId,
                            reclamo.ProvinciaId,
                            reclamo.DistritoId,
                            reclamo.FechaCreacion,
                            reclamo.FechaCierre,
                            reclamo.UltimaModificacion,
                            reclamo.Prioridad
                        },
                        transaction: _context.Transaction);

                    if (reclamo.Asignaciones != null && reclamo.Asignaciones.Any())
                    {
                        const string sqlAsignacion = @"
                            INSERT INTO asignaciones (
                                ticket_id, usuario_id
                            ) VALUES (
                                @TicketId, @UsuarioId
                            )";

                        foreach (var asignacion in reclamo.Asignaciones)
                        {
                            await _context.Connection.ExecuteAsync(
                                sqlAsignacion,
                                new
                                {
                                    asignacion.TicketId,
                                    asignacion.UsuarioId,
                                },
                                transaction: _context.Transaction);
                        }
                    }

                    return 1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CerrarReclamosAutomaticamenteAsync(List<string> ticketIds)
        {
            if (ticketIds.Any())
            {
                return 0;
            }

            var fechaActual = DateTime.Now;

            try
            {
                const string sql = @"UPDATE reclamos
                    set estado= @Estado,
                        fecha_cierre = @FechaCierre
                        ultima_modificacion = @UltimaModificacion,
                        cierre_automatico = true,
                        motivo_rechazo = @MotivoRechazo
                    where ticket_id = ANY(@TicketIds)
                    AND estado NOT IN ('Cerrado')
                    AND fecha_cierre IS NULL
                    ";

                var parameters = new DynamicParameters();
                parameters.Add("@Estado", ReclamoEstado.Cerrado);
                parameters.Add("@FechaCierre", fechaActual);
                parameters.Add("@UltimaModificacion", fechaActual);
                parameters.Add("@MotivoRechazo", "Cerrado automáticamente por inactividad de 3 meses");
                parameters.Add("@TicketIds", ticketIds.ToArray());

                var filasAfectadas = await _context.Connection.ExecuteAsync(
                    sql,
                    parameters,
                    transaction: _context.Transaction);

                return filasAfectadas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GenerarTicketAsync()
        {
            try
            {
                const string sql = "SELECT generar_ticket()";

                var ticket = await _context.Connection.ExecuteScalarAsync<string>(
                    sql,
                    transaction: _context.Transaction);

                return ticket;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Reclamo> GetByTicketAsync(string ticket)
        {
            const string sql = @"
                SELECT 
                    ticket_id as TicketId,
                    usuario_id as UsuarioId,
                    cliente as Cliente,
                    nombres as Nombres,
                    apellidos as Apellidos,
                    telefono as Telefono,
                    correo as Correo,
                    detalle as Detalle,
                    motivo_id as MotivoId,
                    estado as Estado,
                    departamento_id as DepartamentoId,
                    provincia_id as ProvinciaId,
                    distrito_id as DistritoId,
                    fecha_creacion as FechaCreacion,
                    fecha_cierre as FechaCierre,
                    ultima_modificacion as UltimaModificacion,
                    prioridad as Prioridad
                FROM reclamos
                WHERE ticket_id=@ticket
            ";

            try
            {
                var reclamo = await _context.Connection.QuerySingleOrDefaultAsync<Reclamo>(sql, new { ticket });
                return reclamo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<Reclamo>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            throw new NotImplementedException();
        }

        public async Task<(IEnumerable<Reclamo> reclamos, int totalCount)> GetPaginatedAsync(int pageNumber, int pageSize, string ticket = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null, string estado = null, string prioridad = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Skip", (pageNumber - 1) * pageSize);
            parameters.Add("@Take", pageSize);

            var whereClause = new StringBuilder();

            if (!string.IsNullOrEmpty(ticket))
            {
                whereClause.Append(" AND r.ticket_id = @TicketId ");
                parameters.Add("@TicketId", $"%{ticket}%");
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

            var query = $@"
                SELECT r.ticket_id AS TicketId, 
                       r.usuario_id AS UsuarioId, 
                       r.detalle AS Detalle,
                       r.estado AS Estado, 
                       r.prioridad AS Prioridad,
                       r.fecha_creacion AS FechaCreacion, 
                       r.ultima_modificacion AS UltimaModificacion,
                       r.nombres AS Nombres,
                       r.apellidos AS Apellidos
                FROM reclamos r
                WHERE 1=1 {whereClause}
                ORDER BY r.fecha_creacion DESC
                OFFSET @Skip ROWS
                FETCH NEXT @Take ROWS ONLY";

            var countQuery = $@"
                SELECT COUNT(*)
                FROM reclamos r
                WHERE 1=1 {whereClause}";

            var connection = _context.Connection;

            var reclamos = await connection.QueryAsync<Reclamo>(
                query,
                parameters,
                transaction: _context.Transaction);

            var totalCount = await connection.ExecuteScalarAsync<int>(
                countQuery,
                parameters,
                transaction: _context.Transaction);

            return (reclamos, totalCount);
        }

        public async Task<IEnumerable<Reclamo>> GetReclamosParaExportarAsync(
            string ticketId = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string estado = null,
            string prioridad = null)
        {
            var parameters = new DynamicParameters();
            var whereClause = new StringBuilder();

            if (!string.IsNullOrEmpty(ticketId))
            {
                whereClause.Append(" AND r.ticket_id ILIKE @TicketId ");
                parameters.Add("@TicketId", $"%{ticketId}%");
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

            var query = $@"
        SELECT r.ticket_id AS TicketId, 
               r.cliente AS Cliente, 
               r.nombres AS Nombres, 
               r.apellidos AS Apellidos, 
               r.telefono AS Telefono, 
               r.correo AS Correo, 
               r.detalle AS Detalle, 
               r.estado AS Estado,
               r.prioridad AS Prioridad,
               r.fecha_creacion AS FechaCreacion, 
               r.fecha_cierre AS FechaCierre
                FROM reclamos r
                WHERE 1=1 {whereClause}
                ORDER BY r.fecha_creacion DESC";

            return await _context.Connection.QueryAsync<Reclamo>(
                query,
                parameters,
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosAsignadosAsync(string ticketId)
        {
            const string sql = @"
                select 
                    u.usuario_id as UsuarioId,
                    u.email as Email,
                    u.nombre as Nombre,
                    u.apellidos as Apellidos,
                    u.rol_id as RolId
                from usuarios u
                inner join asignaciones a
                on u.usuario_id=a.usuario_id
                where a.ticket_id=@TicketId";

            var parameters = new DynamicParameters();
            parameters.Add("TicketId", ticketId);

            var result = await _context.Connection.QueryAsync<Usuario>(sql, parameters);
            return result;
        }

        public async Task<IEnumerable<Reclamo>> ObtenerReclamosParaCierreAutomaticoAsync()
        {
            DateTime fechaLimite = DateTime.Now.AddMonths(-3);

            const string sql = @"
                select 
                    ticket_id as TicketId,
                    usuario_id as UsuarioId,
                    cliente as Cliente,
                    nombres as Nombres,
                    apellidos as Apellidos,
                    telefono as Telefono,
                    correo as Correo,
                    estado as Estado,
                    fecha_cierre as FechaCierre,
                    motivo_rechazo as MotivoRechazo,
                    cierre_automatico as CierreAutomatico
                from reclamos
                where estado not in ('Cerrado') 
                AND fecha_cierre IS NULL
                AND fecha_creacion <= @FechaLimite
                ORDER BY fecha_creacion
            ";

            try
            {
                var reclamos = await _context.Connection.QueryAsync<Reclamo>(sql, new { FechaLimite = fechaLimite });
                return reclamos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(Reclamo reclamo)
        {
            const string sql = @"
                UPDATE reclamos
                SET 
                    usuario_id = @UsuarioId,
                    cliente = @Cliente,
                    nombres = @Nombres,
                    apellidos = @Apellidos,
                    telefono = @Telefono,
                    correo = @Correo,
                    detalle = @Detalle,
                    motivo_id = @MotivoId,
                    estado = @Estado,
                    departamento_id = @DepartamentoId,
                    provincia_id = @ProvinciaId,
                    distrito_id = @DistritoId,
                    fecha_cierre = @FechaCierre,
                    prioridad = @Prioridad,
                    motivo_rechazo = @MotivoRechazo
                WHERE ticket_id = @TicketId";

            try
            {
                await _context.Connection.ExecuteAsync(
                    sql,
                    new
                    {
                        reclamo.TicketId,
                        reclamo.UsuarioId,
                        reclamo.Cliente,
                        reclamo.Nombres,
                        reclamo.Apellidos,
                        reclamo.Telefono,
                        reclamo.Correo,
                        reclamo.Detalle,
                        reclamo.MotivoId,
                        reclamo.Estado,
                        reclamo.DepartamentoId,
                        reclamo.ProvinciaId,
                        reclamo.DistritoId,
                        reclamo.FechaCierre,
                        reclamo.Prioridad,
                        reclamo.MotivoRechazo
                    },
                    transaction: _context.Transaction);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> VerificarUsuarioAsignadoAsync(string ticketId, int usuarioId)
        {
            const string sql = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 
                    FROM Asignaciones 
                    WHERE ticket_id = @TicketId AND usuario_id = @UsuarioId
                ) THEN 1 ELSE 0 END"
            ;

            var result =  await _context.Connection.ExecuteScalarAsync<bool>(sql, 
                new { TicketId = ticketId, UsuarioId = usuarioId });

            return result;
        }
    }
}
