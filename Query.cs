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
    public Query(string sql) { this.sql = sql; }

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
    public Task<int> ExecuteInt() {
        return base.ExecuteNonQuery(ResultSQL);
    }
    public Task<object?> ExecuteScalar() {
        return base.ExecuteScalar(ResultSQL);
    }
    protected new Query Add<T>(string fieldName, T data) {
        base.Add(fieldName, data);
        return this;
    }
}
