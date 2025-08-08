using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.PlantillaAggregate
{
    public class PlantillaCorreo
    {
        public int Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public string Asunto { get; private set; }
        public string ContenidoHtml { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public DateTime FechaActualizacion { get; private set; }
        public string Descripcion { get; private set; }
        protected PlantillaCorreo()
        {
            
        }
    }
}
