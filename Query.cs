using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL; 
public class Query : Details.QueryBase {

    string sql = "";
    string ResultSQL { get => PreProcessQuery(sql); }
    public static Query Create(string sql) => new Query { sql = sql };

    async public IAsyncEnumerable<DbDataReader> ReadAllRows() {
        var rdr = await ExecuteReader(ResultSQL);
        while (rdr.Read()) {
            yield return rdr;
        }
    }
    async public Task<DbDataReader> ReadOneRow() {
        var rdr = await ExecuteReader(ResultSQL);
        if (!rdr.Read()) throw new Exception("No rows found");
        // todo: check row only 1.
        return rdr;
    }
    public Task<int> ExecuteNonQuery() {
        return base.ExecuteNonQuery(ResultSQL);
    }
    public Task<object?> ExecuteScalar() {
        return base.ExecuteScalar(ResultSQL);
    }
}
