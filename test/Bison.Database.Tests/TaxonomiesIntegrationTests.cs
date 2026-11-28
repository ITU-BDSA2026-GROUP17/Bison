using Bison.Database.Services;

namespace Bison.Database.Tests;

public class TaxonomiesIntegrationTest
{
    [Fact]
    public static async Task CorrectNameAndTaxonIdAssociationTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new TaxonService(new DatabaseRepository(dbOptions.BisonDbContext));
        var idList = new (string, string)[]
        {
            ("MSTSNM:Arter:c18811f4-f785-ea11-aa77-501ac539d1ea", "Sølvhejre"),
            ("MSTSNM:Arter:4b50c87f-f904-4fc0-8bb4-b0b500cd367d", "Eremitibis"),
            ("MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea", "Årefodede"),
            ("MSTSNM:Arter:3efc2bf9-f785-ea11-aa77-501ac539d1ea","Sort ibis"),
            ("MSTSNM:Arter:c4f52bf9-f785-ea11-aa77-501ac539d1ea", "Topskarv")
        };

        foreach (var item in idList)
        {
            // Act
            Console.WriteLine(item.ToString());
            var taxon = await db.GetTaxonById(item.Item1);
            // Assert
            taxon.Should().NotBeNull();
            taxon.VernacularName.Should().Be(item.Item2);

            // Act
            var taxon2 = await db.GetTaxonByVernacularName(item.Item2);
            // Assert
            taxon2.Should().NotBeNull();
            taxon2.DwCId.Should().Be(item.Item1);
        }
    }

    [Fact]
    public static async Task NotCorrectNameOrIdTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new TaxonService(new DatabaseRepository(dbOptions.BisonDbContext));
        var list = new string[]
        {
            "thisisnotreal",
            "",
            "19328tjsdfkj",
            "42",
            "Å``%@)(/&%¤#@£$€{[]}\"\\",
            null,
            " "
        };

        foreach (var str in list)
        {
            // Act
            Console.WriteLine(str);
            var taxon1 = await db.GetTaxonById(str);
            var taxon2 = await db.GetTaxonByVernacularName(str);
            // Assert
            taxon1.Should().BeNull();
            taxon2.Should().BeNull();
        }
    }

    [Fact]
    public static async Task GetSuperTaxonTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new TaxonService(new DatabaseRepository(dbOptions.BisonDbContext));
        var idList = new (string, string)[]
        {
            ("MSTSNM:Arter:c18811f4-f785-ea11-aa77-501ac539d1ea", "MSTSNM:Arter:7f9ef9f3-f785-ea11-aa77-501ac539d1ea"),
            ("MSTSNM:Arter:4b50c87f-f904-4fc0-8bb4-b0b500cd367d", "MSTSNM:Arter:ef2c03c2-1064-461b-9b3e-b0b500cd20d7"),
            ("MSTSNM:Arter:3efc2bf9-f785-ea11-aa77-501ac539d1ea","MSTSNM:Arter:7dbff9f3-f785-ea11-aa77-501ac539d1ea"),
            ("MSTSNM:Arter:c4f52bf9-f785-ea11-aa77-501ac539d1ea", "MSTSNM:Arter:701cda16-1f30-4973-b36b-af7e00fd7609")
        };

        foreach (var item in idList)
        {
            // Act
            Console.WriteLine(item.ToString());
            var taxon = await db.GetTaxonById(item.Item1);
            // Assert
            taxon.Should().NotBeNull();
            taxon.ParentId.Should().NotBeNull();
            taxon.ParentId.Should().Be(item.Item2);
        }

        // Act
        var aarefodede = await db.GetTaxonById("MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea");
        // Assert
        aarefodede.Should().NotBeNull();
        aarefodede.ParentId.Should().BeNull();
    }

    [Fact]
    public static async Task GetSubTaxonTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new TaxonService(new DatabaseRepository(dbOptions.BisonDbContext));
        var list = new (string, int)[]
        {
            ("MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea", 6),
            ("MSTSNM:Arter:5c5967e4-f785-ea11-aa77-501ac539d1ea", 2),
            ("MSTSNM:Arter:3efc2bf9-f785-ea11-aa77-501ac539d1ea", 0),
            ("MSTSNM:Arter:25aaf9f3-f785-ea11-aa77-501ac539d1ea", 1)
        };

        foreach (var parent in list)
        {
            // Act
            var taxon = await db.ToTaxonAsync(await db.GetTaxonById(parent.Item1));
            // Assert
            taxon.Should().NotBeNull();
            taxon.Children.Count.Should().Be(parent.Item2);
        }
    }

    [Fact]
    public static async Task GetTaxonsAsyncOrderingIsCorrectTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        dbOptions.SeedTaxonsOnly();
        var db = new TaxonService(new DatabaseRepository(dbOptions.BisonDbContext));

        // Act
        var taxons = await db.GetTaxonsAsync(10, 5);

        // Assert
        taxons.Select(t => t.DwCId)
            .Should()
            .BeEqualTo([
                "MSTSNM:Arter:5c5967e4-f785-ea11-aa77-501ac539d1ea",
            "MSTSNM:Arter:af5967e4-f785-ea11-aa77-501ac539d1ea",
            "MSTSNM:Arter:7f9ef9f3-f785-ea11-aa77-501ac539d1ea",
            "MSTSNM:Arter:819ef9f3-f785-ea11-aa77-501ac539d1ea",
            "MSTSNM:Arter:dca0f9f3-f785-ea11-aa77-501ac539d1ea",
            "MSTSNM:Arter:74a1f9f3-f785-ea11-aa77-501ac539d1ea",
            "MSTSNM:Arter:25aaf9f3-f785-ea11-aa77-501ac539d1ea",
            "MSTSNM:Arter:eeb1f9f3-f785-ea11-aa77-501ac539d1ea",
            "MSTSNM:Arter:fab9f9f3-f785-ea11-aa77-501ac539d1ea",
            "MSTSNM:Arter:0aadf9f3-f785-ea11-aa77-501ac539d1ea"
            ]);
    }
}
