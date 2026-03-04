namespace StilettoSQL.Profile;

// Сконвертированные данные, которые отправляются в провайдер
public class StDataToDb {
    public enum DbType { AutoSelect, Json }
    public object? data;
    public DbType dbType = DbType.AutoSelect;
}

public interface IStConverterToDb {
    StDataToDb? Convert(object? value);
}

public interface IStConverterFromDb {
    bool Convert<T>(object? value, out T? result);
}

public record StProfile {

    public StProfile() {
        Provider = ProviderFactory<PostgressProvider>.Instance;
    }

    public StProfile(StProviderType provider) {
        if (provider == StProviderType.Postgress) {
            Provider = ProviderFactory<PostgressProvider>.Instance;
        }
        throw new NotSupportedException();
    }

    public string ConnectionString { get; init; } = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=postgres;Database=postgres;Connection Idle Lifetime=900";
    internal IDbProvider Provider { get; init; }

    public IStConverterToDb? UserConverterToDb { get; init; }
    public IStConverterFromDb? UserConverterFromDb { get; init; }

    internal StDataToDb ConvertToDb<T>(T? data) {

        var res = UserConverterToDb?.Convert(data);
        if (res != null) {
            return res;
        }

        // стандартный конверт
        return new StDataToDb { data = data };
    }

    internal T? ConvertFromDb<T>(object? obj) {

        // 2. Обработка пустоты
        if (obj is null or DBNull) {
            if (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
                return default;

            throw new InvalidOperationException($"Value is null, but {typeof(T).Name} is not nullable.");
        }

        // 3. Прямой каст (быстро и строго: string->string, long->long, long->long?, ...)
        if (obj is T variable)
            return variable;

        // 1. Приоритет пользователю
        if (UserConverterFromDb?.Convert(obj, out T? res) == true)
            return res;

        // 4. Безопасная до-конвертация чисел (исключая строки)
        // Позволяет int -> long, long->int (плохо!) но запрещает "123" -> 123
        //if (obj is not string) {
        //    Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        //    return (T)Convert.ChangeType(obj, targetType);
        //}

        throw new InvalidCastException($"Cannot cast {obj.GetType().Name} to {typeof(T).Name}.");
    }



}
