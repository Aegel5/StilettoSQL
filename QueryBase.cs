using StilettoSQL.Query;
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
    public string ReplacePlaceholders(string sql, StProfile profile) {

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
                    //if (sql[i] == '\\' && sql[i+1] == '\'') {
                    //    i += 2 ;
                    //    continue;
                    //}
                    if (sql[i] == '\'') {
                        i++;
                        break;
                    }
                }
                continue;
            }

            if (sql[i] == '?' && sql[i + 1] == '?') {
                sb ??= new(sql.Length + positionParms.Count);
                sb.Append(sql, i_from, i - i_from);
                if (profile.ProviderSQL == StProviderSQL.PostgreSQL) {
                    sb.Append('$');
                    sb.Append(++count);
                } else if (profile.ProviderSQL == StProviderSQL.MySQL) {
                    sb.Append('?');
                } else {
                    throw new NotSupportedException();
                }
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

    class ActionCtx : IDisposable {

        public DbConnection? con;
        public DbCommand? cmd;
        public bool need_dispose_con = true;

        public void Dispose() {
            if(need_dispose_con) con?.Dispose();
            cmd?.Dispose();
        }
    }

    async Task<ActionCtx> NewCommandCtx(string sql) {

        var ctx = new ActionCtx();

        try {

            var profile = StProfile.CurrentProfile;
            var transact = StProfile.CurrentTransaction_.Value;

            if (transact == null) {
                // как обычно, делаем новое подключение
                ctx.con = profile.CreateConnection();
                await ctx.con.OpenAsync();
                ctx.cmd = ctx.con.CreateCommand();
            } else {
                // коннектимся к транзакции
                ctx.need_dispose_con = false; // не владеем
                if (transact.Connection != null) {
                    ctx.con = transact.Connection;
                } else {
                    // первое подключение
                    ctx.con = transact.Connection = profile.CreateConnection();
                    await ctx.con.OpenAsync();
                    transact.transact = await ctx.con.BeginTransactionAsync();
                }
                ctx.cmd = ctx.con.CreateCommand();
                ctx.cmd.Transaction = transact.transact;
            }

            var cmd = ctx.cmd;

            cmd.CommandText = ReplacePlaceholders(sql, profile);
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

            return ctx;

        } catch (Exception) {
            ctx.Dispose(); // чистка
            throw;
        }
    }
    protected async IAsyncEnumerable<DbDataReader> ExecuteReader(string sql) {
        using var ctx = await NewCommandCtx(sql);
        using var rdr = await ctx.cmd.ExecuteReaderAsync();
        while (await rdr.ReadAsync()) {
            yield return rdr;
        }
    }
    async protected Task<object?> ExecuteScalar(string sql) {
        using var ctx = await NewCommandCtx(sql);
        return ctx.cmd.ExecuteScalar();
    }
    async protected Task<T?> ExecuteScalar<T>(string sql) {
        using var ctx = await NewCommandCtx(sql);
        // Use reader for auto-conversion.
        using var rdr = await ctx.cmd.ExecuteReaderAsync();
        if (!await rdr.ReadAsync()) return StProfile.ConvertToNull<T>();
        return StProfile.CurrentProfile.ConvertFromDb<T>(rdr, 0);
    }

    async protected Task<int> ExecuteNonQuery(string sql) {
        using var ctx = await NewCommandCtx(sql);
        return await ctx.cmd.ExecuteNonQueryAsync();
    }



}
