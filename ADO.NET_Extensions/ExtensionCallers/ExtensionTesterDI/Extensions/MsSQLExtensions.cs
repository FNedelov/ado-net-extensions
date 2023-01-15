using ExtensionTesterDI.Classes;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ExtensionTesterDI.Extensions
{
    public static class MsSQLExtensions
    {
        #region Async methods
        /// <summary>
        /// Asynchronously executes a non query.
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="conn">Connection to use.</param>
        /// <param name="parameters">Parameters, if there are any.</param>
        /// <param name="ctk">Cancellation token.</param>
        /// <returns>Returns whether any rows were changed.</returns>
        public static async Task<bool> ExecuteNonQueryAsync(this SqlConnection conn, string query, List<SqlParameter>? parameters = default, CancellationToken ctk = default)
        {
            using SqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            return await cmd.ExecuteNonQueryAsync(ctk) > -1;
        }

        /// <summary>
        /// Asynchronously executes one or multiple queries using transaction.
        /// </summary>
        /// <param name="transactionData">Queries with their parameters.</param>
        /// <param name="conn">Connection to use.</param>
        /// /// <param name="ctk">Cancellation token.</param>
        /// <returns></returns>
        public static async Task ExecuteTransactionAsync(this SqlConnection conn, List<DBTransactionData> transactionData, CancellationToken ctk = default)
        {
            SqlTransaction transaction = conn.BeginTransaction();
            try
            {
                transactionData.ForEach(async x => await conn.ExecuteNonQueryAsync(x.Query, Utils.ConvertToGenericSqlParams<SqlParameter>(x.Parameters), ctk));
                await transaction.CommitAsync(ctk);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ctk);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously fills and returns datatable using an adapter.
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="conn">Connection to use.</param>
        /// <param name="parameters">Parameters, if there are any.</param>
        /// <param name="ctk">Cancellation token.</param>
        /// <returns>Result datatable.</returns>
        public static async Task<DataTable> ExecuteQueryAdapterAsync(this SqlConnection conn, string query, List<SqlParameter>? parameters = default, CancellationToken ctk = default)
        {
            DataTable dataTable = new();
            await Task.Delay(1, ctk);
            using SqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            SqlDataAdapter adapter = new(cmd);
            adapter.Fill(dataTable);
            return dataTable;
        }

        /// <summary>
        /// Asynchronously returns a data reader that can be used to read from.
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="conn">Connection to use.</param>
        /// <param name="parameters">Parameters, if there are any.</param>
        /// <param name="ctk">Cancellation token.</param>
        /// <returns>MySqlDataReader.</returns>
        public static async Task<SqlDataReader> ExecuteQueryReaderAsync(this SqlConnection conn, string query, List<SqlParameter>? parameters = default, CancellationToken ctk = default)
        {
            return await GenerateMySqlCommand(query, conn, parameters).ExecuteReaderAsync(ctk);
        }

        /// <summary>
        /// Asynchronously executes scalar (returns only the first field of the first row).
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="conn">Connection to use.</param>
        /// <param name="parameters">Parameters, if there are any.</param>
        /// <param name="defaultValue">Return value if column value is null.</param>
        /// <param name="ctk">Cancellation token.</param>
        /// <returns>Value of first row's first field. If there are no results, returns default.</returns>
        public static async Task<T?> ExecuteScalarAsync<T>(this SqlConnection conn, string query, List<SqlParameter>? parameters = default, T? defaultValue = default, CancellationToken ctk = default)
        {
            using SqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            object? result = await cmd.ExecuteScalarAsync(ctk);
            return result == null ? defaultValue : (T)result;
        }
        #endregion

        #region Sync methods
        /// <summary>
        /// Synchronously executes a non query.
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="conn">Connection to use.</param>
        /// <param name="parameters">Parameters, if there are any.</param>
        /// <returns>Returns whether any rows were changed.</returns>
        public static bool ExecuteNonQuery(this SqlConnection conn, string query, List<SqlParameter>? parameters = default)
        {
            using SqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            return cmd.ExecuteNonQuery() > -1;
        }

        /// <summary>
        /// Synchronously executes one or multiple queries using transaction.
        /// </summary>
        /// <param name="transactionData">Queries with their parameters.</param>
        /// <param name="conn">Connection to use.</param>
        /// <returns></returns>
        public static void ExecuteTransaction(this SqlConnection conn, List<DBTransactionData> transactionData)
        {
            IDbTransaction transaction = conn.BeginTransaction();
            try
            {
                transactionData.ForEach(x => conn.ExecuteNonQuery(x.Query, Utils.ConvertToGenericSqlParams<SqlParameter>(x.Parameters)));
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Synchronously fills and returns a datatable using an adapter.
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="conn">Connection to use.</param>
        /// <param name="parameters">Parameters, if there are any.</param>
        /// <returns>Result datatable.</returns>
        public static DataTable ExecuteQueryAdapter(this SqlConnection conn, string query, List<SqlParameter>? parameters = default)
        {
            DataTable dataTable = new();

            using SqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            SqlDataAdapter adapter = new(cmd);
            adapter.Fill(dataTable);
            return dataTable;
        }

        /// <summary>
        /// Synchronously returns a data reader that can be used to read from.
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="conn">Connection to use.</param>
        /// <param name="parameters">Parameters, if there are any.</param>
        /// <returns>MySqlDataReader.</returns>
        public static SqlDataReader ExecuteQueryReader(this SqlConnection conn, string query, List<SqlParameter>? parameters = default)
        {
            return GenerateMySqlCommand(query, conn, parameters).ExecuteReader();
        }

        /// <summary>
        /// Synchronously executes scalar (returns only the first field of the first row).
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="conn">Connection to use.</param>
        /// <param name="parameters">Parameters, if there are any.</param>
        /// <param name="defaultValue">Return value if column value is null.</param>
        /// <returns>Value of first row's first field. If there are no results, returns default.</returns>
        public static T? ExecuteScalar<T>(this SqlConnection conn, string query, List<SqlParameter>? parameters = default, T? defaultValue = default)
        {
            using SqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            object result = cmd.ExecuteScalar();
            return result == null ? defaultValue : (T)result;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Generates the sql command.
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="connection">Connection to use.</param>
        /// <param name="parameters">List of query parameters, if there are any.</param>
        /// <returns>Generated MySQLCommand.</returns>
        private static SqlCommand GenerateMySqlCommand(string query, SqlConnection connection, List<SqlParameter>? parameters)
        {
            SqlCommand cmd = new(query, connection);

            if (parameters != default && parameters.Count > 0) cmd.Parameters.AddRange(parameters.ToArray());
            return cmd;
        }
        #endregion
    }
}