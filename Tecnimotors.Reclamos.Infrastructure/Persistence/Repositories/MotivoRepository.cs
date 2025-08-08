using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class MotivoRepository : IMotivoRepository
    {
        private readonly IDbContext _dbContext;

        public MotivoRepository(IDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Motivo>> GetAllAsync()
        {
            const string sql = @"
                SELECT 
                    motivo_id AS MotivoId, 
                    nombre AS Nombre, 
                    descripcion AS Descripcion, 
                    activo AS Activo, 
                    fecha_registro AS FechaRegistro, 
                    fecha_actualizacion AS FechaActualizacion
                FROM motivos
                ORDER BY nombre";

            var motivos = await _dbContext.Connection.QueryAsync<Motivo>(sql, transaction: _dbContext.Transaction);

            return motivos;
        }

        public async Task<Motivo> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    motivo_id AS MotivoId, 
                    nombre AS Nombre, 
                    descripcion AS Descripcion, 
                    activo AS Activo, 
                    fecha_registro AS FechaRegistro, 
                    fecha_actualizacion AS FechaActualizacion
                FROM motivos
                WHERE motivo_id = @MotivoId";

            var result = await _dbContext.Connection.QuerySingleOrDefaultAsync<Motivo>(
                sql,
                new { MotivoId = id },
                transaction: _dbContext.Transaction);

            if (result == null)
                return null;

            return result;
        }

        public async Task AddAsync(Motivo motivo)
        {
            const string sql = @"
                INSERT INTO motivos (nombre, descripcion, activo, fecha_registro)
                VALUES (@Nombre, @Descripcion, @Activo, @FechaRegistro)
                RETURNING motivo_id";

            var parameters = new
            {
                motivo.Nombre,
                motivo.Descripcion,
                motivo.Activo,
                motivo.FechaRegistro
            };

            var motivoId = await _dbContext.Connection.ExecuteScalarAsync<int>(
                sql,
                parameters,
                transaction: _dbContext.Transaction);

            typeof(Motivo).GetProperty("MotivoId")?.SetValue(motivo, motivoId);
        }

        public async Task UpdateAsync(Motivo motivo)
        {
            const string sql = @"
                UPDATE motivos
                SET nombre = @Nombre,
                    descripcion = @Descripcion,
                    activo = @Activo,
                    fecha_actualizacion = @FechaActualizacion
                WHERE motivo_id = @MotivoId";

            var parameters = new
            {
                motivo.MotivoId,
                motivo.Nombre,
                motivo.Descripcion,
                motivo.Activo,
                FechaActualizacion = DateTime.UtcNow
            };

            await _dbContext.Connection.ExecuteAsync(
                sql,
                parameters,
                transaction: _dbContext.Transaction);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                UPDATE motivos
                SET activo = false,
                    fecha_actualizacion = @FechaActualizacion
                WHERE motivo_id = @MotivoId";

            var affected = await _dbContext.Connection.ExecuteAsync(
                sql,
                new
                {
                    MotivoId = id,
                    FechaActualizacion = DateTime.UtcNow
                },
                transaction: _dbContext.Transaction);

            return affected > 0;
        }

        public async Task<(IEnumerable<Motivo>, int)> GetPaginatedAsync(int pageIndex, int pageSize, string nombreFilter = null)
        {
            var parametros = new DynamicParameters();
            string whereClause = string.Empty;

            // Aplicar filtro por nombre si está definido
            if (!string.IsNullOrWhiteSpace(nombreFilter))
            {
                whereClause = "WHERE LOWER(nombre) LIKE LOWER(@Nombre)";
                parametros.Add("Nombre", $"%{nombreFilter}%");
            }

            // Consulta para contar el total de registros
            string countSql = $@"
        SELECT COUNT(*)
        FROM motivos
        {whereClause}";

            // Consulta para obtener los datos paginados
            string dataSql = $@"
        SELECT 
            motivo_id AS MotivoId, 
            nombre AS Nombre, 
            descripcion AS Descripcion, 
            activo AS Activo, 
            fecha_registro AS FechaRegistro, 
            fecha_actualizacion AS FechaActualizacion
        FROM motivos
        {whereClause}
        ORDER BY nombre
        LIMIT @PageSize OFFSET @Offset";

            parametros.Add("PageSize", pageSize);
            parametros.Add("Offset", (pageIndex - 1) * pageSize);

            // Ejecutar ambas consultas
            var totalCount = await _dbContext.Connection.ExecuteScalarAsync<int>(countSql, parametros, transaction: _dbContext.Transaction);
            var motivos = await _dbContext.Connection.QueryAsync<Motivo>(dataSql, parametros, transaction: _dbContext.Transaction);

            return (motivos, totalCount);
        }
    }
}