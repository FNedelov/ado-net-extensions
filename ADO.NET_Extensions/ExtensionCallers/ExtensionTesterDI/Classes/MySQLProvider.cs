using ExtensionTesterDI.Extensions;
using ExtensionTesterDI.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace ExtensionTesterDI.Classes
{
    public class MySQLProvider : IExtensionsCaller
    {
        private MySqlConnection _mySQLConn { get; set; }
        public DbConnection DBConn 
        { 
            get => _mySQLConn; 
            set => _mySQLConn = (MySqlConnection)value; 
        }

        public MySQLProvider(MySqlConnection mysqlConn)
        {
            _mySQLConn = mysqlConn;
        }

        public bool ExecuteNonQuery(string query, List<DbParameter>? parameters = null)
        {
            return _mySQLConn.ExecuteNonQuery(query, Utils.ConvertToGenericSqlParams<MySqlParameter>(parameters));
        }

        public async Task<bool> ExecuteNonQueryAsync(string query, List<DbParameter>? parameters = null, CancellationToken ctk = default)
        {
            return await _mySQLConn.ExecuteNonQueryAsync(query, Utils.ConvertToGenericSqlParams<MySqlParameter>(parameters), ctk);
        }

        public DataTable ExecuteQueryAdapter(string query, List<DbParameter>? parameters = null)
        {
            return _mySQLConn.ExecuteQueryAdapter(query, Utils.ConvertToGenericSqlParams<MySqlParameter>(parameters));
        }

        public async Task<DataTable> ExecuteQueryAdapterAsync(string query, List<DbParameter>? parameters = null, CancellationToken ctk = default)
        {
            return await _mySQLConn.ExecuteQueryAdapterAsync(query, Utils.ConvertToGenericSqlParams<MySqlParameter>(parameters), ctk);
        }

        public DbDataReader ExecuteQueryReader(string query, List<DbParameter>? parameters = null)
        {
            return _mySQLConn.ExecuteQueryReader(query, Utils.ConvertToGenericSqlParams<MySqlParameter>(parameters));
        }

        public async Task<DbDataReader> ExecuteQueryReaderAsync(string query, List<DbParameter>? parameters = null, CancellationToken ctk = default)
        {
            return await _mySQLConn.ExecuteQueryReaderAsync(query, Utils.ConvertToGenericSqlParams<MySqlParameter>(parameters), ctk);
        }

        public T? ExecuteScalar<T>(string query, List<DbParameter>? parameters = null, T? defaultValue = default)
        {
            return _mySQLConn.ExecuteScalar(query, Utils.ConvertToGenericSqlParams<MySqlParameter>(parameters), defaultValue);
        }

        public async Task<T?> ExecuteScalarAsync<T>(string query, List<DbParameter>? parameters = null, T? defaultValue = default, CancellationToken ctk = default)
        {
            return await _mySQLConn.ExecuteScalarAsync(query, Utils.ConvertToGenericSqlParams<MySqlParameter>(parameters), defaultValue, ctk);
        }

        public void ExecuteTransaction(List<DBTransactionData> transactionData)
        {
            _mySQLConn.ExecuteTransaction(transactionData);
        }

        public async Task ExecuteTransactionAsync(List<DBTransactionData> transactionData, CancellationToken ctk = default)
        {
            await _mySQLConn.ExecuteTransactionAsync(transactionData, ctk);
        }
    }
}