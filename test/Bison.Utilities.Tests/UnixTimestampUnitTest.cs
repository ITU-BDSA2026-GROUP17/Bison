namespace Bison.Utilities.Tests;

using Bison.Utilities;

using FluentAssertions;

using FsCheck.Xunit;

public class UnixTimestampUnitTest
{
    static DateTime GetDateTimeWithoutSubseconds()
    {
        var current = DateTime.Now;
        var ticks = current.Ticks / TimeSpan.TicksPerSecond;

        DateTime without = new(ticks * TimeSpan.TicksPerSecond);
        without.Nanosecond.Should().Be(0);
        return without;
    }

    [Fact]
    public void UnixTimestampTranslationWorks()
    {
        var current = GetDateTimeWithoutSubseconds();
        var unix = DateTimeUtilities.DateTimeToUnixTimeStamp(current);
        var restored = DateTimeUtilities.UnixTimeStampToDateTime(unix);

        restored.Should().Be(current);
    }
}
