using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Auth.Commands.RegistrarUsuario
{
    public class RegistrarUsuarioCommandValidator: AbstractValidator<RegistrarUsuarioCommand>
    {
        public RegistrarUsuarioCommandValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(r => r.Nombre).NotEmpty();
            RuleFor(r => r.Apellidos).NotEmpty();
            RuleFor(r => r.Password).NotEmpty();
            RuleFor(r => r.RolId).NotNull();
        }
    }
}
