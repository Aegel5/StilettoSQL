using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL; 
public class Updater : Details.QueryBase {
    string tableName = "";
    string? whereSQL = null;
    public static Updater Create(string tableName) => new Updater { tableName = tableName };

    public Updater Where(string whereSQL) {
        this.whereSQL = whereSQL;
        return this;
    }

    string BuildSql() {

        StringBuilder sb = new();
        sb.Append("update ");
        sb.Append(tableName);
        if (parms != null) {
            sb.Append(" set ");
            bool first = true;
            foreach (var item in parms) {
                if(!first) sb.Append(",");
                first = false;
                sb.Append(item.Key);
                sb.Append("=@");
                sb.Append(item.Key);
            }

        }

        if (whereSQL != null) {
            var wh = PreProcessQuery(whereSQL);
            sb.Append(' ');
            sb.Append(wh);
        }

        var res = sb.ToString();
        return res;
    }
    public Task<int> Finish_GetRowTouched() {
        return ExecuteNonQuery(BuildSql());
    }
    public new Updater Add<T>(string fieldName, T data) {
        base.Add(fieldName, data);
        return this;
    }
}
