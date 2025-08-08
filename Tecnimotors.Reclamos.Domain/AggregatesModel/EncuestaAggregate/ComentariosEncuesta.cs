using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate
{
    public class ComentariosEncuesta
    {
        public int Id { get; set; }
        public int EncuestaEnviadaId { get; set; }
        public string TipoComentario { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaComentario { get; set; }
    }
}
