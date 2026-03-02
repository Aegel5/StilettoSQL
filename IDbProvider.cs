using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL;

public enum EnumProvider {
    Postgress
}

internal record ParamsForProvider {
    public required string sql { get; init; }
    public Dictionary<string, DataToDb>? fields { get; init; }
    public TimeSpan? timeout { get; init; }
}

internal interface IDbProvider {
    Task<DbDataReader> ExecuteReader(ParamsForProvider parms);
    Task<object?> ExecuteScalar(ParamsForProvider parms);
    Task<int> ExecuteNonQuery(ParamsForProvider parms);

}

