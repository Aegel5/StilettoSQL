namespace StilettoSQL;

public record Profile {

    public string ConnectionString = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=postgres;Database=postgres;Connection Idle Lifetime=900";
    public EnumProvider Provider = EnumProvider.Postgress;

    internal IDbProvider ProviderImpl { get; init; }

    public Profile() {
        ProviderImpl = new PostgressProvider(ConnectionString);
    }

    public Func<object, DataToDb>? CustomDataConverterToDb { get; init; } = null;

    public delegate bool CustomDataConverterFromDbDelegate(object input, Type T, out object result);
    public CustomDataConverterFromDbDelegate? CustomDataConverterFromDb { get; init; } = null;

    internal Dictionary<string, DataToDb> ConvertToDb(Dictionary<string, object> parms) {
        Dictionary<string, DataToDb> res = new();
        foreach (var item in parms) {
            bool added = false;
            if (CustomDataConverterToDb != null) {
                // пробуем вызвать пользовательский конверт
                var converted = CustomDataConverterToDb(item.Value);
                if (converted?.data != null) {
                    // ok сконвертировали, добавляем
                    res.Add(item.Key, converted);
                    added = true;
                }
            }
            if (!added) {
                // добавляем дефолт
                res.Add(item.Key, new DataToDb { data = item.Value });
            }
        }
        return res;
    }

    internal T ConvertFromDb<T>(object obj) {
        if (CustomDataConverterFromDb != null) {
            if (CustomDataConverterFromDb(obj, typeof(T), out var res)){
                return (T)res;
            }
        }
        // делаем стандартный конверт
        return (T)Convert.ChangeType(obj, typeof(T));
    }


}
