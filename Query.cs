using StilettoSQL.Details;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL.Query; 
public class Query : Details.QueryBase {

    string sql = "";
    string ResultSQL { get => PreProcessQuery(sql); }
    public Query(string sql, params object[] list) {
        this.sql = sql;
        AddPosParms(list);
    }

    internal Query(string sql, QueryBase qbase):base(qbase) {
        this.sql = sql;
    }

    public IAsyncEnumerable<DbDataReader> ReadAllRows() {
        return ExecuteReader(ResultSQL);
    }
    //async public Task<DbDataReader> ReadSingleRow() {
    //    // todo: check row excactly 1. do copy data because we must close connection in this function.
    //    throw new NotSupportedException();
    //}
    public Task<int> ExecuteNonQuery() {
        return base.ExecuteNonQuery(ResultSQL);
    }
    public Task<object?> ExecuteScalar() {
        return base.ExecuteScalar(ResultSQL);
    }
    public new Query Add<T>(string fieldName, T data) {
        base.Add(fieldName, data);
        return this;
    }


    public static IAsyncEnumerable<DbDataReader> ReadAllRows(string sql, params object[] list) {
        var q = new Query(sql, list);
        return q.ReadAllRows();
    }

    public static Task<int> Exec_GetRowsToched(string sql, params object[] list) {
        var q = new Query(sql, list);
        return q.ExecuteNonQuery();
    }
}
