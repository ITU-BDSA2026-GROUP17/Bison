// See https://aka.ms/new-console-template for more information

namespace Bison
{
    class Utilities
    {
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

    public class ObservationRecord
    {
        public required string Author { get; set; }
        public required string Observation { get; set; }
        public int Timestamp { get; set; }

        public DateTime GetAsDateTime()
        {
            return Utilities.UnixTimeStampToDateTime(Timestamp);
        }

        public override string ToString()
        {
            return Author + " @ " + GetAsDateTime().ToString() + ": " + Observation;
        }
    }

    public class Program
    {
        static int Main(string[] args)
        {
            var root = UserInterface.GetRootCommand();

            var result = root.Parse(args);
            return result.Invoke();
        }
    }
}
