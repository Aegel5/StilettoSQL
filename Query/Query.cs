using StilettoSQL.Internal;
using StilettoSQL.Profile;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL.Query; 

public class Query : Internal.QueryBase {

    string sql = "";
    public Query(string sql, params object[] list) {
        this.sql = sql;
        AddPosParms(list);
    }

    internal Query(string sql, QueryBase qbase):base(qbase) {
        this.sql = sql;
    }

    public IAsyncEnumerable<DbDataReader> ReadAllRows() {
        return ExecuteReader(sql);
    }
    //async public Task<DbDataReader> ReadSingleRow() {
    //    // todo: check row excactly 1. do copy data because we must close connection in this function.
    //    throw new NotSupportedException();
    //}
    public Task<int> ExecuteNonQuery() {
        return base.ExecuteNonQuery(sql);
    }
    public Task<T?> ExecuteScalar<T>() {
        return base.ExecuteScalar<T>(sql);
    }
    public Task<object?> ExecuteScalar() {
        return base.ExecuteScalar(sql);
    }


    public static IAsyncEnumerable<DbDataReader> ReadAllRows(string sql, params object[] list) {
        var q = new Query(sql, list);
        return q.ReadAllRows();
    }

    public static Task<int> Exec_GetRowsTouched(string sql, params object[] list) {
        var q = new Query(sql, list);
        return q.ExecuteNonQuery();
    }

    public static Task<T?> Exec_GetValue<T>(string sql, params object[] list) {
        return new Query(sql, list).ExecuteScalar<T>();
    }


    public Task<int> Exec_GetRowsTouched() {
        return ExecuteNonQuery();
    }

}
