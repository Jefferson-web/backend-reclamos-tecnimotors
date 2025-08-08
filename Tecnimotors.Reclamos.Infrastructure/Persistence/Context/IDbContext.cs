using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Context
{
    public interface IDbContext: IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        Task<IDbTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        bool HasActiveTransaction { get; }
    }
}
