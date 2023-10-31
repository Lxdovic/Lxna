namespace Lxna; 

public class Timer {
    public long Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public long GetDiff() {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Time;
    }

    public static long Now() {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}