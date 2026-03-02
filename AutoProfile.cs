using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL;
public sealed class AutoProfile : IDisposable {

    private readonly Profile? _old;

    public AutoProfile(Profile newProfile) {
        _old = Global.CurrentProfile_.Value;
        Global.CurrentProfile_.Value = newProfile;
    }
    public void Dispose() => Global.CurrentProfile_.Value = _old;
}
