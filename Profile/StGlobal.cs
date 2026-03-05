namespace StilettoSQL.Profile;
public static class StGlobal {
    public static StProfile DefaultProfile = new();
    internal static AsyncLocal<StProfile?> CurrentProfile_ = new();
    public static StProfile CurrentProfile => CurrentProfile_.Value ?? DefaultProfile;
}


