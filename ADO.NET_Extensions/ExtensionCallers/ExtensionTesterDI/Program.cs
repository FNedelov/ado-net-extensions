using ExtensionTesterDI.Classes;
using ExtensionTesterDI.Extensions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

internal class Program
{
    #region Properties
    private static readonly List<DbParameter> s_genericSqlParams = new()
    {
        new MySqlParameter("@queryParam1", 5),
        new MySqlParameter("@queryParam2", "param2Value")
    };
    //private static readonly List<DbParameter> s_msSqlParams = new()
    //{
    //    new SqlParameter("@queryParam1", 5),
    //    new SqlParameter("@queryParam2", "param2Value")
    //};
    private static readonly List<DBTransactionData> s_genericSqlTransactionData = new()
    {
        new DBTransactionData()
        {
            Query = "query1",
            Parameters = s_genericSqlParams
        },
        new DBTransactionData()
        {
            Query = "query2",
            Parameters = s_genericSqlParams
        }
    };
    //private static readonly List<DBTransactionData> s_msSqlTransactionData = new()
    //{
    //    new DBTransactionData()
    //    {
    //        Query = "query1",
    //        Parameters = s_msSqlParams
    //    },
    //    new DBTransactionData()
    //    {
    //        Query = "query2",
    //        Parameters = s_msSqlParams
    //    }
    //};
    private static readonly CancellationTokenSource s_tksrc = new();
    private static readonly CancellationToken s_ctk = s_tksrc.Token;
    private static string s_query = "query";
    #endregion

    //private static readonly MySQLProvider sqlProvider = new(new MySqlConnection("MySQLConnString"));
    private static readonly MsSQLProvider sqlProvider = new(new SqlConnection("SQLConnString"));

    private static async Task Main(string[] args)
    {
        SyncExtensions();
        await AsyncTaskExtensions();

    }

    static void SyncExtensions()
    {
        // ExecuteNonQuery
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            dbCaller.ExecuteNonQuery(s_query, s_genericSqlParams);
        }

        // ExecuteTransactionAsync
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            dbCaller.ExecuteTransaction(s_genericSqlTransactionData);
        }

        // ExecuteQueryAdapter
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            DataTable dt = dbCaller.ExecuteQueryAdapter(s_query, s_genericSqlParams);
        }

        // ExecuteQueryReader            
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            using DbDataReader reader = dbCaller.ExecuteQueryReader(s_query, s_genericSqlParams);
            while (reader.Read())
            {
                Console.WriteLine(reader.ReadField<string>("stringField"));
                Console.WriteLine(reader.ReadField<int>("intField"));
            }
        }

        // ExecuteScalar
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            int value = dbCaller.ExecuteScalar<int>(s_query, s_genericSqlParams);
        }
    }

    static async Task AsyncTaskExtensions()
    {
        // ExecuteNonQuery
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            await dbCaller.ExecuteNonQueryAsync(s_query, s_genericSqlParams, s_ctk);
        }

        // ExecuteTransactionAsync
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            await dbCaller.ExecuteTransactionAsync(s_genericSqlTransactionData, s_ctk);
        }

        // ExecuteQueryAdapter
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            DataTable dt = await dbCaller.ExecuteQueryAdapterAsync(s_query, s_genericSqlParams, s_ctk);
        }

        // ExecuteQueryReader            
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            using DbDataReader reader = await dbCaller.ExecuteQueryReaderAsync(s_query, s_genericSqlParams, s_ctk);
            while (reader.Read())
            {
                Console.WriteLine(await reader.ReadFieldAsync<string>("stringField", default, s_ctk));
                Console.WriteLine(await reader.ReadFieldAsync<int>("intField"));
            }
        }

        // ExecuteScalar
        using (DatabaseCaller dbCaller = new(sqlProvider))
        {
            int value = await dbCaller.ExecuteScalarAsync<int>(s_query, s_genericSqlParams, default, s_ctk);
        }
    }
}