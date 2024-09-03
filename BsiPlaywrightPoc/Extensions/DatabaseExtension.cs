using System.Data;
using System.Data.SqlClient;

namespace BsiPlaywrightPoc.Extensions;

public static class DatabaseExtension
{
    public static DataTable? SqlExecute(this string connectionString, string sql)
    {
        try
        {
            var results = new DataTable();

            using var con = new SqlConnection(connectionString);
            con.Open();

            using var cmd = new SqlCommand(sql, con) { CommandTimeout = 90 };

            using var reader = cmd.ExecuteReader();

            if (!reader.HasRows) return null;
            {
                results.Load(reader);
            }

            return results;
        }
        catch (Exception e)
        {
            // user a logger instead
            Console.WriteLine(e);
            throw;
        }
    }
}