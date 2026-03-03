using StilettoSQL.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL.Query; 
public class QueryBuilder : QueryBase {

    StringBuilder sb = new();
    bool whereAdded = false;

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

    public QueryBuilder And(string sql, params object[] list) {

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

    public Query ToQuery() {
        return new Query(sb.ToString(), this);
    }
}
