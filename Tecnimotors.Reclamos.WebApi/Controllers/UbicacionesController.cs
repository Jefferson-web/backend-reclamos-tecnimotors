// Tecnimotors.Reclamos.WebApi/Controllers/UbicacionesController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Features.Ubicaciones.Queries.GetDepartamentos;
using Tecnimotors.Reclamos.Application.Features.Ubicaciones.Queries.GetDistritosByProvincia;
using Tecnimotors.Reclamos.Application.Features.Ubicaciones.Queries.GetProvinciasByDepartamento;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UbicacionesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UbicacionesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("departamentos")]
        public async Task<IActionResult> GetDepartamentos()
        {
            var query = new GetDepartamentosQuery();
            var departamentos = await _mediator.Send(query);
            return Ok(departamentos);
        }

        [HttpGet("provincias/{departamentoId}")]
        public async Task<IActionResult> GetProvinciasByDepartamento(string departamentoId)
        {
            var query = new GetProvinciasByDepartamentoQuery(departamentoId);
            var provincias = await _mediator.Send(query);
            return Ok(provincias);
        }

        [HttpGet("distritos/{provinciaId}")]
        public async Task<IActionResult> GetDistritosByProvincia(string provinciaId)
        {
            var query = new GetDistritosByProvinciaQuery(provinciaId);
            var distritos = await _mediator.Send(query);
            return Ok(distritos);
        }
    }
}