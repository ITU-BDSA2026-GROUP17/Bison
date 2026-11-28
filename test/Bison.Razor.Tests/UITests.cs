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
        await ServerProcess.CreateUser(Page, name, userName);
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
        await Expect(Page.GetByRole(AriaRole.Main)).ToMatchAriaSnapshotAsync("- heading \"Observation 1\" [level=2]\n- list:\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Roger Histand\":\n          - /url: /obs/RogerHistand\n      - text: /Grey heron standing still in the reeds at the lake\\. — \\(Fiskehejre\\) \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n- list:\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Luanna Muro\":\n          - /url: /obs/LuannaMuro\n      - text: /Nice! I saw one there yesterday too\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/");
        await Page.GotoAsync("/ob/3");
        await Expect(Page.GetByRole(AriaRole.Main)).ToMatchAriaSnapshotAsync("- heading \"Observation 3\" [level=2]\n- list:\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Wendell Ballan\":\n          - /url: /obs/WendellBallan\n      - text: /Large white heron in a wet meadow\\. Yellow bill, black legs\\. — \\(Sølvhejre\\) \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n- list:\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Roger Histand\":\n          - /url: /obs/RogerHistand\n      - text: /Do you have a photo\\? Size is hard to judge at distance\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Wendell Ballan\":\n          - /url: /obs/WendellBallan\n      - text: /Only a blurry one\\. It looked bigger than a grey heron\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Roger Histand\":\n          - /url: /obs/RogerHistand\n      - text: \"/PROPOSES: \\\"Silkehejre\\\" Could it be a little egret\\\\? Check for yellow feet\\\\. — \\\\d+\\\\/\\\\d+\\\\/\\\\d+ \\\\d+:\\\\d+:\\\\d+/\"\n  - listitem:\n    - paragraph:\n      - strong:\n        - link \"Nathan Sirmon\":\n          - /url: /obs/NathanSirmon\n      - text: \"/PROPOSES: \\\"Sølvhejre\\\" Yellow bill and large size fit great egret\\\\. I agree\\\\. — \\\\d+\\\\/\\\\d+\\\\/\\\\d+ \\\\d+:\\\\d+:\\\\d+/\"");
    }

    [Test]
    public async Task EditPostsWorksCorrectlyTest()
    {
        await Page.GotoAsync("/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "About 30 spoonbills at the" }).ClickAsync();
        await Expect(Page.Locator("a button", new() { HasText = "Edit" })).Not.ToBeVisibleAsync();
        await Page.GotoAsync("/");
        await ServerProcess.CreateUser(Page, "Test User", "testuser");

        await Page.GotoAsync("/ob/426");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Edit" })).Not.ToBeVisibleAsync();

        await Page.GotoAsync("/");
        await Page.Locator("#Input_Observation").FillAsync("Test obs");
        await Page.Locator("#Input_TaxonId").SelectOptionAsync(["MSTSNM:Arter:ee6dddf8-f785-ea11-aa77-501ac539d1ea"]);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Test obs" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Edit" })).ToBeVisibleAsync();
        await Expect(Page.Locator("p", new() { HasText = "Test obs" })).ToContainTextAsync("Test obs");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();
        await Page.Locator("#Input_Text").FillAsync("Test obs Part 2");
        await Page.Locator("#Input_TaxonId").SelectOptionAsync(["MSTSNM:Arter:2d66ddf8-f785-ea11-aa77-501ac539d1ea"]);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm" }).ClickAsync();

        await Expect(Page.Locator("small")).ToContainTextAsync("Nathejre");
        await Expect(Page.Locator("p", new() { HasText = "Test obs part 2" })).ToContainTextAsync("Test obs Part 2");

        await Page.Locator("#Input_Text").FillAsync("test part 1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "test part 1" }).GetByRole(AriaRole.Button).ClickAsync();
        await Page.Locator("#Input_Text").FillAsync("test part 3");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Main)).ToContainTextAsync("test part 3");

        await Page.Locator("#Input_Text").FillAsync("propsal 1");
        await Page.Locator("#Input_TaxonId").SelectOptionAsync(["MSTSNM:Arter:4b50c87f-f904-4fc0-8bb4-b0b500cd367d"]);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Main)).ToContainTextAsync("PROPOSES: \"Eremitibis\" propsal 1");

        await Page.GetByRole(AriaRole.Paragraph).Filter(new() { HasText = "Test User PROPOSES: \"" }).GetByRole(AriaRole.Button).ClickAsync();
        await Page.Locator("#Input_Text").FillAsync("propsal 3");
        await Page.Locator("#Input_TaxonId").SelectOptionAsync(["MSTSNM:Arter:ca4d8cf8-f785-ea11-aa77-501ac539d1ea"]);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Main)).ToContainTextAsync("PROPOSES: \"Silkehejre\" propsal 3");
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
