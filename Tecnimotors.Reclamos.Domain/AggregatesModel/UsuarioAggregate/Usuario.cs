using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate
{
    public class Usuario
    {
        public int UsuarioId { get; private set; }
        public string Email { get; private set; }
        public byte[] Password { get; private set; }
        public byte[] PasswordSalt { get; private set; }
        public string Nombre { get; private set; }
        public string Apellidos { get; private set; }
        public bool Activo { get; private set; }
        public int RolId { get; set; }
        public string NombreRol { get; set; }
        public DateTime FechaRegistro { get; private set; }
        public DateTime? FechaActualizacion { get; private set; }

        protected Usuario()
        {
            
        }

        public Usuario(int usuarioId, string email, string nombre, string apellidos, int rolId)
        {
            UsuarioId = usuarioId;
            Email = email;
            Nombre = nombre;
            Apellidos = apellidos;
            RolId = rolId;
        }

        public Usuario(int usuarioId, string email, byte[] password, byte[] passwordSalt, string nombre, string apellidos, int rolId)
        {
            UsuarioId = usuarioId;
            Email = email;
            Password = password;
            PasswordSalt = passwordSalt;
            Nombre = nombre;
            Apellidos = apellidos;
            RolId = rolId;
            FechaRegistro = DateTime.Now;
        }

        public Usuario(string email, byte[] password, byte[] passwordSalt, string nombre, string apellidos, int rolId)
        {
            Email = email;
            Password = password;
            PasswordSalt = passwordSalt;
            Nombre = nombre;
            Apellidos = apellidos;
            RolId = rolId;
            FechaRegistro = DateTime.Now;
        }

        public void ActualizarPassword(byte[] nuevoPassword, byte[] nuevoPasswordSalt)
        {
            if (nuevoPassword != null && nuevoPassword.Length == 0)
            {
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(nuevoPassword));
            }
            Password = nuevoPassword;
            PasswordSalt = nuevoPasswordSalt;
            FechaActualizacion = DateTime.Now;
        }
    }
}
