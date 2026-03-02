using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL.Query; 
public class Inserter : Details.QueryBase {

    string tableName = "";
    bool skipOnConflict;

    public static Inserter Create(string tableName) => new Inserter {tableName = tableName };

    public Inserter SkipOnConflict(bool skip = true) {
        skipOnConflict = skip;
        return this;
    }

    string BuildSql(string? returnField = null) {

        StringBuilder sb = new();
        sb.Append("insert into ");
        sb.Append(tableName);
        if (namedParms != null) {
            sb.Append($"({string.Join(",", namedParms.Keys)})");
            sb.Append("values(");
            bool sec = false;
            foreach (var item in namedParms) {
                if (sec) sb.Append(",");
                sec = true;
                sb.Append("@");
                sb.Append(item.Key);
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


    public async Task<long> Finish_GetLong(string fieldName = "id") {
        return Convert.ToInt64(await ExecuteScalar(BuildSql()));
    }
    public Task<int> Finish_GetRowTouched() {
        return ExecuteNonQuery(BuildSql());
    }

    public new Inserter Add<T>(string fieldName, T data) {
        base.Add(fieldName, data);
        return this;
    }
}
