using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL.Query; 
public class Inserter : Details.QueryBase {

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

        bool usePositions = StGlobal.CurrentProvider.PreferPositionParms;

        StringBuilder sb = new();
        sb.Append("insert into ");
        sb.Append(tableName);
        if (namedParms != null) {
            sb.Append($"({string.Join(",", namedParms.Keys)})");
            sb.Append("values(");
            int count = 0;
            foreach (var item in namedParms) {
                count++;
                if (count != 1) sb.Append(",");
                if (usePositions) {
                    sb.Append("$");
                    sb.Append(count);
                } else {
                    sb.Append("@");
                    sb.Append(item.Key);
                }
            }
            sb.Append(")");

            if (usePositions) {
                // переводим namedParms в positions c той же сортировкой, что использовали при построении
                positionParms = namedParms.Values.ToList();
                namedParms = null;
            }
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


    public async Task<long> Exec_GetLong(string fieldName = "id") {
        return Convert.ToInt64(await ExecuteScalar(BuildSql()));
    }
    public Task<int> Exec_GetRowsTouched() {
        return ExecuteNonQuery(BuildSql());
    }

    public new Inserter Add<T>(string fieldName, T data) {
        base.Add(fieldName, data);
        return this;
    }
}
