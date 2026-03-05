using System.Data;
using System.Data.Common;

namespace StilettoSQL.Profile;

public enum StProviderSQL {
    PostgreSQL,
    MySQL
}

public interface IStDataConverter {
    bool ToDb(object value, IDbDataParameter parm);
    bool FromDb<T>(object value, out T result);
}

public record StProfile {
    public StProviderSQL ProviderSQL { get; init; } = StProviderSQL.PostgreSQL;
    public required Func<DbConnection> CreateConnection { get; init; }
    public IStDataConverter? DataConverter { get; init; }

    internal T? ConvertFromDb<T>(object? obj) {

        if (obj is null or DBNull) {
            if (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
                return default;

            throw new InvalidCastException($"Value is null/DBNull, but {typeof(T).Name} is not nullable.");
        }

        // Прямой каст (быстро и строго: string->string, long->long, long->long?, ...)
        if (obj is T variable)
            return variable;

        if (DataConverter?.FromDb(obj, out T res) == true)
            return res;

        throw new InvalidCastException($"Cannot cast {obj.GetType().Name} to {typeof(T).Name}.");
    }

}
