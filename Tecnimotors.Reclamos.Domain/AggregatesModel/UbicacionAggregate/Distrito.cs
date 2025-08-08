using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate
{
    public class Distrito
    {
        public string Id { get; private set; }
        public string Nombre { get; private set; }
        public string ProvinciaId { get; private set; }
        public string DepartamentoId { get; private set; }

        public Distrito(string id, string nombre, string provinciaId, string departamentoId)
        {
            Id = id;
            Nombre = nombre;
            ProvinciaId = provinciaId;
            DepartamentoId = departamentoId;
        }
    }
}
