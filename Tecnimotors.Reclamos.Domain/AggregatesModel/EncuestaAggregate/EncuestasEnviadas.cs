using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate
{
    public class EncuestasEnviadas
    {
        public int Id { get; set; }
        public string TicketId { get; set; }
        public int ConfiguracionEncuestaId { get; set; }
        public Guid TokenAcceso { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaRespuesta { get; set; }
        public string EstadoEncuesta { get; set; }
        public string EmailEnviado { get; set; }
        public string IPRespuesta { get; set; }
        public string DispositivoRespuesta { get; set; }
    }
}
