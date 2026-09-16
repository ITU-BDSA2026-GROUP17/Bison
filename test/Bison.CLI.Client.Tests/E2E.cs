namespace Bison.CLI.Client.Tests;

using System.Diagnostics;

using FluentAssertions;

public class E2E
{
    public static Process StartDatabase() {
        using var process = Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            FileName = "dotnet",
            Arguments = "run --project src/Bison.Database",
            WorkingDirectory = "../../../../../"
        });
        Assert.True(process is not null);

        while (true)
        {
            var output = process.StandardOutput.ReadLine();
            if (output == null || output.Contains("localhost"))
            {
                break;
            }
        }

        return process;
    }
    [Fact]
    public async Task CommentOnNonExistentObservationTest()
    {
        var db = StartDatabase();
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
        db.Close();
    }

    [Fact]
    public async Task TestObservationWorks()
    {
        var db = StartDatabase();
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
        db.Close();
    }

    [Fact]
    public async Task CommentOnExistentObservationWorks()
    {
        var db = StartDatabase();
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
        db.Close();
    }
}
