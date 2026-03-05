using StilettoSQL.Profile;
using System.Data;
using System.Data.Common;
using System.Text;

namespace StilettoSQL.Internal;

public class QueryBase {

    internal List<object?>? positionParms;
    protected TimeSpan? _Timeout { get; set; }

    protected void ConsumeParmsFrom(QueryBase other) {
        if (other.positionParms == null) return;
        positionParms ??= new();
        positionParms.AddRange(other.positionParms);
        other.positionParms = null;
    }

    protected void AddPosParm(object? data) {
        (positionParms ??= new()).Add(data);
    }

    protected void AddPosParms(params object?[] list) {
        if (list.Length == 0) return;
        (positionParms ??= new()).AddRange(list);
    }
    public string ReplacePlaceholders(string sql) {

        if (positionParms == null) return sql;

        StringBuilder? sb = null;
        int i_from = 0;
        int count = 0;
        int i = 0;
        while (i < sql.Length - 1) {

            // skip strings
            if (sql[i] == '\'') {
                i++;
                for (; i < sql.Length - 1; i++) {
                    if (sql[i] == '\'') {
                        //if (sql[i + 1] == '\'') {
                        //    i++; 
                        //    continue;
                        //}
                        //if (sql[i - 1] == '\\') {
                        //    continue;
                        //}
                        i++;
                        break;
                    }
                }
                continue;
            }

            if (sql[i] == '?' && sql[i + 1] == '?') {
                sb ??= new(sql.Length + Math.Max(0, positionParms.Count - 8));
                sb.Append(sql, i_from, i - i_from);
                sb.Append('$');
                sb.Append(++count);
                i += 2;
                i_from = i;
            } else {
                i++;
            }

        }

        if (sb == null) {
            // ok user can use native ($1, $2, ...) or (?, ?, ...)
            return sql;
        }

        sb.Append(sql, i_from, sql.Length - i_from);

        if (count != positionParms.Count)
            throw new Exception($"Excepted {count} parms. Have: {positionParms.Count}");

        var res = sb.ToString();
        return res;
    }

    DbConnection NewConnection() {
        return (StGlobal.CurrentProfile.CreateConnection 
            ?? throw new Exception("Need set CreateConnection"))();
    }
    DbCommand NewCommand(IDbConnection con, string sql) {

        var profile = StGlobal.CurrentProfile;

        IDbCommand cmd = con.CreateCommand();
        cmd.CommandText = ReplacePlaceholders(sql);
        if (_Timeout != null) {
            cmd.CommandTimeout = (int)_Timeout.Value.TotalSeconds;
        }
        if (positionParms != null) {
            foreach (var item in positionParms) {
                var p = cmd.CreateParameter();
                if (item == null) p.Value = DBNull.Value;
                else {
                    if (profile.DataConverter?.ToDb(item, p) != true) {
                        // стандартный конверт
                        p.Value = item;
                    }
                }
                cmd.Parameters.Add(p);
            }
        }
        return cmd as DbCommand ?? throw new NotSupportedException();
    }
    public async IAsyncEnumerable<DbDataReader> ExecuteReader(string sql) {
        using var con = NewConnection();
        using var cmd = NewCommand(con, sql);
        await con.OpenAsync();
        using var rdr = await cmd.ExecuteReaderAsync();
        while (await rdr.ReadAsync()) {
            yield return rdr;
        }
    }
    async public Task<object?> ExecuteScalar(string sql) {
        using var con = NewConnection();
        using var cmd = NewCommand(con, sql);
        await con.OpenAsync();
        var res = await cmd.ExecuteScalarAsync();
        return res;
    }
    async public Task<T?> ExecuteScalar<T>(string sql) {
        return StGlobal.CurrentProfile.ConvertFromDb<T>(await ExecuteScalar(sql));
    }
    async public Task<int> ExecuteNonQuery(string sql) {
        using var con = NewConnection();
        using var cmd = NewCommand(con, sql);
        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync();
    }



}
