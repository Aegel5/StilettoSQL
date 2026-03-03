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

    protected Dictionary<string, StDataToDb>? namedParms;
    protected List<StDataToDb>? positionParms;
    TimeSpan? timeout;

    internal QueryBase(QueryBase initFrom) {
        namedParms = initFrom.namedParms;
        positionParms = initFrom.positionParms;
    }

    internal QueryBase() { }

    protected void Add<T>(T data) {
        positionParms ??= new();
        positionParms.Add(StGlobal.CurrentProfile.ConvertToDb(data));
    }

    protected void AddPosParms(params object[] list) {
        foreach (var item in list) {
            Add(item);
        }
    }

    ParamsForProvider CreateParms(string sql) {

        if ((namedParms?.Count ?? 0) > 0 && (positionParms?.Count ?? 0) > 0)
            throw new Exception("Positional and named arguments cannot be mixed");

        var res = new ParamsForProvider {
            sql = sql,
            timeout = timeout,
            namedParms = namedParms,
            positionParms = positionParms
        };
        return res;
    }

    private static readonly ConcurrentDictionary<string, string> _sqlCache = new();

    private static readonly Regex _regex = new(
        @"(?i)(\w+)(?:[^\w]|\b(?:LIKE|IN|NOT|IS)\b)+\?\?",
        RegexOptions.Compiled);

    protected string PreProcessQuery(string template) {

        if (!template.Contains("??")) {
            return template;
        }

        // делаем препроцессинг (пока на регулярках)

        if (_sqlCache.Count >= 5000)
            _sqlCache.Clear();

        var res = _sqlCache.GetOrAdd(template, t => {

            int count = 0;
            string resultSql = "";
            if (StGlobal.CurrentProvider.PreferPositionParms) {
                // просто меняем $$ на $1, $2 и т.д.
                resultSql = Regex.Replace(template, @"\?\?", m => {
                    count++;
                    return "$" + count; // Возвращаем $1, $2 и т.д.
                });
                if (count != positionParms?.Count) {
                    throw new Exception("Number of position params is not equals in sql");
                }
            } else {

                resultSql = _regex.Replace(t, m => {
                    count++;
                    var name = m.Groups[1].Value;
                    if (namedParms?.ContainsKey(name) != true) {
                        throw new Exception($"Not found named param for placeholder {name}");
                    }
                    return m.Value.Replace("$$", "@" + name);
                });

                // allowed mix @prm and $$
                //if (count != namedParms?.Count) {
                //    throw new Exception("Number of named params is not equals in sql");
                //}
            }

            return resultSql;

        });

        return res;
    }

    protected void Add<T>(string fieldName, T data) {
        namedParms ??= new();
        namedParms.Add(fieldName, StGlobal.CurrentProfile.ConvertToDb(data));
    }



    //public QueryBase Timeout(TimeSpan timeout) {
    //    this.timeout = timeout;
    //    return this;
    //}

    protected Task<int> ExecuteNonQuery(string sql) {
        return StGlobal.CurrentProvider.ExecuteNonQuery(CreateParms(sql));
    }

    protected Task<object?> ExecuteScalar(string sql) {
        return StGlobal.CurrentProvider.ExecuteScalar(CreateParms(sql));
    }

    protected IAsyncEnumerable<DbDataReader> ExecuteReader(string sql) {
        return StGlobal.CurrentProvider.ExecuteReader(CreateParms(sql));
    }


}
