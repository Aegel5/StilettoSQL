namespace StilettoSQL.Profile;
public sealed class AutoProfile : IDisposable {

    private readonly StProfile? _old;

    public AutoProfile(StProfile newProfile) {
        _old = StGlobal.CurrentProfile_.Value;
        StGlobal.CurrentProfile_.Value = newProfile;
    }
    public void Dispose() => StGlobal.CurrentProfile_.Value = _old;
}
