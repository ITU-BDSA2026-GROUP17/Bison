namespace Bison.Database.Tests;

using System.Net.Mail;

using Bison.Database.Services;

public class CorrectEmailFormatUniTest
{
    [Fact]
    public void AssertNullMailIsInvalid()
    {
        // Arrange
        // Act
        var isValid = IUserService.IsValidEmail(null);
        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void AssertInvalidMailIsInvalid()
    {
        // Arrange
        // Act
        var isValid = IUserService.IsValidEmail("not-valid-mail-lol");
        // Assert
        isValid.Should().BeFalse();
    }

    [Property]
    public void AssertCorrectFormat(MailAddress mail)
    {
        // Arrange
        // Act
        var isValid = IUserService.IsValidEmail(mail.Address);
        // Assert
        isValid.Should().BeTrue();
    }
}
