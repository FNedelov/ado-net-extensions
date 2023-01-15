using System.Data.Common;

namespace ExtensionTesterDI.Classes
{
    public class DBTransactionData
    {
        /// <summary>
        /// MySQL query
        /// </summary>
        public string? Query { get; set; }
        /// <summary>
        /// MySQL uery parameters
        /// </summary>
        public List<DbParameter>? Parameters { get; set; }
    }
}