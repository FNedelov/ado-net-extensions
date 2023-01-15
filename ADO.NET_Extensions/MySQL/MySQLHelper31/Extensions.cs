using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Threading;

namespace MySQLHelper31
{
    /// <summary>
    /// Created by: Ferenc Nedelov
    /// For further information refer to README.txt
    /// </summary>
    public static class Extensions
    {
        // Async methods use nuget package, which in turn requires internet connection to download!!!
        // If you have no internet connection, manually reference the necessary DLLs as found in "References" folder (folder 8028)!!!
        #region Async methods
        /// <summary>
        /// Asynchronously executes a non query.
        /// </summary>
        /// <param name="query">Query to use.</param>
        /// <param name="conn">Connection to use.</param>
        /// <param name="parameters">Parameters, if there are any.</param>
        /// <param name="ctk">Cancellation token.</param>
        /// <returns>Returns whether any rows were changed.</returns>
        public static async Task<bool> ExecuteNonQueryAsync(this string query, MySqlConnection conn, List<MySqlParameter> parameters = default, CancellationToken ctk = default)
        {
            using MySqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            return await cmd.ExecuteNonQueryAsync(ctk) > -1;
        }

        /// <summary>
        /// Asynchronously executes one or multiple queries using transaction.
        /// </summary>
        /// <param name="transactionData">Queries with their parameters.</param>
        /// <param name="conn">Connection to use.</param>
        /// /// <param name="ctk">Cancellation token.</param>
        /// <returns></returns>
        public static async Task ExecuteTransactionAsync(this List<TransactionData> transactionData, MySqlConnection conn, CancellationToken ctk = default)
        {
            MySqlTransaction transaction = await conn.BeginTransactionAsync(ctk);
            try
            {
                transactionData.ForEach(async x => await x.Query.ExecuteNonQueryAsync(conn, x.Parameters, ctk));
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
        public static async Task<DataTable> ExecuteQueryAdapterAsync(this string query, MySqlConnection conn, List<MySqlParameter> parameters = default, CancellationToken ctk = default)
        {
            var dataTable = new DataTable();

            using MySqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            var adapter = new MySqlDataAdapter(cmd);
            await adapter.FillAsync(dataTable, ctk);
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
        public static async Task<MySqlDataReader> ExecuteQueryReaderAsync(this string query, MySqlConnection conn, List<MySqlParameter> parameters = default, CancellationToken ctk = default)
        {
            return (MySqlDataReader)await GenerateMySqlCommand(query, conn, parameters).ExecuteReaderAsync(ctk);
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
        public static async Task<T> ExecuteScalarAsync<T>(this string query, MySqlConnection conn, List<MySqlParameter> parameters = default, T defaultValue = default, CancellationToken ctk = default)
        {
            using MySqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            object result = await cmd.ExecuteScalarAsync(ctk);
            return result == null ? defaultValue : (T)result;
        }

        /// <summary>
        /// Enables asynchronous data reading from a specific field.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="reader">Reader to use.</param>
        /// <param name="fieldName">Field to query.</param>
        /// <param name="defaultValue">Return value if column value is null.</param>
        /// <param name="ctk">Cancellation token.</param>
        /// <returns>Value of field.</returns>
        public static async Task<T> ReadFieldAsync<T>(this MySqlDataReader reader, string fieldName, T defaultValue = default, CancellationToken ctk = default)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader), "Reader cannot be null.");
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException("Field name cannot be null or empty.", nameof(fieldName));

            int idx = reader.GetOrdinal(fieldName);
            bool isDBNull = await reader.IsDBNullAsync(idx, ctk);
            return isDBNull ? defaultValue : await reader.GetFieldValueAsync<T>(idx, ctk);
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
        public static bool ExecuteNonQuery(this string query, MySqlConnection conn, List<MySqlParameter> parameters = default)
        {
            using MySqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            return cmd.ExecuteNonQuery() > -1;
        }

        /// <summary>
        /// Synchronously executes one or multiple queries using transaction.
        /// </summary>
        /// <param name="transactionData">Queries with their parameters.</param>
        /// <param name="conn">Connection to use.</param>
        /// <returns></returns>
        public static void ExecuteTransaction(this List<TransactionData> transactionData, MySqlConnection conn)
        {
            IDbTransaction transaction = conn.BeginTransaction();
            try
            {
                transactionData.ForEach(x => x.Query.ExecuteNonQuery(conn, x.Parameters));
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
        public static DataTable ExecuteQueryAdapter(this string query, MySqlConnection conn, List<MySqlParameter> parameters = default)
        {
            var dataTable = new DataTable();

            using MySqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            var adapter = new MySqlDataAdapter(cmd);
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
        public static MySqlDataReader ExecuteQueryReader(this string query, MySqlConnection conn, List<MySqlParameter> parameters = default)
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
        public static T ExecuteScalar<T>(this string query, MySqlConnection conn, List<MySqlParameter> parameters = default, T defaultValue = default)
        {
            using MySqlCommand cmd = GenerateMySqlCommand(query, conn, parameters);
            object result = cmd.ExecuteScalar();
            return result == null ? defaultValue : (T)result;
        }

        /// <summary>
        /// Enables synchronous data reading from a specific field.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="reader">Reader to use.</param>
        /// <param name="fieldName">Field to query.</param>
        /// <param name="defaultValue">Return value if column value is null.</param>
        /// <returns>Value of field.</returns>
        public static T ReadField<T>(this IDataReader reader, string fieldName, T defaultValue = default)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader), "Reader cannot be null.");
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException("Field name cannot be null or empty.", nameof(fieldName));

            object obj = reader[fieldName];
            return obj.IsNull() ? defaultValue : (T)obj;
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
        private static MySqlCommand GenerateMySqlCommand(string query, MySqlConnection connection, List<MySqlParameter> parameters)
        {
            var cmd = new MySqlCommand(query, connection);

            if (parameters != default && parameters.Count > 0) cmd.Parameters.AddRange(parameters.ToArray());
            return cmd;
        }

        /// <summary>
        /// Checks if object value equals null (NOT default!) || DBNull.Value.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="obj">Object to check.</param>
        /// <returns>Boolean result of expression.</returns>
        private static bool IsNull<T>(this T obj) where T : class
        {
            return (obj == null || obj == DBNull.Value);
        }
        #endregion
    }
}
