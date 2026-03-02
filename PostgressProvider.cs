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
internal class PostgressProvider : IDbProvider {
    async Task<NpgsqlConnection> NewConnection() {
        var res = new NpgsqlConnection(Global.CurrentProfile.ConnectionString);
        await res.OpenAsync();
        return res;
    }
    NpgsqlCommand NewCommand(NpgsqlConnection con, ParamsForProvider parms) {
        NpgsqlCommand cmd = new(parms.sql, con);
        if (parms.timeout != null) {
            cmd.CommandTimeout = (int)parms.timeout.Value.TotalSeconds;
        }
        if (parms.fields != null) {
            foreach (var item in parms.fields) {
                if (item.Value == null) {
                    cmd.Parameters.AddWithValue("@" + item.Key, DBNull.Value);
                } else {
                    if (item.Value.dbType == DataToDb.DbType.Json) {
                        cmd.Parameters.AddWithValue("@" + item.Key, NpgsqlDbType.Json, item.Value.data);
                    } else {
                        cmd.Parameters.AddWithValue("@" + item.Key, item.Value.data);
                    }
                }
            }
        }
        return cmd;
    }
    public async Task<DbDataReader> ExecuteReader(ParamsForProvider parms) {
        using var con = await NewConnection();
        using var cmd = NewCommand(con, parms);
        var res = await cmd.ExecuteReaderAsync();
        return res;
    }
    async public Task<object?> ExecuteScalar(ParamsForProvider parms) {
        using var con = await NewConnection();
        using var cmd = NewCommand(con, parms);
        return await cmd.ExecuteScalarAsync();
    }
    async public Task<int> ExecuteNonQuery(ParamsForProvider parms) {
        using var con = await NewConnection();
        using var cmd = NewCommand(con, parms);
        return await cmd.ExecuteNonQueryAsync();
    }
}
