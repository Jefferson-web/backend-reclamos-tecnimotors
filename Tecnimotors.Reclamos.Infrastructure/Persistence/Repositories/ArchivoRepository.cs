using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class ArchivoRepository : IArchivoRepository
    {
        private readonly IDbContext _context;
        public ArchivoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Archivo archivo)
        {
            const string sql = @"
            INSERT INTO archivos (
                nombre_original, nombre_sistema, extension, tipo_mime,
                ruta_almacenamiento, tamano_bytes, fecha_subida
            ) VALUES (
                @NombreOriginal, @NombreSistema, @Extension, @TipoMime,
                @RutaAlmacenamiento, @TamanoBytes, @FechaSubida
            ) RETURNING archivo_id";

            return await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    archivo.NombreOriginal,
                    archivo.NombreSistema,
                    archivo.Extension,
                    archivo.TipoMime,
                    archivo.RutaAlmacenamiento,
                    archivo.TamanoBytes,
                    archivo.FechaSubida
                },
                transaction: _context.Transaction);
        }

        public async Task<bool> AsociarConReclamoAsync(int archivoId, string ticketId)
        {
            const string sql = @"
            INSERT INTO reclamo_archivos (
                ticket_id, archivo_id
            ) VALUES (
                @TicketId, @ArchivoId
            )";

            var affectedRows = await _context.Connection.ExecuteAsync(
                sql,
                new
                {
                    TicketId = ticketId,
                    ArchivoId = archivoId
                },
                transaction: _context.Transaction);

            return affectedRows > 0;
        }

        public async Task<Archivo> GetByIdAsync(int archivoId)
        {
            const string sql = @"
                SELECT 
                    archivo_id AS ArchivoId,
                    nombre_original AS NombreOriginal,
                    nombre_sistema AS NombreSistema,
                    extension AS Extension,
                    tipo_mime AS TipoMime,
                    ruta_almacenamiento AS RutaAlmacenamiento,
                    tamano_bytes AS TamanoBytes,
                    fecha_subida AS FechaSubida
                FROM archivos
                WHERE archivo_id = @ArchivoId";

            return await _context.Connection.QueryFirstOrDefaultAsync<Archivo>(
                sql,
                new { ArchivoId = archivoId },
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<Archivo>> GetByReclamoAsync(string ticketId)
        {
            const string sql = @"
                SELECT 
                    a.archivo_id AS ArchivoId,
                    a.nombre_original AS NombreOriginal,
                    a.nombre_sistema AS NombreSistema,
                    a.extension AS Extension,
                    a.tipo_mime AS TipoMime,
                    a.ruta_almacenamiento AS RutaAlmacenamiento,
                    a.tamano_bytes AS TamanoBytes,
                    a.fecha_subida AS FechaSubida
                FROM archivos a
                JOIN reclamo_archivos ra ON a.archivo_id = ra.archivo_id
                WHERE ra.ticket_id = @TicketId
                ORDER BY a.fecha_subida DESC";

            return await _context.Connection.QueryAsync<Archivo>(
                sql,
                new { TicketId = ticketId },
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<Archivo>> GetByInteraccionAsync(string interaccionId)
        {
            const string sql = @"
                SELECT 
                    a.archivo_id AS ArchivoId,
                    a.nombre_original AS NombreOriginal,
                    a.nombre_sistema AS NombreSistema,
                    a.extension AS Extension,
                    a.tipo_mime AS TipoMime,
                    a.ruta_almacenamiento AS RutaAlmacenamiento,
                    a.tamano_bytes AS TamanoBytes,
                    a.fecha_subida AS FechaSubida
                FROM archivos a
                JOIN interaccion_archivos ia ON a.archivo_id = ia.archivo_id
                WHERE ia.interaccion_id = @InteraccionId
                ORDER BY a.fecha_subida DESC";

            return await _context.Connection.QueryAsync<Archivo>(
                sql,
                new { InteraccionId = interaccionId },
                transaction: _context.Transaction);
        }

        public async Task<bool> AsociarArchivoInteraccionAsync(string interaccionId, int archivoId)
        {
            const string sql = @"
                INSERT INTO interaccion_archivos(
                    interaccion_id,
                    archivo_id
                )
                VALUES(
                    @InteraccionId,
                    @ArchivoId
                )";

            int affectedRows = await _context.Connection.ExecuteAsync(
                sql,
                new { InteraccionId = interaccionId, ArchivoId = archivoId },
                transaction: _context.Transaction);

            return affectedRows > 0;
        }
    }
}
