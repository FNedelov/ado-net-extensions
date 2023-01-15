using System.Data;
using ExtensionTesterDI.Classes;
using System.Data.Common;

namespace ExtensionTesterDI.Interfaces
{
    public interface IExtensionsCaller
    {
        public DbConnection DBConn { get; set; }

        #region Async methods
        Task<bool> ExecuteNonQueryAsync(string query, List<DbParameter>? parameters = default, CancellationToken ctk = default);

        Task ExecuteTransactionAsync(List<DBTransactionData> transactionData, CancellationToken ctk = default);

        Task<DataTable> ExecuteQueryAdapterAsync(string query, List<DbParameter>? parameters = default, CancellationToken ctk = default);

        Task<DbDataReader> ExecuteQueryReaderAsync(string query, List<DbParameter>? parameters = default, CancellationToken ctk = default);

        Task<T?> ExecuteScalarAsync<T>(string query, List<DbParameter>? parameters = default, T? defaultValue = default, CancellationToken ctk = default);
        #endregion

        #region Sync methods
        bool ExecuteNonQuery(string query, List<DbParameter>? parameters = default);

        void ExecuteTransaction(List<DBTransactionData> transactionData);

        DataTable ExecuteQueryAdapter(string query, List<DbParameter>? parameters = default);

        DbDataReader ExecuteQueryReader(string query, List<DbParameter>? parameters = default);

        T? ExecuteScalar<T>(string query, List<DbParameter>? parameters = default, T? defaultValue = default);
        #endregion
    }
}