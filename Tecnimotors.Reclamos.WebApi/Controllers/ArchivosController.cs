using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tecnimotors.Reclamos.Application.Features.Archivos.Queries.DownloadArchivo;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArchivosController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ArchivosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadArchivo(int id)
        {
            try
            {
                var query = new DownloadArchivoQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFound($"No se encontró el archivo con ID {id}");

                return File(result.Contenido, result.TipoContenido, result.NombreArchivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al descargar el archivo. Por favor, contacte al administrador.");
            }
        }
    }
}
