using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;

namespace Tecnimotors.Reclamos.Application.Common.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Verificar si el request implementa ITransactionalRequest
            // Si no lo implementa, no aplicamos transacción
            if (!(request is ITransactionalRequest))
            {
                return await next();
            }

            var requestName = typeof(TRequest).Name;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ejecutar el siguiente handler en la cadena
                var response = await next();

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }

    // Interfaz de marcado para requests que requieren transacción
    public interface ITransactionalRequest { }
}