using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.CrearReclamo
{
    public class CrearReclamoCommandValidator : AbstractValidator<CrearReclamoCommand>
    {
        private readonly IMotivoRepository _motivoRepository;
        private readonly IUbicacionRepository _ubicacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CrearReclamoCommandValidator(
            IMotivoRepository motivoRepository,
            IUbicacionRepository ubicacionRepository,
            IUsuarioRepository usuarioRepository)
        {
            _motivoRepository = motivoRepository;
            _ubicacionRepository = ubicacionRepository;
            _usuarioRepository = usuarioRepository;

            RuleFor(x => x.Cliente)
                .NotEmpty().WithMessage("El cliente es obligatorio")
                .MaximumLength(100).WithMessage("El cliente no debe exceder los 100 caracteres");

            RuleFor(x => x.Nombres)
                .NotEmpty().WithMessage("Los nombres son obligatorios")
                .MaximumLength(100).WithMessage("Los nombres no deben exceder los 100 caracteres");

            RuleFor(x => x.Apellidos)
                .NotEmpty().WithMessage("Los apellidos son obligatorios")
                .MaximumLength(100).WithMessage("Los apellidos no deben exceder los 100 caracteres");

            RuleFor(x => x.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio")
                .MaximumLength(20).WithMessage("El teléfono no debe exceder los 20 caracteres")
                .Matches(@"^[0-9+\-\s]+$").WithMessage("El teléfono solo debe contener números y caracteres permitidos");

            RuleFor(x => x.Correo)
                .EmailAddress().WithMessage("Debe ingresar un correo electrónico válido")
                .MaximumLength(100).WithMessage("El correo no debe exceder los 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Correo));

            RuleFor(x => x.Detalle)
                .NotEmpty().WithMessage("El detalle del reclamo es obligatorio");

            RuleFor(x => x.MotivoId)
                .NotEmpty().WithMessage("El motivo es obligatorio")
                .MustAsync(async (motivoId, cancellation) =>
                {
                    var motivo = await _motivoRepository.GetByIdAsync(motivoId);
                    return motivo != null && motivo.Activo;
                }).WithMessage("El motivo seleccionado no existe o no está activo");

            RuleFor(x => x.DepartamentoId)
                .NotEmpty().WithMessage("El departamento es obligatorio")
                .MustAsync(async (departamentoId, cancellation) =>
                {
                    return await ubicacionRepository.ExisteDepartamentoAsync(departamentoId);
                }).WithMessage("El departamento seleccionado no existe");

            RuleFor(x => x.ProvinciaId)
                .NotEmpty().WithMessage("La provincia es obligatoria")
                .MustAsync(async (model, provinciaId, cancellation) =>
                {
                    return await ubicacionRepository.ExisteProvinciaAsync(provinciaId, model.DepartamentoId);
                }).WithMessage("La provincia seleccionada no existe o no pertenece al departamento indicado");

            RuleFor(x => x.DistritoId)
                .NotEmpty().WithMessage("El distrito es obligatorio")
                .MustAsync(async (model, distritoId, cancellation) =>
                {
                    return await ubicacionRepository.ExisteDistritoAsync(distritoId, model.ProvinciaId);
                }).WithMessage("El distrito seleccionado no existe o no pertenece a la provincia indicada");

            // Validaciones para archivos adjuntos
            RuleForEach(x => x.Archivos)
                .Must(archivo => archivo != null && archivo.Length > 0)
                .WithMessage("Los archivos no pueden estar vacíos")
                .Must(archivo => archivo.Length <= 10 * 1024 * 1024) // 10MB máximo
                .WithMessage("Los archivos no deben exceder los 10MB")
                .Must(ArchivoTieneExtensionPermitida)
                .WithMessage("Solo se permiten archivos con extensiones: .pdf, .jpg, .jpeg, .png, .doc, .docx, .xls, .xlsx");

            // Validaciones para usuarios asignados
            RuleForEach(x => x.UsuariosAsignadosIds)
                .MustAsync(async (usuarioId, cancellation) => {
                    var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
                    return usuario != null && usuario.Activo;
                }).WithMessage("Uno o más usuarios seleccionados no existen o no están activos");

            // Validación del número máximo de usuarios que se pueden asignar
            RuleFor(x => x.UsuariosAsignadosIds)
                .Must(lista => lista == null || lista.Count <= 10) // Máximo 10 usuarios asignados
                .WithMessage("No se pueden asignar más de 10 usuarios a un reclamo");
        }

        private bool ArchivoTieneExtensionPermitida(IFormFile archivo)
        {
            if (archivo == null)
                return false;

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            var extensionesPermitidas = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".txt" };

            return extensionesPermitidas.Contains(extension);
        }
    }
}
