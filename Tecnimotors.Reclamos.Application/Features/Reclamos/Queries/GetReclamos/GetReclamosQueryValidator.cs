using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetReclamos
{
    public class GetReclamosQueryValidator : AbstractValidator<GetReclamosQuery>
    {
        public GetReclamosQueryValidator()
        {
            // Hacer que estos campos sean opcionales
            RuleFor(x => x.TicketId).Empty();
            RuleFor(x => x.Estado).Empty();
            RuleFor(x => x.Prioridad).Empty();

            // Validaciones para PageNumber y PageSize
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("El número de página debe ser mayor a 0");
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100");

            // Validación para FechaDesde y FechaHasta
            When(x => x.FechaDesde.HasValue && x.FechaHasta.HasValue, () => {
                RuleFor(x => x.FechaHasta)
                    .GreaterThanOrEqualTo(x => x.FechaDesde)
                    .WithMessage("La fecha hasta debe ser mayor o igual a la fecha desde");
            });
        }
    }
}
