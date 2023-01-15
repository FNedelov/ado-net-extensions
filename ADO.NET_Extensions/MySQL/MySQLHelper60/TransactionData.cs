using MySql.Data.MySqlClient;

namespace MySQLHelper60
{
    /// <summary>
    /// Class used in transactions
    /// </summary>
    public class TransactionData
    {
        /// <summary>
        /// MySQL query
        /// </summary>
        public string? Query { get; set; }
        /// <summary>
        /// MySQL uery parameters
        /// </summary>
        public List<MySqlParameter>? Parameters { get; set; }
    }
}