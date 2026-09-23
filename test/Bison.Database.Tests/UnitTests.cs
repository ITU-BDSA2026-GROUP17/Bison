using Bison.Models;
using Bison.Utilities;

using FluentAssertions;

namespace Bison.Database.Tests;

public class UnitTests
{
    static (CSVDatabase, Action) SetupTestDatabase()
    {
        var obsPath = Path.GetTempFileName();
        var comPath = Path.GetTempFileName();
        var obsIdPath = Path.GetTempFileName();
        var proposalPath = Path.GetTempFileName();
        CSVDatabase db = new(obsPath, comPath, obsIdPath, proposalPath);

        db.StoreObservation(
            new Observation
            {
                Author = "lrec",
                Text = "Eurasian jay",
                Location = "Assistentens Kirkegård",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        db.StoreComment(
            new Comment
            {
                Id = 0,
                Author = "mawb",
                Text = "I do think it's a jay of somekind",
                CreatedAt = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        db.StoreComment(
            new Comment
            {
                Id = 0,
                Author = "lrec",
                Text = "I think it might be a bird",
                CreatedAt = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        db.StoreObservation(
            new Observation
            {
                Author = "mawb",
                Text = "Ghost",
                Location = "Here",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        db.StoreComment(
            new Comment
            {
                Id = 1,
                Author = "pask",
                Text = "You should be medicated",
                CreatedAt = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        db.StoreComment(
            new Comment
            {
                Id = 1,
                Author = "mawb",
                Text = "Well yes, but for a different reason",
                CreatedAt = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        db.StoreObservation(
            new Observation
            {
                Author = "toov",
                Text = "Magnus",
                Location = "ITU",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        db.StoreComment(
            new Comment
            {
                Id = 2,
                Author = "mawb",
                Text = "Please don't observe me like this.",
                CreatedAt = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        db.StoreComment(
            new Comment
            {
                Id = 2,
                Author = "toov",
                Text = "ok ig",
                CreatedAt = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        db.StoreObservation(
            new Observation
            {
                Author = "pask",
                Text = "Saw a group of crows",
                Location = "Tivoli",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );

        db.StoreObservation(
            new Observation
            {
                Author = "lrec",
                Text = "Peanut Butter Baby",
                Location = "The Table",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        db.StoreComment(
            new Comment
            {
                Id = 4,
                Author = "toov",
                Text = "Ahh",
                CreatedAt = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        return (db, () =>
        {
            SavingRetrievingTest.DeleteFileIfExists(obsPath);
            SavingRetrievingTest.DeleteFileIfExists(comPath);
            SavingRetrievingTest.DeleteFileIfExists(obsIdPath);
        }
        );
    }

    private static readonly bool[] BooleanArray = [false, true];

    [Fact]
    public static void TestExceptionsOnInvalidFiles()
    {
        // tests empty and empty parent
        try
        {
            _ = new SimpleCounter("");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("filePath");
        }
        try
        {
            _ = new SimpleCounter("/");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentNullException e)
        {
            e.ParamName.Should().Be("parent");
        }

        var permutations = from b1 in BooleanArray
                           from b2 in BooleanArray
                           from b3 in BooleanArray
                           from b4 in BooleanArray
                           select new { b1, b2, b3, b4 };

        foreach (var permutation in permutations)
        {
            if (!permutation.b1 && !permutation.b2 && !permutation.b3 && !permutation.b4)
                continue;

            var should_be = permutation.b1 ?
                "observationFilePath" :
                permutation.b2 ?
                    "commentFilePath" :
                    permutation.b3 ?
                        "observationIDCounter" :
                        "proposalFilePath";


            try
            {
                _ = new CSVDatabase(
                    permutation.b1 ? "" : "a",
                    permutation.b2 ? "" : "b",
                    permutation.b3 ? "" : "c",
                    permutation.b4 ? "" : "d"
                );
                throw new Exception("Should have failed by now!");
            }
            catch (ArgumentException e)
            {
                e.ParamName.Should().Be(should_be);
            }
        }

        foreach (var permutation in permutations)
        {
            if (!permutation.b1 && !permutation.b2 && !permutation.b3 && !permutation.b4)
                continue;

            try
            {
                _ = new CSVDatabase(
                    permutation.b1 ? "/" : "data.test",
                    permutation.b2 ? "/" : "data.test",
                    permutation.b3 ? "/" : "data.test",
                    permutation.b4 ? "/" : "data.test"
                );
                throw new Exception("Should have failed by now!");
            }
            catch (ArgumentException e)
            {
                e.ParamName.Should().Be("parent");
            }
        }
    }

    [Fact]
    public static void TestSimpleCounterInvalidFile()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "test");
        SimpleCounter simple = new(path);

        simple.NextNumber().Should().Be(0);

        File.WriteAllText(path, "test");

        simple.NextNumber().Should().Be(0);

        SavingRetrievingTest.DeleteFileIfExists(path);
    }

    [Fact]
    public static void TestSimpleCounterFile()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "-1");
        SimpleCounter simple = new(path);

        simple.NextNumber().Should().Be(0);

        File.WriteAllText(path, "5");
        simple = new(path);

        simple.NextNumber().Should().Be(6);

        SavingRetrievingTest.DeleteFileIfExists(path);
    }

    [Fact]
    public static void ReadObservationsWorksCorrectly()
    {
        var (db, finished) = SetupTestDatabase();

        var observations = db.ReadObservations().ToArray();
        observations.Length.Should().Be(5);

        finished();
    }

    [Fact]
    public static void ReadAllCommentsWorksCorrectly()
    {
        var (db, finished) = SetupTestDatabase();

        var commentsForObs1 = db.ReadCommentsForObservation(0).ToArray();
        commentsForObs1.Length.Should().Be(2);

        var commentsForObs2 = db.ReadCommentsForObservation(1).ToArray();
        commentsForObs2.Length.Should().Be(2);

        var commentsForObs3 = db.ReadCommentsForObservation(2).ToArray();
        commentsForObs3.Length.Should().Be(2);

        var commentsForObs4 = db.ReadCommentsForObservation(3).ToArray();
        commentsForObs4.Length.Should().Be(0);

        var commentsForObs5 = db.ReadCommentsForObservation(4).ToArray();
        commentsForObs5.Length.Should().Be(1);

        finished();
    }

    [Fact]
    public static void ReadCommentsForObservationWorksCorrectly()
    {
        var (db, finished) = SetupTestDatabase();

        var commentsForFirstObservation = db.ReadCommentsForObservation(0).ToArray();
        commentsForFirstObservation.Length.Should().Be(2);

        var commentsForSecondObservation = db.ReadCommentsForObservation(1).ToArray();
        commentsForSecondObservation.Length.Should().Be(2);

        var commentsForThirdObservation = db.ReadCommentsForObservation(2).ToArray();
        commentsForThirdObservation.Length.Should().Be(2);

        var commentsForFourthObservation = db.ReadCommentsForObservation(3).ToArray();
        commentsForFourthObservation.Length.Should().Be(0);

        var commentsForFifthObservation = db.ReadCommentsForObservation(4).ToArray();
        commentsForFifthObservation.Length.Should().Be(1);

        finished();
    }

    [Fact]
    public static void TaxonomiesCanGetByID()
    {
        var taxonomies = new Taxonomies();
        var taxon = taxonomies.GetTaxonRecordByID("MSTSNM:Arter:c18811f4-f785-ea11-aa77-501ac539d1ea");
        taxon.Should().NotBeNull();
        taxon.VernacularName.Should().Be("Sølvhejre");
    }

    [Fact]
    public static void TaxonomiesCanGetByName()
    {
        var taxonomies = new Taxonomies();
        var taxon = taxonomies.GetTaxonRecordByName("Sølvhejre");
        taxon.Should().NotBeNull();
        taxon.TaxonID.Should().Be("MSTSNM:Arter:c18811f4-f785-ea11-aa77-501ac539d1ea");
    }

    [Fact]
    public static void TaxonomiesCanGetSuper()
    {
        var taxonomies = new Taxonomies();
        var taxon = taxonomies.GetTaxonRecordByName("Sølvhejre");
        taxon.Should().NotBeNull();
        var parent = taxonomies.GetSuperTaxon(taxon);
        parent.Should().NotBeNull();
        parent.TaxonID.Should().Be("MSTSNM:Arter:7f9ef9f3-f785-ea11-aa77-501ac539d1ea");
    }

    [Fact]
    public static void PelecaniformesShouldNotHaveSuper()
    {
        var taxonomies = new Taxonomies();
        var taxon = taxonomies.GetTaxonRecordByName("Årefodede");
        taxon.Should().NotBeNull();
        var parent = taxonomies.GetSuperTaxon(taxon);
        parent.Should().BeNull();
    }

    [Fact]
    public static void TaxonomiesCanGetSubs()
    {
        var taxonomies = new Taxonomies();
        var taxon = taxonomies.GetTaxonRecordByName("Årefodede");
        taxon.Should().NotBeNull();
        var children = taxonomies.GetSubTaxons(taxon).ToList();
        children.Count.Should().Be(6);
    }

    [Fact]
    public static void SpeciesDoesNotHaveSubs()
    {
        var taxonomies = new Taxonomies();
        var taxon = taxonomies.GetTaxonRecordByName("Sølvhejre");
        taxon.Should().NotBeNull();
        var children = taxonomies.GetSubTaxons(taxon).ToList();
        children.Count.Should().Be(0);
    }
}
