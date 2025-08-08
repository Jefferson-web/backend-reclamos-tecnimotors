using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate
{
    public class ConfiguracionEncuesta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string TipoEncuesta { get; set; }
        public int DiasEsperaEnvio { get; set; }
        public int DiasVigencia { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
