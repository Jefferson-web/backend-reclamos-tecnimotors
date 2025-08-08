using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate
{
    public class Provincia
    {
        public string Id { get; private set; }
        public string Nombre { get; private set; }
        public string DepartamentoId { get; private set; }

        public Provincia(string id, string nombre, string departamentoId)
        {
            Id = id;
            Nombre = nombre;
            DepartamentoId = departamentoId;
        }
    }
}
