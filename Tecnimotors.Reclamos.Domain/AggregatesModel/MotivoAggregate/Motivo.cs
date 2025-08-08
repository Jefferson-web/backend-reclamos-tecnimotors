using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate
{
    public class Motivo
    {
        public int MotivoId { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public DateTime? FechaActualizacion { get; private set; }

        public Motivo()
        {
            
        }

        public Motivo(string nombre, string descripcion) 
        { 
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
            Activo = true;
            FechaRegistro = DateTime.Now;
        }

        public void Actualizar(string nombre, string descripcion, bool activo)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
            Activo = activo;
            FechaActualizacion = DateTime.Now;
        }

        public void Desactivar()
        {
            Activo = false;
            FechaActualizacion = DateTime.Now;
        }

        public void Activar()
        {
            Activo = true;
            FechaActualizacion = DateTime.Now;
        }
    }
}
