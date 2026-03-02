using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StilettoSQL; 
public static class Global {
    public static Profile DefaultProfile = new();
    internal static AsyncLocal<Profile?> CurrentProfile_ = new();
    public static Profile CurrentProfile => CurrentProfile_.Value ?? DefaultProfile;
    internal static IDbProvider CurrentProvider {
        get {
            if (CurrentProfile.Provider == EnumProvider.Postgress)
                return ProviderFactory<PostgressProvider>.Instance;
            throw new NotSupportedException();
        }
    }
}


