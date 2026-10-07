namespace Bison.Database.Tests;

public class TaxonomiesUnitTest
{
    private readonly static Taxonomies Taxonomies = new();

    [Fact]
    public static void CorrectNameAndTaxonIdAssociationTest()
    {
        // Arrange
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
            var taxon = Taxonomies.GetTaxonRecordByID(item.Item1);
            // Assert
            taxon.Should().NotBeNull();
            taxon.VernacularName.Should().Be(item.Item2);

            // Act
            var taxon2 = Taxonomies.GetTaxonRecordByName(item.Item2);
            // Assert
            taxon2.Should().NotBeNull();
            taxon2.TaxonID.Should().Be(item.Item1);
        }
    }

    [Fact]
    public static void NotCorrectNameOrIdTest()
    {
        // Arrange
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
            var taxon = Taxonomies.GetTaxonRecordByID(str);
            // Assert
            taxon.Should().BeNull();

            // Act
            var taxon2 = Taxonomies.GetTaxonRecordByName(str);
            // Assert
            taxon2.Should().BeNull();
        }
    }

    [Fact]
    public static void GetSuperTaxonTest()
    {
        // Arrange
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
            var taxon = Taxonomies.GetTaxonRecordByID(item.Item1);
            // Assert
            taxon.Should().NotBeNull();
            taxon.ParentNameUsageID.Should().Be(item.Item2);

            // Act
            var parent = Taxonomies.GetTaxonRecordByID(item.Item2);
            // Assert
            parent.Should().NotBeNull();
        }

        // Act
        var aarefodede = Taxonomies.GetTaxonRecordByID("MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea");
        // Assert
        aarefodede.Should().NotBeNull();

        // Act
        var aarefodedeParent = Taxonomies.GetTaxonRecordByID(aarefodede.ParentNameUsageID);
        // Assert
        aarefodedeParent.Should().BeNull();
    }

    [Fact]
    public static void GetSubTaxonTest()
    {
        // Arrange
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
            var taxon = Taxonomies.GetTaxonRecordByID(parent.Item1);
            // Assert
            taxon.Should().NotBeNull();

            // Act
            var subs = Taxonomies.GetSubTaxons(taxon).ToList();
            // Assert
            subs.Count.Should().Be(parent.Item2);
        }
    }
}
