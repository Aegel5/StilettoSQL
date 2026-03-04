using StilettoSQL.Profile;
using System.Data.Common;

namespace StilettoSQL.Query;
public static class _DbLoader_Extension {

    // делаем cast
    public static T? Val<T>(this DbDataReader s, string key)
        => StGlobal.CurrentProfile.ConvertFromDb<T>(s[key]);

    // отдаем напрямую
    public static object? Val(this DbDataReader s, string key) => s[key];

}
