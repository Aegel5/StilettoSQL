namespace StilettoSQL;

// Сконвертированные данные, которые отправляются в провайдер
public class DataToDb {
    public enum DbType { AutoSelect, Json }
    public object? data;
    public DbType dbType = DbType.AutoSelect;
}

public interface IConverterToDb {
    bool Convert<T>(T value, out DataToDb? result);
}

public interface IConverterFromDb {
    bool Convert<T>(object value, out T? result);
}

public record Profile {

    public string ConnectionString { get; init; } = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=postgres;Database=postgres;Connection Idle Lifetime=900";
    public EnumProvider Provider { get; init; } = EnumProvider.Postgress;

    public IConverterToDb? UserConverterToDb { get; init; }
    public IConverterFromDb? UserConverterFromDb { get; init; }

    internal DataToDb ConvertToDb<T>(T data) {

        if (UserConverterToDb?.Convert(data, out var res) == true) {
            return res;
        } 

        // стандартный конверт
        return new DataToDb { data = data };
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
