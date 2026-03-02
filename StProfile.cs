namespace StilettoSQL;

// Сконвертированные данные, которые отправляются в провайдер
public class StDataToDb {
    public enum DbType { AutoSelect, Json }
    public object? data;
    public DbType dbType = DbType.AutoSelect;
}

public interface IStConverterToDb {
    bool Convert<T>(T value, out StDataToDb? result);
}

public interface IStConverterFromDb {
    bool Convert<T>(object value, out T? result);
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

    internal StDataToDb ConvertToDb<T>(T data) {

        if (UserConverterToDb?.Convert(data, out var res) == true) {
            return res;
        } 

        // стандартный конверт
        return new StDataToDb { data = data };
    }

    internal T? ConvertFromDb<T>(object obj) {

        if (UserConverterFromDb?.Convert(obj, out T res) == true){
            return res;
        }

        // делаем стандартный конверт

        if (obj is DBNull) {

            Type type = typeof(T);

            // 1. Ссылочные типы (Class, Interface) всегда могут быть null
            if (!type.IsValueType) 
                return default;

            // 2. Значимые типы (struct) могут быть null только если это Nullable<T>
            if (Nullable.GetUnderlyingType(type) != null) {
                return default;
            }

            throw new Exception("value is DBNull, but type can't hold nullable");
        }

        return (T)Convert.ChangeType(obj, typeof(T));
    }


}
