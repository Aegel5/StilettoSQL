using StilettoSQL.Query;
using System.Data;
using System.Data.Common;

namespace StilettoSQL.Query;

public enum StProviderSQL {
    PostgreSQL,
    MySQL
}

public interface IStDataConverter {
    bool ToDb(object value, IDbDataParameter parm);
    bool FromDb<T>(DbDataReader reader, int ordinal, out T result);
}

public record StProfile {
    public static StProfile DefaultProfile { get; set; } = new StProfile {
        CreateConnection = () => throw new NotImplementedException()
    };
    internal static AsyncLocal<StProfile?> CurrentProfile_ = new();
    internal static AsyncLocal<AutoTransaction?> CurrentTransaction_ = new();
    public static StProfile CurrentProfile => CurrentProfile_.Value ?? DefaultProfile;

    public sealed class AutoProfile : IDisposable {

        private readonly StProfile? _old;

        public AutoProfile(StProfile newProfile) {
            _old = CurrentProfile_.Value;
            CurrentProfile_.Value = newProfile;
        }
        public void Dispose() => CurrentProfile_.Value = _old;
    }

    public static void ChangeProfileAsyncLocal(StProfile prof) {
        CurrentProfile_.Value = prof;
    }

    public static AutoProfile ChangeProfileAutoRestore(StProfile prof) {
        return new AutoProfile(prof);
    }
    public StProviderSQL ProviderSQL { get; init; } = StProviderSQL.PostgreSQL;
    public required Func<DbConnection> CreateConnection { get; init; }
    public IStDataConverter? DataConverter { get; init; }

    internal AutoTransaction? transaction;

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
