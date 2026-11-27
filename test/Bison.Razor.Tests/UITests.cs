using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;

namespace Bison.Razor.Tests;

[TestFixture]
public partial class UITests : PageTest
{
    private ServerProcess _serverProcess;

    [SetUp]
    public async Task Init()
    {
        _serverProcess = await ServerProcess.StartServer();
    }
    [TearDown]
    public async Task Cleanup()
    {
        await _serverProcess.StopServer();
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            ColorScheme = ColorScheme.Light,
            ViewportSize = new()
            {
                Width = 1920,
                Height = 1080
            },
            BaseURL = ServerProcess.BASE_URL,
        };
    }

    [TestCase("Test User", "testuser")]
    [TestCase("Goated_Person", "my_user_name_is_goated")]
    [TestCase("WOAH ME", "WOAHOMG._.wha")]
    [Test]
    public async Task TestCreatingObservationWorks(string name, string userName)
    {
        await Page.GotoAsync("/");
        await Expect(Page.Locator(".observationbox").First).Not.ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true }).FillAsync(name);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(userName);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("test@example.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync("Testpass@1234");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync("Testpass@1234");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(userName);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Testpass@1234");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).PressAsync("Enter");
        await Expect(Page.Locator(".observationbox").First).ToBeVisibleAsync();
        await Expect(Page.Locator(".observationbox").First).ToContainTextAsync(name);
    }

    [Test]
    public async Task CorrectObservationsOnPages()
    {
        await Page.GotoAsync("/");
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("About 30 spoonbills at the lagoon, seen through the scope.");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Next Page" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Three grey herons at a gravel pit, seen through the scope.");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Previous Page" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("About 30 spoonbills at the lagoon, seen through the scope.");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Nathan Sirmon" }).First.ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("About 30 spoonbills at the lagoon, seen through the scope.");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Next Page" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Four bitterns resting near the lake.");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Previous Page" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("About 30 spoonbills at the lagoon, seen through the scope.");
        await Page.GotoAsync("/ob/1");
        await Expect(Page.Locator("body")).ToMatchAriaSnapshotAsync("- heading \"Observation 1\" [level=2]\n- list:\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Roger Histand\":\n          - /url: /obs/RogerHistand\n      - text: /Grey heron standing still in the reeds at the lake\\. — \\(Fiskehejre\\) \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n- list:\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Luanna Muro\":\n          - /url: /obs/LuannaMuro\n      - text: /Nice! I saw one there yesterday too\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/");
        await Page.GotoAsync("/ob/3");
        await Expect(Page.Locator("body")).ToMatchAriaSnapshotAsync("- heading \"Observation 3\" [level=2]\n- list:\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Wendell Ballan\":\n          - /url: /obs/WendellBallan\n      - text: /Large white heron in a wet meadow\\. Yellow bill, black legs\\. — \\(Sølvhejre\\) \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n- list:\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Nathan Sirmon\":\n          - /url: /obs/NathanSirmon\n      - text: \"/PROPOSES: \\\"Sølvhejre\\\" Yellow bill and large size fit great egret\\\\. I agree\\\\. — \\\\d+\\\\/\\\\d+\\\\/\\\\d+ \\\\d+:\\\\d+:\\\\d+/\"\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Roger Histand\":\n          - /url: /obs/RogerHistand\n      - text: \"/PROPOSES: \\\"Silkehejre\\\" Could it be a little egret\\\\? Check for yellow feet\\\\. — \\\\d+\\\\/\\\\d+\\\\/\\\\d+ \\\\d+:\\\\d+:\\\\d+/\"\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Wendell Ballan\":\n          - /url: /obs/WendellBallan\n      - text: /Only a blurry one\\. It looked bigger than a grey heron\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Roger Histand\":\n          - /url: /obs/RogerHistand\n      - text: /Do you have a photo\\? Size is hard to judge at distance\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/");
    }

    [Test]
    public async Task OnlyUsersObservations()
    {
        await Page.GotoAsync("/obs/RogerHistand");
        await Expect(Page.Locator("#messagelist li p strong a")
            .Filter(new() { HasNotText = "Roger Histand" }))
            .ToHaveCountAsync(0);
    }

    [TestCase("/")]
    [TestCase("/ob/3")]
    [TestCase("/obs/RogerHistand")]
    [Test]
    public async Task AxeCheckPageIsClean(string page)
    {
        await Page!.GotoAsync(page);

        AxeResult axeResults = await Page!.RunAxe();
        Assert.That(axeResults.Violations, Is.Null.Or.Empty);
    }
}
