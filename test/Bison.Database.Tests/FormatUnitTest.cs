namespace Bison.Database.Tests;

using System.Globalization;

public class FormatUnitTest
{
    [Property]
    public void AssertCorrectFormat(DateTime dateTime)
    {
        // Arrange
        var correctFormat = dateTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        // Act
        var testFormat = dateTime.ToString(IDatabaseRepository.DATE_FORMAT);
        // Assert
        testFormat.Should().Be(correctFormat);
    }
}