using Npgsql;
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

namespace StilettoSQL.Details;

public class QueryBase {

    protected Dictionary<string, object>? parms;
    TimeSpan? timeout;

    ParamsForProvider CreateParms(string sql) {
        var res = new ParamsForProvider {
            sql = sql,
            timeout = timeout
        };
        if (parms != null) {
            res = res with { fields = Global.CurrentProfile.ConvertToDb(parms) };
        }
        return res;
    }

    // Кэш: Ключ — ваш шаблон с $$, Значение — готовый SQL с @параметрами
    private static readonly ConcurrentDictionary<string, string> _sqlCache = new();

    // Скомпилированная регулярка (живет один раз)
    private static readonly Regex _regex = new(
        @"(?i)(\w+)(?:[^\w]|\b(?:LIKE|IN|NOT|IS)\b)+\$\$",
        RegexOptions.Compiled);

    protected string PreProcessQuery(string template) {

        if (!template.Contains("$$")) return template;

        if (_sqlCache.Count >= 5000)
            _sqlCache.Clear();

        var res = _sqlCache.GetOrAdd(template, t => {
            return _regex.Replace(t, m =>
                m.Value.Replace("$$", "@" + m.Groups[1].Value));
        });

        return res;
    }


    public QueryBase Add(string fieldName, object data) {
        if (parms == null) parms = new();
        parms.Add(fieldName, data);
        return this;
    }

    public QueryBase Timeout(TimeSpan timeout) {
        this.timeout = timeout;
        return this;
    }

    protected Task<int> ExecuteNonQuery(string sql) {
        return Global.CurrentProvider.ExecuteNonQuery(CreateParms(sql));
    }

    protected Task<object?> ExecuteScalar(string sql) {
        return Global.CurrentProvider.ExecuteScalar(CreateParms(sql));
    }

    protected Task<DbDataReader> ExecuteReader(string sql) {
        return Global.CurrentProvider.ExecuteReader(CreateParms(sql));
    }


}
