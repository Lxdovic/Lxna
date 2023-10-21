namespace Lxna; 

public class Timer {
    public long CreatedAt;
    
    public Timer() {
        CreatedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
    
    public long GetDiff() {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds() - CreatedAt;
    }

    public static long Now() {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}