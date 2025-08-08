using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.RolAggregate
{
    public class Rol
    {
        public int RolId { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public DateTime? FechaActualizacion { get; private set; }

        protected Rol()
        {

        }

        public Rol(string nombre, string descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = true;
            FechaRegistro = DateTime.Now;
        }

        public Rol(int rolId, string nombre, string descripcion)
        {
            RolId = rolId;
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = true;
            FechaRegistro = DateTime.Now;
        }
    }
}
