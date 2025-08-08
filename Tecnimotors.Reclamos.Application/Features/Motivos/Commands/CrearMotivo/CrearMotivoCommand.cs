using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Commands.CrearMotivo
{
    public record CrearMotivoCommand: IRequest<CrearMotivoResponse>
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}
