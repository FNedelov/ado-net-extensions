using ExtensionTesterDI.Interfaces;
using System.Data;
using System.Data.Common;

namespace ExtensionTesterDI.Classes
{
    public class DatabaseCaller : IDisposable
    {
        private IExtensionsCaller _dbProvider;

        public DatabaseCaller(IExtensionsCaller dbProvider)
        {
            _dbProvider = dbProvider;
            _dbProvider.DBConn.Open();
        }

        public void Dispose()
        {
            _dbProvider.DBConn.Close();
        }

        public bool ExecuteNonQuery(string query, List<DbParameter>? parameters = null)
        {
            return _dbProvider.ExecuteNonQuery(query, parameters);
        }

        public async Task<bool> ExecuteNonQueryAsync(string query, List<DbParameter>? parameters = null, CancellationToken ctk = default)
        {
            return await _dbProvider.ExecuteNonQueryAsync(query, parameters, ctk);
        }

        public DataTable ExecuteQueryAdapter(string query, List<DbParameter>? parameters = null)
        {
            return _dbProvider.ExecuteQueryAdapter(query, parameters);
        }

        public async Task<DataTable> ExecuteQueryAdapterAsync(string query, List<DbParameter>? parameters = null, CancellationToken ctk = default)
        {
            return await _dbProvider.ExecuteQueryAdapterAsync(query, parameters, ctk);
        }

        public DbDataReader ExecuteQueryReader(string query, List<DbParameter>? parameters = null)
        {
            return _dbProvider.ExecuteQueryReader(query, parameters);
        }

        public async Task<DbDataReader> ExecuteQueryReaderAsync(string query, List<DbParameter>? parameters = null, CancellationToken ctk = default)
        {
            return await _dbProvider.ExecuteQueryReaderAsync(query, parameters, ctk);
        }

        public T? ExecuteScalar<T>(string query, List<DbParameter>? parameters = null, T? defaultValue = default)
        {
            return _dbProvider.ExecuteScalar(query, parameters, defaultValue);
        }

        public async Task<T?> ExecuteScalarAsync<T>(string query, List<DbParameter>? parameters = null, T? defaultValue = default, CancellationToken ctk = default)
        {
            return await _dbProvider.ExecuteScalarAsync(query, parameters, defaultValue, ctk);
        }

        public void ExecuteTransaction(List<DBTransactionData> transactionData)
        {
            _dbProvider.ExecuteTransaction(transactionData);
        }

        public async Task ExecuteTransactionAsync(List<DBTransactionData> transactionData, CancellationToken ctk = default)
        {
            await _dbProvider.ExecuteTransactionAsync(transactionData, ctk);
        }
    }
}