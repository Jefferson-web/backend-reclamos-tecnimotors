using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class HistorialEstadoRepository : IHistorialEstadoRepository
    {
        private readonly IDbContext _context;

        public HistorialEstadoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<HistorialEstado> GetByIdAsync(int historialId)
        {
            const string sql = @"
                SELECT 
                    historial_id AS HistorialId,
                    ticket_id AS TicketId,
                    usuario_id AS UsuarioId,
                    estado_anterior AS EstadoAnterior,
                    estado_nuevo AS EstadoNuevo,
                    comentario AS Comentario,
                    fecha_registro AS FechaRegistro
                FROM historial_estados
                WHERE historial_id = @HistorialId";

            return await _context.Connection.QueryFirstOrDefaultAsync<HistorialEstado>(
                sql,
                new { HistorialId = historialId },
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<HistorialEstado>> GetByTicketIdAsync(string ticketId)
        {
            const string sql = @"
                SELECT 
                    historial_id AS HistorialId,
                    ticket_id AS TicketId,
                    usuario_id AS UsuarioId,
                    estado_anterior AS EstadoAnterior,
                    estado_nuevo AS EstadoNuevo,
                    comentario AS Comentario,
                    fecha_registro AS FechaRegistro
                FROM historial_estados
                WHERE ticket_id = @TicketId
                ORDER BY fecha_registro ASC";

            return await _context.Connection.QueryAsync<HistorialEstado>(
                sql,
                new { TicketId = ticketId },
                transaction: _context.Transaction);
        }

        public async Task<int> CreateAsync(HistorialEstado historialEstado)
        {
            const string sql = @"
                INSERT INTO historial_estados(
                    ticket_id,
                    usuario_id,
                    estado_anterior,
                    estado_nuevo,
                    comentario,
                    fecha_registro
                )
                VALUES(
                    @TicketId,
                    @UsuarioId,
                    @EstadoAnterior,
                    @EstadoNuevo,
                    @Comentario,
                    @FechaRegistro
                )
                RETURNING historial_id";

            return await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    historialEstado.TicketId,
                    historialEstado.UsuarioId,
                    historialEstado.EstadoAnterior,
                    historialEstado.EstadoNuevo,
                    historialEstado.Comentario,
                    historialEstado.FechaRegistro
                },
                transaction: _context.Transaction);
        }
    }
}
