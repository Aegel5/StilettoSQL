using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StilettoSQL.Profile; 
public static class StGlobal {
    public static StProfile DefaultProfile = new();
    internal static AsyncLocal<StProfile?> CurrentProfile_ = new();
    public static StProfile CurrentProfile => CurrentProfile_.Value ?? DefaultProfile;
    internal static IDbProvider CurrentProvider => CurrentProfile.Provider;
}


