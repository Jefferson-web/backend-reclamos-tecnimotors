using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.CrearReclamo
{
    public class CrearReclamoCommand : IRequest<string>
    {
        public string Cliente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Detalle { get; set; }
        public string Prioridad { get; set; }
        public int MotivoId { get; set; }
        public string DepartamentoId { get; set; }
        public string ProvinciaId { get; set; }
        public string DistritoId { get; set; }
        public List<IFormFile> Archivos { get; set; } = new List<IFormFile>();
        public List<int> UsuariosAsignadosIds { get; set; } = new List<int>();
    }
}
