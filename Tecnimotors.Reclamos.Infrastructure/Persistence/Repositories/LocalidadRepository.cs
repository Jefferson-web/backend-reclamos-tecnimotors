using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.LocalidadAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class LocalidadRepository : ILocalidadRepository
    {
        private readonly IDbContext _context;
        public LocalidadRepository(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Localidad> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    localidad_id AS LocalidadId, 
                    nombre AS Nombre, 
                    fecha_registro AS FechaRegistro, 
                    fecha_actualizacion AS FechaActualizacion
                FROM localidades
                WHERE localidad_id = @LocalidadId";

            return await _context.Connection.QueryFirstOrDefaultAsync<Localidad>(
                sql,
                new { LocalidadId = id },
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<Localidad>> GetAllAsync()
        {
            const string sql = @"
                SELECT 
                    localidad_id AS LocalidadId, 
                    nombre AS Nombre, 
                    fecha_registro AS FechaRegistro, 
                    fecha_actualizacion AS FechaActualizacion
                FROM localidades
                ORDER BY nombre";

            return await _context.Connection.QueryAsync<Localidad>(
                sql,
                transaction: _context.Transaction);
        }

        public async Task<int> CreateAsync(Localidad localidad)
        {
            const string sql = @"
                INSERT INTO localidades(
                    nombre, 
                    fecha_registro
                )
                VALUES(
                    @Nombre, 
                    @FechaRegistro
                )
                RETURNING localidad_id";

            return await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    localidad.Nombre,
                    localidad.FechaRegistro
                },
                transaction: _context.Transaction);
        }

        public async Task<bool> UpdateAsync(Localidad localidad)
        {
            const string sql = @"
                UPDATE localidades
                SET 
                    nombre = @Nombre,
                    fecha_actualizacion = @FechaActualizacion
                WHERE localidad_id = @LocalidadId";

            int affectedRows = await _context.Connection.ExecuteAsync(
                sql,
                new
                {
                    localidad.LocalidadId,
                    localidad.Nombre,
                    FechaActualizacion = DateTime.Now
                },
                transaction: _context.Transaction);

            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                DELETE FROM localidades
                WHERE localidad_id = @LocalidadId";

            int affectedRows = await _context.Connection.ExecuteAsync(
                sql,
                new { LocalidadId = id },
                transaction: _context.Transaction);

            return affectedRows > 0;
        }
    }
}
