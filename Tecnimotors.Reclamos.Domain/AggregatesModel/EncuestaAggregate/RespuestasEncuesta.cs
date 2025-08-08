using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate
{
    public class RespuestasEncuesta
    {
        public int Id { get; set; }
        public int EncuestaEnviadaId { get; set; }
        public int PreguntaId { get; set; }
        public int ValorLikert { get; set; }
        public DateTime FechaRespuesta { get; set; }
    }
}
