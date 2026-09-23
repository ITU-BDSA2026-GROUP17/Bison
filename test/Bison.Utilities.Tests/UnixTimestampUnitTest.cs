namespace Bison.Utilities.Tests;

using Bison.Utilities;

using FluentAssertions;

using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

public class UnixTimestampUnitTest
{
    public static Arbitrary<DateTime> DateGenerator()
    {
        DateTime minDate = new(1970, 1, 1, 0, 0, 0);
        DateTime maxDate = new(2038, 1, 19, 3, 14, 7);
        int totalSecRange = (int)(maxDate - minDate).TotalSeconds;

        // Gen.Choose picks a random integer, which we map back to a DateTime
        Gen<DateTime> dateGenerator = Gen.Choose(0, totalSecRange)
            .Select(secs => minDate.AddSeconds(secs));

        return dateGenerator.ToArbitrary();
    }

    [Property(Arbitrary = new[] { typeof(UnixTimestampUnitTest) })]
    public void UnixTimestampTranslationWorks(DateTime dateTime)
    {
        var unix = DateTimeUtilities.DateTimeToUnixTimeStamp(dateTime);
        var restored = DateTimeUtilities.UnixTimeStampToDateTime(unix);

        restored.Should().Be(dateTime);
    }
}
