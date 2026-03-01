using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL; 
public class Reader {

    string sql = "";
    Dictionary<string, object> parms;

    public Reader(string sql_) {
        sql_ = sql;
    }

    async public IAsyncEnumerable<DbDataReader> ReadAll() {
        var rdr = await Global.CurrentProvider.ExecuteReader(sql, Global.CurrentProfile.ConvertToDb(parms));
        while (rdr.Read()) {
            yield return rdr;
        }
    }

}
