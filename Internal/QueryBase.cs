using Npgsql;
using StilettoSQL.Profile;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StilettoSQL.Internal;

public class QueryBase {

    internal List<StDataToDb>? positionParms;
    protected TimeSpan? _Timeout { get; set; }

    internal QueryBase(QueryBase initFrom) {
        positionParms = initFrom.positionParms;
    }

    internal QueryBase() { }

    protected void ConsumeParmsFrom(QueryBase other) {
        if (other.positionParms == null) return;
        positionParms ??= new();
        positionParms.AddRange(other.positionParms);
        other.positionParms = null;
    }

    protected StDataToDb ConvertToDb(object? obj) {
        return StGlobal.CurrentProfile.ConvertToDb(obj);
    }

    protected void AddPosParm<T>(T data) {
        positionParms ??= new();
        positionParms.Add(ConvertToDb(data));
    }

    protected void AddPosParms(params object[] list) {
        foreach (var item in list) {
            AddPosParm(item);
        }
    }

    ParamsForProvider CreateProviderParms(string sql) {

        sql = ReplacePlaceholders(sql);

        var res = new ParamsForProvider {
            sql = sql,
            timeout = _Timeout,
            positionParms = positionParms
        };
        return res;
    }

    protected Task<int> ExecuteNonQuery(string sql) {
        return StGlobal.CurrentProvider.ExecuteNonQuery(CreateProviderParms(sql));
    }

    protected Task<object?> ExecuteScalar(string sql) {
        return StGlobal.CurrentProvider.ExecuteScalar(CreateProviderParms(sql));
    }

    async protected Task<T?> ExecuteScalar<T>(string sql) {
        return StGlobal.CurrentProfile.ConvertFromDb<T>(await ExecuteScalar(sql));
    }

    protected IAsyncEnumerable<DbDataReader> ExecuteReader(string sql) {
        return StGlobal.CurrentProvider.ExecuteReader(CreateProviderParms(sql));
    }

    public string ReplacePlaceholders(string sql) {

        if (positionParms == null) return sql;

        StringBuilder? sb = null;
        int i_prev = 0;
        int count = 0;
        int i = 0;
        while (i < sql.Length - 1) {

            // todo: skip strings

            if (sql[i] == '?' && sql[i + 1] == '?') {
                sb ??= new(sql.Length + Math.Max(0, positionParms.Count - 8));
                sb.Append(sql, i_prev, i - i_prev);
                sb.Append('$');
                sb.Append(++count);
                i+=2;
                i_prev = i;
            } else {
                i++;
            }

        }

        if (count == 0) {
            // ok user can use native $1, $2, ...
            return sql;
        }

        sb.Append(sql, i_prev, i - i_prev + 1);

        if (count != positionParms.Count)
            throw new Exception($"Excepted {count} parms. Have: {positionParms.Count}");

        var res = sb.ToString();
        return res;
    }



}
