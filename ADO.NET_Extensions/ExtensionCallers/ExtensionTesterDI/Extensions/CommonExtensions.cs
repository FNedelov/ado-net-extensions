using System.Data.Common;

namespace ExtensionTesterDI.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Enables asynchronous data reading from a specific field.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="reader">Reader to use.</param>
        /// <param name="fieldName">Field to query.</param>
        /// <param name="defaultValue">Return value if column value is null.</param>
        /// <param name="ctk">Cancellation token.</param>
        /// <returns>Value of field.</returns>
        public static async Task<T?> ReadFieldAsync<T>(this DbDataReader reader, string fieldName, T? defaultValue = default, CancellationToken ctk = default)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader), "Reader cannot be null.");
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException("Field name cannot be null or empty.", nameof(fieldName));

            int idx = reader.GetOrdinal(fieldName);
            bool isDBNull = await reader.IsDBNullAsync(idx, ctk);
            return isDBNull ? defaultValue : await reader.GetFieldValueAsync<T>(idx, ctk);
        }

        /// <summary>
        /// Enables synchronous data reading from a specific field.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="reader">Reader to use.</param>
        /// <param name="fieldName">Field to query.</param>
        /// <param name="defaultValue">Return value if column value is null.</param>
        /// <returns>Value of field.</returns>
        public static T? ReadField<T>(this DbDataReader reader, string fieldName, T? defaultValue = default)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader), "Reader cannot be null.");
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException("Field name cannot be null or empty.", nameof(fieldName));

            object obj = reader[fieldName];
            return obj.IsNull() ? defaultValue : (T)obj;
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
    }
}