namespace StilettoSQL.Profile;

public static class StGlobal {
    public static StProfile DefaultProfile { get; set; } = new StProfile {
        CreateConnection = () => throw new NotImplementedException()
    };
    internal static AsyncLocal<StProfile?> CurrentProfile_ = new();
    public static StProfile CurrentProfile => CurrentProfile_.Value ?? DefaultProfile;

    public sealed class AutoProfile : IDisposable {

        private readonly StProfile? _old;

        public AutoProfile(StProfile newProfile) {
            _old = CurrentProfile_.Value;
            CurrentProfile_.Value = newProfile;
        }
        public void Dispose() => CurrentProfile_.Value = _old;
    }

    public static void ChangeProfileAsyncLocal(StProfile prof) {
        CurrentProfile_.Value = prof;
    }

    public static AutoProfile ChangeProfileAutoRestore(StProfile prof) {
        return new AutoProfile(prof);
    }
}


