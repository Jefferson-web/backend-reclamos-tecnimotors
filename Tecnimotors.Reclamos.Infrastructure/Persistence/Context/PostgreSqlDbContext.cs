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
        private readonly object _lock = new object();

        public PostgreSqlDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("La cadena de conexión 'DefaultConnection' no está configurada");
        }

        public IDbConnection Connection
        {
            get
            {
                ThrowIfDisposed();

                lock (_lock)
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
        }

        public IDbTransaction Transaction => _transaction;

        public bool HasActiveTransaction => _transaction != null && !_disposed;

        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            ThrowIfDisposed();

            if (_transaction != null)
            {
                return _transaction;
            }

            var connection = Connection; // Esto asegura que la conexión esté abierta
            _transaction = await Task.Run(() => connection.BeginTransaction());
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            ThrowIfDisposed();

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
                CleanupTransaction();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_disposed) return; // Si ya está disposed, no hacer nada

            try
            {
                if (_transaction != null)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            _transaction.Rollback();
                        }
                        catch (ObjectDisposedException)
                        {
                            // La transacción ya fue disposed, ignorar
                        }
                        catch (InvalidOperationException)
                        {
                            // La transacción no está en un estado válido, ignorar
                        }
                    });
                }
            }
            finally
            {
                CleanupTransaction();
            }
        }

        private void CleanupTransaction()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // Ya fue disposed, ignorar
                }
                finally
                {
                    _transaction = null;
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PostgreSqlDbContext));
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
                    lock (_lock)
                    {
                        // Limpiar transacción primero
                        try
                        {
                            if (_transaction != null)
                            {
                                try
                                {
                                    _transaction.Rollback();
                                }
                                catch (ObjectDisposedException) { }
                                catch (InvalidOperationException) { }
                                finally
                                {
                                    _transaction.Dispose();
                                    _transaction = null;
                                }
                            }
                        }
                        catch (ObjectDisposedException) { }

                        // Limpiar conexión
                        try
                        {
                            if (_connection != null)
                            {
                                if (_connection.State == ConnectionState.Open)
                                {
                                    _connection.Close();
                                }
                                _connection.Dispose();
                                _connection = null;
                            }
                        }
                        catch (ObjectDisposedException) { }
                    }
                }
                _disposed = true;
            }
        }

        ~PostgreSqlDbContext()
        {
            Dispose(false);
        }
    }
}