namespace Lxna; 

public class Timer {
    public long Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    public long GetDiff() {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds() - Time;
    }

    public static long Now() {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}