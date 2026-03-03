using StilettoSQL.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL.Query; 
public class Inserter : Internal.QueryBase {

    List<(string name, string? sql)> names = new();

    string tableName = "";
    bool skipOnConflict;

    public Inserter(string tableName) {
        this.tableName = tableName;
    }

    public Inserter SkipOnConflict(bool skip = true) {
        skipOnConflict = skip;
        return this;
    }

    string BuildSql(string? returnField = null) {

        StringBuilder sb = new();
        sb.Append("insert into ");
        sb.Append(tableName);

        if (names.Count > 0) {

            // names
            sb.Append('(');
            int count = 0;
            foreach (var item in names) {
                if (count != 1) sb.Append(",");
                sb.Append(item.name);
            }
            sb.Append(')');

            // values
            sb.Append("values(");
            count = 0;
            foreach (var item in names) {
                count++;
                if (count != 1) sb.Append(",");
                if (item.sql != null) {
                    sb.Append(item.sql);
                } else {
                    sb.Append("??");
                }
            }
            sb.Append(")");
        }

        if (skipOnConflict) {
            sb.Append($" on conflict do nothing");
        }

        if (returnField != null) {
            if (StGlobal.CurrentProfile.Provider.Type == StProviderType.Postgress) {
                sb.Append($" returning {returnField}");
            } else {
                throw new NotSupportedException();
            }
        }

        var res = sb.ToString();
        return res;
    }


    public Task<long> Exec_GetLong(string fieldName = "id") {
        return Exec_GetScalar<long>(fieldName);
    }

    public Task<T?> Exec_GetScalar<T>(string fieldName = "id") {
        return base.ExecuteScalar<T>(BuildSql(fieldName));
    }

    public Task<int> Exec_GetRowsTouched() {
        return ExecuteNonQuery(BuildSql());
    }

    public Inserter Value(string fieldName, object? data) {
        AddPosParm(data);
        names.Add((fieldName,null));
        return this;
    }

    public Inserter ValueSql(string fieldName, QueryBuilder q) {
        ConsumeParmsFrom(q);
        names.Add((fieldName,q.ToString()));
        return this;
    }

}
