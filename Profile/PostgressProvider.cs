using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace StilettoSQL.Profile; 

internal class PostgressProvider : IDbProvider {
    public StProviderType Type => StProviderType.Postgress;
    public bool PreferPositionParms => true;

    NpgsqlConnection NewConnection() {
        var res = new NpgsqlConnection(StGlobal.CurrentProfile.ConnectionString);
        return res;
    }
    NpgsqlCommand NewCommand(NpgsqlConnection con, ParamsForProvider parms) {
        NpgsqlCommand cmd = new(parms.sql, con);
        if (parms.timeout != null) {
            cmd.CommandTimeout = (int)parms.timeout.Value.TotalSeconds;
        }
        if (parms.positionParms != null) {
            foreach (var item in parms.positionParms) {
                var p = cmd.CreateParameter();
                // Приводим к NpgsqlParameter, чтобы получить доступ к специфическим полям Postgres
                if (p is NpgsqlParameter npgsqlParam) {
                    if (item.dbType == StDataToDb.DbType.Json) {
                        // Используем Json или Jsonb (Jsonb быстрее и современнее)
                        npgsqlParam.NpgsqlDbType = NpgsqlDbType.Json;
                    }
                }
                p.Value = item.data ?? DBNull.Value;
                cmd.Parameters.Add(p);
            }

        }
        return cmd;
    }
    public async IAsyncEnumerable<DbDataReader> ExecuteReader(ParamsForProvider parms) {
        using var con = NewConnection();
        using var cmd = NewCommand(con, parms);
        await con.OpenAsync();
        using var rdr = await cmd.ExecuteReaderAsync();
        while (await rdr.ReadAsync()) {
            yield return rdr;
        }
    }
    async public Task<object?> ExecuteScalar(ParamsForProvider parms) {
        using var con = NewConnection();
        using var cmd = NewCommand(con, parms);
        await con.OpenAsync();
        return await cmd.ExecuteScalarAsync();
    }
    async public Task<int> ExecuteNonQuery(ParamsForProvider parms) {
        using var con = NewConnection();
        using var cmd = NewCommand(con, parms);
        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync();
    }
}
