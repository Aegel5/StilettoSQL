using System.Data.Common;
namespace StilettoSQL.Query; 

public class Query : Internal.QueryBase {

    string sql = "";
    public Query(string sql, params object[] list) {
        this.sql = sql;
        AddPosParms(list);
    }

    public Query Timeout(TimeSpan timeout) {
        _Timeout = timeout;
        return this;
    }

    public IAsyncEnumerable<DbDataReader> ReadAllRows() {
        return ExecuteReader(sql);
    }
    public Task<int> ExecuteGetRowsTouched() {
        return base.ExecuteNonQuery(sql);
    }
    public Task<T?> ExecuteScalar<T>() {
        return base.ExecuteScalar<T>(sql);
    }
    public Task<object?> ExecuteScalar() {
        return base.ExecuteScalar(sql);
    }


    public static IAsyncEnumerable<DbDataReader> ReadAllRows(string sql, params object[] list) {
        return new Query(sql, list).ReadAllRows();
    }

    public static Task<int> ExecuteGetRowsTouched(string sql, params object[] list) {
        return new Query(sql, list).ExecuteGetRowsTouched();
    }

    public static Task<T?> ExecuteScalar<T>(string sql, params object[] list) {
        return new Query(sql, list).ExecuteScalar<T>();
    }

    public Query Value(object obj) {
        AddPosParm(obj);
        return this;
    }


}
