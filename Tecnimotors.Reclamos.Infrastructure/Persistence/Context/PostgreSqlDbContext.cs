using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Infrastructure.Persistence.Context
{
    internal class PostgreSqlDbContext : IDbContext
    {
        private readonly string _connectionString;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool _disposed;

        public PostgreSqlDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("La cadena de conexión 'DefaultConnection' no está configurada");
        }

        public IDbConnection Connection
        {
            get 
            {
                if (_connection == null)
                {
                    _connection = new NpgsqlConnection(_connectionString);
                    _connection.Open();
                }
                else if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                return _connection;
            }
        }

        public IDbTransaction Transaction => _transaction;

        public bool HasActiveTransaction => _transaction != null;

        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                return _transaction;
            }

            if (ConnectionState.Open != Connection.State)
            {
                await Task.Run(() => Connection.Open());
            }

            _transaction = Connection.BeginTransaction();
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction == null)
                {
                    throw new InvalidOperationException("No hay una transacción activa para confirmar");
                }

                await Task.Run(() => _transaction.Commit());
            }
            catch (Exception)
            {
                await RollbackTransactionAsync();
                throw;
            } 
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _connection?.Dispose();
                    _transaction = null;
                    _connection = null;
                }
                _disposed = true;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await Task.Run(() => _transaction.Rollback());
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }
    }
}
