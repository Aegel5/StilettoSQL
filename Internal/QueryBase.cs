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

        if (positionParms != null) {
            sql = PreprocessSQL.ReplacePlaceholders(sql);
        }

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




}
