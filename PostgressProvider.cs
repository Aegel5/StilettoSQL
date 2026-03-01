using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL; 
public class PostgressProvider : IDbProvider {
    string connString;
    public PostgressProvider(string conn) {
        connString = conn;
    }
    NpgsqlConnection NewConnection() => new NpgsqlConnection(connString);
    NpgsqlCommand NewCommand(NpgsqlConnection con, string sql, Dictionary<string, DataToDb> parms) {
        NpgsqlCommand cmd = new(sql, con);
        foreach (var item in parms) {
            if (item.Value == null) {
                cmd.Parameters.AddWithValue("@" + item.Key, DBNull.Value);
            } else {
                if (item.Value.customType == DataToDb.CustomType.Json) {
                    cmd.Parameters.AddWithValue("@" + item.Key, NpgsqlDbType.Json, item.Value.data);
                } else {
                    cmd.Parameters.AddWithValue("@" + item.Key, item.Value.data);
                }
            }
        }
        return cmd;
    }
    async public Task<DbDataReader> ExecuteReader(string sql, Dictionary<string, DataToDb> parms) {
        using var con = NewConnection();
        using var cmd = NewCommand(con, sql, parms);
        var res = await cmd.ExecuteReaderAsync();
        return res;
    }
    public Task<object?> ExecuteScalar(string sql, Dictionary<string, DataToDb> parms) {
        using var con = NewConnection();
        using var cmd = NewCommand(con, sql, parms);
        return cmd.ExecuteScalarAsync();
    }
    public Task<int> ExecuteNonQuery(string sql, Dictionary<string, DataToDb> parms) {
        using var con = NewConnection();
        using var cmd = NewCommand(con, sql, parms);
        return cmd.ExecuteNonQueryAsync();
    }
}
