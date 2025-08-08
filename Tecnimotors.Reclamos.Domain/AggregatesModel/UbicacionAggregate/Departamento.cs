using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate
{
    public class Departamento
    {
        public string Id { get; private set; }
        public string Nombre { get; private set; }

        public Departamento(string id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
    }
}
