using ExtensionTesterDI.Extensions;
using ExtensionTesterDI.Interfaces;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ExtensionTesterDI.Classes
{
    internal class MsSQLProvider : IExtensionsCaller
    {
        private SqlConnection _msSQLConn { get; set; }
        public DbConnection DBConn 
        { 
            get => _msSQLConn; 
            set => _msSQLConn = (SqlConnection)value; 
        }

        public MsSQLProvider(SqlConnection msSqlConn)
        {
            _msSQLConn = msSqlConn;
        }

        public bool ExecuteNonQuery(string query, List<DbParameter>? parameters = null)
        {
            return _msSQLConn.ExecuteNonQuery(query, Utils.ConvertToGenericSqlParams<SqlParameter>(parameters));
        }

        public async Task<bool> ExecuteNonQueryAsync(string query, List<DbParameter>? parameters = null, CancellationToken ctk = default)
        {
            return await _msSQLConn.ExecuteNonQueryAsync(query, Utils.ConvertToGenericSqlParams<SqlParameter>(parameters), ctk);
        }

        public DataTable ExecuteQueryAdapter(string query, List<DbParameter>? parameters = null)
        {
            return _msSQLConn.ExecuteQueryAdapter(query, Utils.ConvertToGenericSqlParams<SqlParameter>(parameters));
        }

        public async Task<DataTable> ExecuteQueryAdapterAsync(string query, List<DbParameter>? parameters = null, CancellationToken ctk = default)
        {
            return await _msSQLConn.ExecuteQueryAdapterAsync(query, Utils.ConvertToGenericSqlParams<SqlParameter>(parameters), ctk);
        }

        public DbDataReader ExecuteQueryReader(string query, List<DbParameter>? parameters = null)
        {
            return _msSQLConn.ExecuteQueryReader(query, Utils.ConvertToGenericSqlParams<SqlParameter>(parameters));
        }

        public async Task<DbDataReader> ExecuteQueryReaderAsync(string query, List<DbParameter>? parameters = null, CancellationToken ctk = default)
        {
            return await _msSQLConn.ExecuteQueryReaderAsync(query, Utils.ConvertToGenericSqlParams<SqlParameter>(parameters), ctk);
        }

        public T? ExecuteScalar<T>(string query, List<DbParameter>? parameters = null, T? defaultValue = default)
        {
            return _msSQLConn.ExecuteScalar(query, Utils.ConvertToGenericSqlParams<SqlParameter>(parameters), defaultValue);
        }

        public async Task<T?> ExecuteScalarAsync<T>(string query, List<DbParameter>? parameters = null, T? defaultValue = default, CancellationToken ctk = default)
        {
            return await _msSQLConn.ExecuteScalarAsync(query, Utils.ConvertToGenericSqlParams<SqlParameter>(parameters), defaultValue, ctk);
        }

        public void ExecuteTransaction(List<DBTransactionData> transactionData)
        {
            _msSQLConn.ExecuteTransaction(transactionData);
        }

        public async Task ExecuteTransactionAsync(List<DBTransactionData> transactionData, CancellationToken ctk = default)
        {
            await _msSQLConn.ExecuteTransactionAsync(transactionData, ctk);
        }
    }
}