using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class UbicacionRepository : IUbicacionRepository
    {
        private readonly IDbContext _context;

        public UbicacionRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Departamento>> GetDepartamentosAsync()
        {
            const string sql = @"
                SELECT id AS Id, nombre AS Nombre
                FROM departamentos
                ORDER BY nombre";

            return await _context.Connection.QueryAsync<Departamento>(
                sql,
                transaction: _context.Transaction);
        }

        public async Task<Departamento> GetDepartamentoByIdAsync(string id)
        {
            const string sql = @"
                SELECT id AS Id, nombre AS Nombre
                FROM departamentos
                WHERE id = @Id";

            return await _context.Connection.QueryFirstOrDefaultAsync<Departamento>(
                sql,
                new { Id = id },
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<Provincia>> GetProvinciasByDepartamentoIdAsync(string departamentoId)
        {
            const string sql = @"
                SELECT id AS Id, nombre AS Nombre, departamento_id AS DepartamentoId
                FROM provincias
                WHERE departamento_id = @DepartamentoId
                ORDER BY nombre";

            return await _context.Connection.QueryAsync<Provincia>(
                sql,
                new { DepartamentoId = departamentoId },
                transaction: _context.Transaction);
        }

        public async Task<Provincia> GetProvinciaByIdAsync(string id)
        {
            const string sql = @"
                SELECT id AS Id, nombre AS Nombre, departamento_id AS DepartamentoId
                FROM provincias
                WHERE id = @Id";

            return await _context.Connection.QueryFirstOrDefaultAsync<Provincia>(
                sql,
                new { Id = id },
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<Distrito>> GetDistritosByProvinciaIdAsync(string provinciaId)
        {
            const string sql = @"
                SELECT id AS Id, nombre AS Nombre, provincia_id AS ProvinciaId, departamento_id AS DepartamentoId
                FROM distritos
                WHERE provincia_id = @ProvinciaId
                ORDER BY nombre";

            return await _context.Connection.QueryAsync<Distrito>(
                sql,
                new { ProvinciaId = provinciaId },
                transaction: _context.Transaction);
        }

        public async Task<Distrito> GetDistritoByIdAsync(string id)
        {
            const string sql = @"
                SELECT id AS Id, nombre AS Nombre, provincia_id AS ProvinciaId, departamento_id AS DepartamentoId
                FROM distritos
                WHERE id = @Id";

            return await _context.Connection.QueryFirstOrDefaultAsync<Distrito>(
                sql,
                new { Id = id },
                transaction: _context.Transaction);
        }

        public async Task<bool> ExisteDepartamentoAsync(string departamentoId)
        {
            const string sql = @"
        SELECT COUNT(1)
        FROM departamentos
        WHERE id = @DepartamentoId";

            var count = await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                new { DepartamentoId = departamentoId },
                transaction: _context.Transaction);

            return count > 0;
        }

        public async Task<bool> ExisteProvinciaAsync(string provinciaId, string departamentoId)
        {
            const string sql = @"
        SELECT COUNT(1)
        FROM provincias
        WHERE id = @ProvinciaId AND departamento_id = @DepartamentoId";

            var count = await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                new { ProvinciaId = provinciaId, DepartamentoId = departamentoId },
                transaction: _context.Transaction);

            return count > 0;
        }

        public async Task<bool> ExisteDistritoAsync(string distritoId, string provinciaId)
        {
            const string sql = @"
        SELECT COUNT(1)
        FROM distritos
        WHERE id = @DistritoId AND provincia_id = @ProvinciaId";

            var count = await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                new { DistritoId = distritoId, ProvinciaId = provinciaId },
                transaction: _context.Transaction);

            return count > 0;
        }
    }
}
