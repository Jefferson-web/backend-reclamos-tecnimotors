using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate
{
    public class PreguntasEncuesta
    {
        public int Id { get; set; }
        public int ConfiguracionEncuestaId { get; set; }
        public string Codigo { get; set; }
        public string TextoPregunta { get; set; }
        public string Categoria { get; set; }
        public int Orden { get; set; }
        public bool Obligatoria { get; set; }
        public bool Activa { get; set; }
    }
}
