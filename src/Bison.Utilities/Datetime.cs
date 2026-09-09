namespace Bison.Utilities;


public class DateTimeUtilities
{
    private DateTimeUtilities() { }

    public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        // taken from https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
        DateTime unixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        unixEpoch = unixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
        return unixEpoch;
    }

    public static int DateTimeToUnixTimeStamp(DateTime dateTime)
    {
        DateTime unixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = dateTime.ToUniversalTime() - unixEpoch;
        return (int)Math.Floor(diff.TotalSeconds);
    }
}
