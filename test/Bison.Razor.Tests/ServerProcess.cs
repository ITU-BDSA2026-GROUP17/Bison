using System.Collections;
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

            // copy parent env to child process
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                info.AddEnvironmentVariable(entry.Key.ToString() ?? "", entry.Value?.ToString());
            }

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

    static internal async Task CreateUser(IPage page, string name, string userName)
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true }).FillAsync(name);
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true }).PressAsync("Tab");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(userName);
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("test@example.com");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync("Testpass@1234");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).PressAsync("Tab");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync("Testpass@1234");
        await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(userName);
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Testpass@1234");
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).PressAsync("Enter");
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

            _process.Dispose();
        }

        try
        {
            if (_dbFilePath is not null)
                File.Delete(_dbFilePath);
        }
        catch { }
    }
}
