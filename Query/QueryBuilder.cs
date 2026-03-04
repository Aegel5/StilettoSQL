using StilettoSQL.Internal;
using System.Data.Common;
using System.Text;

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

        AddPosParms(list);
        sb.Append('(');
        sb.Append(sql);
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

    public override string ToString() {
        return sb.ToString();
    }

    public IAsyncEnumerable<DbDataReader> ReadAllRows() {
        return ExecuteReader(ToString());
    }

    public Task<int> ExecuteGetRowsTouched() {
        return ExecuteNonQuery(ToString());
    }

    public Task<T?> ExecuteScalar<T>() {
        return ExecuteScalar<T>(ToString());
    }

}
