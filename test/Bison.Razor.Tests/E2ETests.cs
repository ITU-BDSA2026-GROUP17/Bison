namespace Bison.Razor.Tests;

[TestFixture]
public partial class EndToEndTests : PageTest
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

    [TestCase("Test User", "testuser", "This is a fun test")]
    [TestCase("Goated_Person", "my_user_name_is_goated", "my_observation_is_about_a_birb")]
    [TestCase("WOAH ME", "WOAHOMG._.wha", "i like trains!")]
    [Test]
    public async Task TestCreatingObservationWorks(string name, string userName, string observation)
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

        await Page.Locator("#Input_Observation").FillAsync(observation);
        await Page.Locator("#Input_TaxonId").SelectOptionAsync(["MSTSNM:Arter:b17392a6-9d0e-486b-91ed-b18b00d742e2"]);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync(name);
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync(observation);
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("(Dværgskarv)");
    }

    [TestCase("Test User", "testuser", "This is a fun test")]
    [TestCase("Goated_Person", "my_user_name_is_goated", "my_observation_is_about_a_birb")]
    [TestCase("WOAH ME", "WOAHOMG._.wha", "i like trains!")]
    [Test]
    public async Task TestCreatingCommentWorks(string name, string userName, string comment)
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

        await Page.GotoAsync("/ob/1");
        await Page.Locator("#Input_Text").FillAsync(comment);
        await Page.Locator("#Input_TaxonId").SelectOptionAsync(["None"]);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.Locator("#messagelist li p", new() { HasText = name })).ToContainTextAsync(name);
        await Expect(Page.Locator("#messagelist li p", new() { HasText = comment })).ToContainTextAsync(comment);
        await Expect(Page.Locator("#messagelist li p", new() { HasText = comment })).Not.ToContainTextAsync("PROPOSES");
    }

    [TestCase("Test User", "testuser", "This is a fun test")]
    [TestCase("Goated_Person", "my_user_name_is_goated", "my_observation_is_about_a_birb")]
    [TestCase("WOAH ME", "WOAHOMG._.wha", "i like trains!")]
    [Test]
    public async Task TestCreatingProposalWorks(string name, string userName, string proposal)
    {
        await Page.GotoAsync("/");
        await Expect(Page.Locator(".observationbox").First).Not.ToBeVisibleAsync();
        await ServerProcess.CreateUser(Page, name, userName);
        await Expect(Page.Locator(".observationbox").First).ToBeVisibleAsync();
        await Expect(Page.Locator(".observationbox").First).ToContainTextAsync(name);

        await Page.GotoAsync("/ob/1");
        await Page.Locator("#Input_Text").FillAsync(proposal);
        await Page.Locator("#Input_TaxonId").SelectOptionAsync(["Skarv"]);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.Locator("#messagelist li p", new() { HasText = name })).ToContainTextAsync(name);
        await Expect(Page.Locator("#messagelist li p", new() { HasText = proposal })).ToContainTextAsync(proposal);
        await Expect(Page.Locator("#messagelist li p", new() { HasText = "PROPOSES: \"Skarv\"" })).ToContainTextAsync("PROPOSES: \"Skarv\"");
    }
}
