using Dapper;
using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class InteraccionRepository : IInteraccionRepository
    {
        private readonly IDbContext _context;
        public InteraccionRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AsociarArchivoAsync(int interaccionId, int archivoId)
        {
            try
            {
                const string sql = @"
                    INSERT INTO interaccion_archivos(
                        interaccion_id,
                        archivo_id
                    )
                    VALUES(
                        @InteraccionId,
                        @ArchivoId
                    )
                    ON CONFLICT (interaccion_id, archivo_id) DO NOTHING";

                int affectedRows = await _context.Connection.ExecuteAsync(
                    sql,
                    new { InteraccionId = interaccionId, ArchivoId = archivoId },
                    transaction: _context.Transaction);

                return affectedRows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CreateAsync(Interaccion interaccion)
        {
            try
            {
                const string sql = @"
                    INSERT INTO interacciones(
                        ticket_id,
                        usuario_id,
                        mensaje,
                        fecha_registro,
                        fecha_modificacion
                    )
                    VALUES(
                        @TicketId,
                        @UsuarioId,
                        @Mensaje,
                        @FechaRegistro,
                        @FechaModificacion
                    )
                    RETURNING interaccion_id";

                var parametros = new
                {
                    interaccion.TicketId,
                    interaccion.UsuarioId,
                    interaccion.Mensaje,
                    interaccion.FechaRegistro,
                    interaccion.FechaModificacion
                };

                var interaccionId = await _context.Connection.ExecuteScalarAsync<int>(
                    sql,
                parametros,
                    transaction: _context.Transaction);

                return interaccionId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task DesasociarTodosLosArchivosAsync(int interaccionId)
        {
            try
            {
                const string sql = @"
                    DELETE FROM interaccion_archivos
                    WHERE interaccion_id = @InteraccionId";

                await _context.Connection.ExecuteAsync(
                    sql,
                    new { InteraccionId = interaccionId },
                    transaction: _context.Transaction);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int interaccionId)
        {
            try
            {
                await DesasociarTodosLosArchivosAsync(interaccionId);

                const string sql = @"
                    DELETE FROM interacciones
                    WHERE id = @InteraccionId";

                int affectedRows = await _context.Connection.ExecuteAsync(
                    sql,
                    new { InteraccionId = interaccionId },
                    transaction: _context.Transaction);

                return affectedRows > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<bool> UpdateAsync(Interaccion interaccion)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Interaccion>> ObtenerInteracciones(string ticketId)
        {
            const string sql = @"
                select 
                    interaccion_id,
                    ticket_id,
                    usuario_id,
                    mensaje,
                    fecha_registro,
                    fecha_modificacion
                from interacciones
                where ticket_id=@ticketId";

            try
            {
                var interacciones = await _context.Connection.QueryAsync<Interaccion>(sql, new { ticketId });
                return interacciones;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
