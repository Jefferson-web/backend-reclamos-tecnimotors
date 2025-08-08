using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos
{
    public class DashboardDto
    {
        public IEnumerable<EstadisticaCardDto> EstadisticasCards { get; set; }
        public DistribucionEstadosDto DistribucionEstados { get; set; }
        public AnalisisMotivosParetoDto AnalisisMotivos { get; set; }
        public TendenciaReclamosDto TendenciaReclamos { get; set; }
    }

    public class EstadisticaCardDto
    {
        public string Icono { get; set; }
        public decimal Valor { get; set; }
        public string Titulo { get; set; }
        public decimal CambioPorcentual { get; set; }
        public string PeriodoTiempo { get; set; }
        public string Sufijo { get; set; }
        public bool EsDecimal { get; set; }
    }

    public class DistribucionEstadosDto
    {
        public IEnumerable<EstadoReclamoDto> Distribucion { get; set; }
    }

    public class EstadoReclamoDto
    {
        public string Estado { get; set; }
        public int Cantidad { get; set; }
    }

    public class AnalisisMotivosParetoDto
    {
        public IEnumerable<MotivoReclamoDto> Motivos { get; set; }
    }

    public class MotivoReclamoDto
    {
        public string Motivo { get; set; }
        public int Cantidad { get; set; }
        public decimal PorcentajeAcumulado { get; set; }
    }

    public class TendenciaReclamosDto
    {
        public IEnumerable<string> Meses { get; set; }
        public Dictionary<int, IEnumerable<int>> DatosPorAnio { get; set; }
    }

    public class DashboardFiltrosDto
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? Anio { get; set; }
    }
}
