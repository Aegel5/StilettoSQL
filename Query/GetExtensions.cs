using StilettoSQL.Profile;
using System.Data.Common;

namespace StilettoSQL.Query;

public static class _DbLoader_Extension {

    // With cast
    public static T? Val<T>(this DbDataReader reader, int ordinal) // quick access (in cycle)
        => StGlobal.CurrentProfile.ConvertFromDb<T>(reader, ordinal);
    public static T? Val<T>(this DbDataReader reader, string key)  // access by name
        => Val<T>(reader, reader.GetOrdinal(key));

    // Get default types
    //public static object? Val(this DbDataReader s, string key) => s[key];
    //public static object? Val(this DbDataReader s, int ordinal) => s.GetValue(ordinal); 

}
