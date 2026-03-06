using System.Data;
using System.Data.Common;

namespace StilettoSQL.Profile;

public enum StProviderSQL {
    PostgreSQL,
    MySQL
}

public interface IStDataConverter {
    bool ToDb(object value, IDbDataParameter parm);
    bool FromDb<T>(DbDataReader reader, int ordinal, out T result);
}

public record StProfile {
    public StProviderSQL ProviderSQL { get; init; } = StProviderSQL.PostgreSQL;
    public required Func<DbConnection> CreateConnection { get; init; }
    public IStDataConverter? DataConverter { get; init; }

    static internal T? ConvertToNull<T>() {
        if (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
            return default;

        throw new InvalidCastException($"Value is NULL, but {typeof(T).Name} is not nullable.");
    }

    internal T? ConvertFromDb<T>(DbDataReader reader, int ordinal) {

        if (reader.IsDBNull(ordinal)) {
            return ConvertToNull<T>();
        }

        if (DataConverter?.FromDb(reader, ordinal, out T res) == true)
            return res;

        return reader.GetFieldValue<T>(ordinal);

    }

}
