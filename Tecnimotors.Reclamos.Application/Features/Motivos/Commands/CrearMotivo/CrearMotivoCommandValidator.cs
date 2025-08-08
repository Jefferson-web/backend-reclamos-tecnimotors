using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Commands.CrearMotivo
{
    public class CrearMotivoCommandValidator: AbstractValidator<CrearMotivoCommand>
    {
        public CrearMotivoCommandValidator()
        {
            RuleFor(m => m.Nombre).NotEmpty();
            RuleFor(m => m.Descripcion).NotEmpty();
        }
    }
}
