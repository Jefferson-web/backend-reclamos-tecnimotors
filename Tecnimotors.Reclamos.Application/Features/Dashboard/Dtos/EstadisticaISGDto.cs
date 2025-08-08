using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Dashboard.Dtos
{
    public class EstadisticaISGDto
    {
        public string Periodo { get; set; }
        public int TotalEncuestas { get; set; }
        public decimal ISG { get; set; }
        public int MuySatisfechos { get; set; }
        public int Satisfechos { get; set; }
        public int Neutrales { get; set; }
        public int Insatisfechos { get; set; }
    }
}
