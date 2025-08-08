using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tecnimotors.Reclamos.Application.Features.Dashboard.Queries;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetDashboardData([FromQuery] DashboardFiltrosDto filtros)
        {
            var query = new GetDashboardDataQuery(filtros.FechaDesde, filtros.FechaHasta, filtros.Anio);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("estadisticas-generales")]
        public async Task<ActionResult<DistribucionEstadosDto>> GetEstadisticasGenerales([FromQuery] DateTime? fechaDesde = null, [FromQuery] DateTime? fechaHasta = null)
        {
            var query = new GetEstadisticasGeneralesQuery(fechaDesde, fechaHasta);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("distribucion-estados")]
        public async Task<ActionResult<DistribucionEstadosDto>> GetDistribucionEstados([FromQuery] DateTime? fechaDesde = null, [FromQuery] DateTime? fechaHasta = null)
        {
            var query = new GetDistribucionEstadosQuery(fechaDesde, fechaHasta);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("analisis-motivos")]
        public async Task<ActionResult<AnalisisMotivosParetoDto>> GetAnalisisMotivos([FromQuery] DateTime? fechaDesde = null, [FromQuery] DateTime? fechaHasta = null)
        {
            var query = new GetAnalisisMotivosParetoQuery(fechaDesde, fechaHasta);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("tendencia-reclamos")]
        [ProducesResponseType(typeof(TendenciaReclamosDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TendenciaReclamosDto>> GetTendenciaReclamos([FromQuery] int? anio = null)
        {
            try
            {
                var query = new TendenciaReclamosQuery(anio);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al obtener la tendencia de reclamos", error = ex.Message });
            }
        }
    }
}
