namespace Bison.CLI.Client.Tests;

using System.Diagnostics;

using FluentAssertions;

public class E2E
{
    [Fact]
    public async void CommentOnNonExistentObservationTest()
    {
        using var process = Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            FileName = "dotnet",
            Arguments = "run --project src/Bison.CLI.Client -- comment 2147483647 \"test here\"",
            WorkingDirectory = "../../../../../"
        });
        Assert.True(process is not null);

        var output = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();

        output.Trim().Should().Be("Observation id 2147483647 does not exist");
    }

    [Fact]
    public async void TestObservationWorks()
    {
        using var process = Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            FileName = "dotnet",
            Arguments = "run --project src/Bison.CLI.Client -- observe \"Penguin\" \"At zoo\"",
            WorkingDirectory = "../../../../../"
        });
        Assert.True(process is not null);

        var output = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();

        output.Trim().Should().Be("Observation has been saved.");
    }

    [Fact]
    public async void CommentOnExistentObservationWorks()
    {
        using var process = Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            FileName = "dotnet",
            Arguments = "run --project src/Bison.CLI.Client -- comment 0 \"King penguin spotted as well\"",
            WorkingDirectory = "../../../../../"
        });
        Assert.True(process is not null);

        var output = process.StandardOutput.ReadToEnd();
        await process.WaitForExitAsync();

        output.Trim().Should().Be("Comment has been saved.");
    }
}
