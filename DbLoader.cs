using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL; 
public static class _DbLoader_EXT {
    public static long LoadLong(this DbDataReader s, string key) {

        return Global.CurrentProfile.ConvertFromDb<long>(s[key]);
    }
    public static int LoadInt(this DbDataReader s, string key) {
        return Global.CurrentProfile.ConvertFromDb<int>(s[key]);
    }
    public static string LoadString(this DbDataReader s, string key) {
        return Global.CurrentProfile.ConvertFromDb<string>(s[key]);
    }

    public static void LoadOut<T>(this DbDataReader s, string key, out T val) {
        val = Global.CurrentProfile.ConvertFromDb<T>(s[key]);
    }
    public static void LoadRef<T>(this DbDataReader s, string key, ref T val) {
        val = Global.CurrentProfile.ConvertFromDb<T>(s[key]);
    }
}
