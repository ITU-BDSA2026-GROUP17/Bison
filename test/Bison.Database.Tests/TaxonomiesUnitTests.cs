using Microsoft.EntityFrameworkCore;

namespace Bison.Database.Tests;

public class TaxonomiesUnitTest
{
    [Fact]
    public static async Task CorrectNameAndTaxonIdAssociationTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = dbOptions.BisonDbContext;
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
            var taxon = await db.Taxons.Where(t => t.DWC_TaxonId == item.Item1).FirstAsync();
            // Assert
            taxon.Should().NotBeNull();
            taxon.VernacularName.Should().Be(item.Item2);

            // Act
            var taxon2 = await db.Taxons.Where(t => t.VernacularName == item.Item2).FirstAsync();
            // Assert
            taxon2.Should().NotBeNull();
            taxon2.DWC_TaxonId.Should().Be(item.Item1);
        }
    }

    [Fact]
    public static async Task NotCorrectNameOrIdTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = dbOptions.BisonDbContext;
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
            var taxon = await db.Taxons.Where(t => t.DWC_TaxonId == str).FirstAsync();
            // Assert
            taxon.Should().BeNull();
        }
    }

    [Fact]
    public static async Task GetSuperTaxonTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = dbOptions.BisonDbContext;
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
            var taxon = await db.Taxons.Where(t => t.DWC_TaxonId == item.Item1).FirstAsync();
            // Assert
            taxon.Should().NotBeNull();
            taxon.Parent.Should().NotBeNull();
            taxon.Parent.DWC_TaxonId.Should().Be(item.Item2);
        }

        // Act
        var aarefodede = await db.Taxons.Where(t => t.DWC_TaxonId == "MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea").FirstAsync();
        // Assert
        aarefodede.Should().NotBeNull();
        aarefodede.Parent.Should().BeNull();
    }

    [Fact]
    public static async Task GetSubTaxonTest()
    {
        // Arrange
        using var dbOptions = new DbOptions();
        var db = dbOptions.BisonDbContext;
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
            var taxon = await db.Taxons.Where(t => t.DWC_TaxonId == parent.Item1).FirstAsync();
            // Assert
            taxon.Should().NotBeNull();
            taxon.Children.Count.Should().Be(parent.Item2);
        }
    }
}
