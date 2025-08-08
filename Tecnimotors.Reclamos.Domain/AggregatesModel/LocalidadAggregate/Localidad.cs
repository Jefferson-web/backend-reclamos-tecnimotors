using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.LocalidadAggregate
{
    public class Localidad
    {
        public int LocalidadId { get; private set; }
        public string Nombre { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public DateTime? FechaActualizacion { get; private set; }

        // Constructor protegido para EF Core/Dapper
        protected Localidad()
        {
        }

        public Localidad(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la localidad no puede estar vacío", nameof(nombre));

            Nombre = nombre;
            FechaRegistro = DateTime.UtcNow;
        }

        public void ActualizarNombre(string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre de la localidad no puede estar vacío", nameof(nuevoNombre));

            Nombre = nuevoNombre;
            FechaActualizacion = DateTime.UtcNow;
        }
    }
}
