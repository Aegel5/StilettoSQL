using StilettoSQL.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL.Query; 

public class QueryBuilder : QueryBase {

    protected StringBuilder sb = new();
    bool whereAdded = false;
    bool setAdded = false;

    public QueryBuilder() { }

    public QueryBuilder(string sql, params object[] list) {
        Append(sql, list);
    }

    public QueryBuilder Append(string sql, params object[] list) {
        if (sql.Length == 0) throw new Exception("empty sql");
        if (sb.Length > 0 && sb[^1] != ' ' && sql[0] != ' ')
            sb.Append(' ');
        sb.Append(sql);
        AddPosParms(list);
        return this;
    }

    public QueryBuilder Append(QueryBuilder q) {
        ConsumeParmsFrom(q);
        Append(q.ToString());
        return this;
    }

    public QueryBuilder WhereAndEq(string fieldName, object data) {
        return WhereAnd($"{fieldName}=??", data);
    }

    public QueryBuilder WhereAnd(string sql, params object[] list) {

        if (!whereAdded) {
            Append("WHERE ");
            whereAdded = true;
        } else {
            Append("AND ");
        }

        sb.Append('(');
        Append(sql, list);
        sb.Append(')');

        return this;
    }

    public QueryBuilder SetValue<T>(string fieldName, T data) {

        if (!setAdded) {
            setAdded = true;
            sb.Append(" SET ");
        } else {
            sb.Append(", ");
        }

        sb.Append(fieldName);
        sb.Append("=??");
        AddPosParm(data);

        return this;
    }

    public Query ToQuery() {
        return new Query(sb.ToString(), this);
    }

    public override string ToString() {
        return sb.ToString();
    }

    public IAsyncEnumerable<DbDataReader> ReadAllRows() {
        return ToQuery().ReadAllRows();
    }

    public Task<int> Exec_GetRowsTouched() {
        return ToQuery().Exec_GetRowsTouched();
    }

}
