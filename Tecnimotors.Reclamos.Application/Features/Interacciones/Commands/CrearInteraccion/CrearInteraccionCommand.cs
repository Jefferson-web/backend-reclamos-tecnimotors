using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Interacciones.Commands.CrearInteraccion
{
    public class CrearInteraccionCommand : IRequest
    {
        public string TicketId { get; set; }
        public string Mensaje { get; set; }
        public List<IFormFile> Archivos { get; set; } = new List<IFormFile>();
    }
}
