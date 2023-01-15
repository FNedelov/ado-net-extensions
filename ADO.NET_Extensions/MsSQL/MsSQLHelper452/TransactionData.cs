using System.Collections.Generic;
using System.Data.SqlClient;

namespace MsSQLHelper452
{
    /// <summary>
    /// Class used in transactions
    /// </summary>
    public class TransactionData
    {
        /// <summary>
        /// MySQL query
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// MySQL uery parameters
        /// </summary>
        public List<SqlParameter> Parameters { get; set; }
    }
}