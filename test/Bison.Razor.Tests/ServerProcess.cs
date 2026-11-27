using System.Diagnostics;

using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;

namespace Bison.Razor.Tests;

public class ServerProcess
{
    public static readonly string BASE_URL = "http://localhost:5273";
    private static ProcessStartInfo Info
    {
        get
        {
            var rootDir = Environment.CurrentDirectory;
            while (!File.Exists(Path.Join(rootDir, "Bison.sln")))
            {
                rootDir = Path.GetDirectoryName(rootDir);
            }
            var info = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = "dotnet",
                Arguments = "run --project src/Bison.Razor",
                WorkingDirectory = rootDir,
            };

            info.AddEnvironmentVariable("PictureBasePath", Path.Join(rootDir, "/pictures/"));

            return info;
        }
    }

    private string? _dbFilePath;
    private Process? _process;

    public static async Task<ServerProcess> StartServer()
    {
        var info = Info;
        var dbFile = Path.GetTempFileName();
        info.AddEnvironmentVariable("BISONDBPATH", dbFile);
        var process = new ServerProcess()
        {
            _process = Process.Start(info) ?? throw new NullReferenceException("Could not start process"),
            _dbFilePath = dbFile
        };

        while (true)
        {
            var output = await process._process.StandardOutput.ReadLineAsync();

            if (output == null || output.Contains("localhost"))
            {
                break;
            }
        }

        return process;
    }

    public async Task StopServer()
    {
        if (_process is not null)
        {
            _process.Kill();

            if (!_process.HasExited)
                await _process.WaitForExitAsync();

            foreach (var process in Process.GetProcessesByName("Bison.Razor"))
            {
                process.Kill();
                await process.WaitForExitAsync();
            }

            Assert.That(
                Process.GetProcessesByName("Bison.Razor"),
                Is.Empty
            );

            _process.Dispose();
        }

        if (_dbFilePath is not null)
            File.Delete(_dbFilePath);
    }
}
