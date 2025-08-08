using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Infrastructure.Persistence.Context;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbContext _context;
        public UsuarioRepository(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
        }

        public async Task<int> CreateAsync(Usuario usuario)
        {
            const string sql = @"
                INSERT INTO usuarios(
                    email, 
                    password_hash, 
                    password_salt,
                    nombre, 
                    apellidos, 
                    activo,
                    rol_id,
                    fecha_registro
                )
                VALUES(
                    @Email, 
                    @Password, 
                    @PasswordSalt,
                    @Nombre, 
                    @Apellidos, 
                    true,
                    @RolId,
                    @FechaRegistro
                )
                RETURNING usuario_id";

            return await _context.Connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    usuario.UsuarioId,
                    usuario.Email,
                    usuario.Password,
                    usuario.PasswordSalt,
                    usuario.Nombre,
                    usuario.Apellidos,
                    usuario.RolId,
                    usuario.FechaRegistro
                },
                transaction: _context.Transaction);
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            const string sql = @"
                UPDATE usuarios
                SET activo = false,
                    fecha_actualizacion = @FechaActualizacion
                WHERE usuario_id = @UsuarioId";

            int affectedRows = await _context.Connection.ExecuteAsync(
                sql,
                new { UsuarioId = id, FechaActualizacion = DateTime.Now },
                transaction: _context.Transaction);

            return affectedRows > 0;
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            const string sql = @"
                SELECT u.usuario_id AS UsuarioId, 
                       u.email AS Email, 
                       u.password_hash AS Password, 
                       u.password_salt AS PasswordSalt,
                       u.nombre AS Nombre, 
                       u.apellidos AS Apellidos, 
                       u.activo AS Activo, 
                       u.rol_id AS RolId,
                       r.nombre AS NombreRol,
                       u.fecha_registro AS FechaRegistro, 
                       u.fecha_actualizacion AS FechaActualizacion
                FROM usuarios u
                INNER JOIN roles r ON u.rol_id = r.rol_id
                WHERE u.email = @Email";

            return await _context.Connection.QueryFirstOrDefaultAsync<Usuario>(
                sql,
                new { Email = email },
                transaction: _context.Transaction);
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT u.usuario_id AS UsuarioId, 
                       u.email AS Email, 
                       u.password_hash AS Password, 
                       u.password_salt AS PasswordSalt,
                       u.nombre AS Nombre, 
                       u.apellidos AS Apellidos, 
                       u.activo AS Activo, 
                       u.rol_id AS RolId,
                       r.nombre AS NombreRol,
                       u.fecha_registro AS FechaRegistro, 
                       u.fecha_actualizacion AS FechaActualizacion
                FROM usuarios u
                INNER JOIN roles r ON u.rol_id = r.rol_id
                WHERE u.usuario_id = @UsuarioId";

            return await _context.Connection.QueryFirstOrDefaultAsync<Usuario>(
                sql,
                new { UsuarioId = id },
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<Usuario>> GetByRolIdAsync(int rolId)
        {
            const string sql = @"
                SELECT u.usuario_id AS UsuarioId, 
                       u.email AS Email, 
                       u.password_hash AS Password, 
                       u.password_salt AS PasswordSalt,
                       u.nombre AS Nombre, 
                       u.apellidos AS Apellidos, 
                       u.activo AS Activo, 
                       u.rol_id AS RolId,
                       r.nombre AS NombreRol,
                       u.fecha_registro AS FechaRegistro, 
                       u.fecha_actualizacion AS FechaActualizacion
                FROM usuarios u
                INNER JOIN roles r ON u.rol_id = r.rol_id
                WHERE u.rol_id = @RolId AND u.activo = true";

            return await _context.Connection.QueryAsync<Usuario>(
                sql,
                new { RolId = rolId },
                transaction: _context.Transaction);
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosByRol(string rol)
        {
            const string sql = @"
                SELECT u.usuario_id AS UsuarioId, 
                       u.email AS Email, 
                       u.password_hash AS Password, 
                       u.password_salt AS PasswordSalt,
                       u.nombre AS Nombre, 
                       u.apellidos AS Apellidos, 
                       u.activo AS Activo, 
                       u.rol_id AS RolId,
                       r.nombre AS NombreRol,
                       u.fecha_registro AS FechaRegistro, 
                       u.fecha_actualizacion AS FechaActualizacion
                FROM usuarios u
                INNER JOIN roles r ON u.rol_id = r.rol_id
                WHERE r.nombre = @Rol AND u.activo = true";

            return await _context.Connection.QueryAsync<Usuario>(
                sql,
                new { Rol = rol },
                transaction: _context.Transaction);
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            const string sql = @"
                UPDATE usuarios
                SET email = @Email,
                    password_hash = @Password,
                    password_salt = @PasswordSalt,
                    nombre = @Nombre,
                    apellidos = @Apellidos,
                    activo = @Activo,
                    rol_id = @RolId,
                    fecha_actualizacion = @FechaActualizacion
                WHERE usuario_id = @UsuarioId";

            int affectedRows = await _context.Connection.ExecuteAsync(
                sql,
                new
                {
                    usuario.UsuarioId,
                    usuario.Email,
                    Password = usuario.Password,
                    PasswordSalt = usuario.PasswordSalt,
                    usuario.Nombre,
                    usuario.Apellidos,
                    usuario.Activo,
                    usuario.RolId,
                    FechaActualizacion = DateTime.Now
                },
                transaction: _context.Transaction);

            return affectedRows > 0;
        }
    }
}
