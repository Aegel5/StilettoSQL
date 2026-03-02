using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL; 
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
        if (parms != null) {
            sb.Append($"({string.Join(",", parms.Keys)})");
            sb.Append("values(");
            bool sec = false;
            foreach (var item in parms) {
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
            if (Global.CurrentProfile.Provider == EnumProvider.Postgress) {
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
