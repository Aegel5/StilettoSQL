using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StilettoSQL;
public sealed class StAutoProfile : IDisposable {

    private readonly StProfile? _old;

    public StAutoProfile(StProfile newProfile) {
        _old = StGlobal.CurrentProfile_.Value;
        StGlobal.CurrentProfile_.Value = newProfile;
    }
    public void Dispose() => StGlobal.CurrentProfile_.Value = _old;
}
