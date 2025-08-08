using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate
{
    public class InteraccionArchivo
    {
        public int InteraccionId { get; private set; }
        public int ArchivoId { get; private set; }

        protected InteraccionArchivo() { }

        public InteraccionArchivo(int interaccionId, int archivoId)
        {
            InteraccionId = interaccionId;
            ArchivoId = archivoId;
        }
    }
}
