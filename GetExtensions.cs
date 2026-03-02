using System.Data.Common;

namespace StilettoSQL.Query; 
public static class _DbLoader_Extension {

    public static T? Val<T>(this DbDataReader s, string key) 
        => StGlobal.CurrentProfile.ConvertFromDb<T>(s[key]);

    public static void Val<T>(this DbDataReader s, string key, ref T? val) {
        val = s.Val<T>(key);
    }
}
