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

internal static class ProviderFactory<T> where T : class, IDbProvider, new() {
    // Этот экземпляр будет уникальным для каждого конкретного типа T
    // Инициализируется лениво и потокобезопасно самим .NET при первом обращении
    public static readonly T Instance = new T();
}

