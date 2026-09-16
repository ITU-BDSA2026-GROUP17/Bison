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
        CSVDatabase db = new(obsPath, comPath, obsIdPath);

        db.StoreObservation(
            new ObservationRecord
            {
                Author = "lrec",
                Observation = "Eurasien jay",
                Location = "Assistentens Kirkegård",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        db.StoreComment(
            new CommentRecord
            {
                ObservationId = 0,
                Author = "mawb",
                Comment = "I do think it's a jay of somekind",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        db.StoreComment(
            new CommentRecord
            {
                ObservationId = 0,
                Author = "lrec",
                Comment = "I think it might be a bird",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        db.StoreObservation(
            new ObservationRecord
            {
                Author = "mawb",
                Observation = "Ghost",
                Location = "Here",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        db.StoreComment(
            new CommentRecord
            {
                ObservationId = 1,
                Author = "pask",
                Comment = "You should be medicated",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        db.StoreComment(
            new CommentRecord
            {
                ObservationId = 1,
                Author = "mawb",
                Comment = "Well yes, but for a different reason",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        db.StoreObservation(
            new ObservationRecord
            {
                Author = "toov",
                Observation = "Magnus",
                Location = "ITU",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        db.StoreComment(
            new CommentRecord
            {
                ObservationId = 2,
                Author = "mawb",
                Comment = "Please don't observe me like this.",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        db.StoreComment(
            new CommentRecord
            {
                ObservationId = 2,
                Author = "toov",
                Comment = "ok ig",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        db.StoreObservation(
            new ObservationRecord
            {
                Author = "pask",
                Observation = "Saw a group of crows",
                Location = "Tivoli",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );

        db.StoreObservation(
            new ObservationRecord
            {
                Author = "lrec",
                Observation = "Peanut Butter Baby",
                Location = "The Table",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        db.StoreComment(
            new CommentRecord
            {
                ObservationId = 4,
                Author = "toov",
                Comment = "Ahh",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
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

        try
        {
            _ = new CSVDatabase("a", "b", "");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("observationIDCounter");
        }

        try
        {
            _ = new CSVDatabase("a", "", "c");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("commentFilePath");
        }

        try
        {
            _ = new CSVDatabase("", "b", "c");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("observationFilePath");
        }

        try
        {
            _ = new CSVDatabase("", "", "c");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("observationFilePath");
        }

        try
        {
            _ = new CSVDatabase("", "b", "");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("observationFilePath");
        }

        try
        {
            _ = new CSVDatabase("a", "", "");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("commentFilePath");
        }

        try
        {
            _ = new CSVDatabase("", "", "");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("observationFilePath");
        }

        try
        {
            _ = new CSVDatabase("/", "data.test", "data.test");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("parent");
        }

        try
        {
            _ = new CSVDatabase("data.test", "/", "data.test");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("parent");
        }

        try
        {
            _ = new CSVDatabase("data.test", "data.test", "/");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("parent");
        }


        try
        {
            _ = new CSVDatabase("data.test", "/", "/");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("parent");
        }

        try
        {
            _ = new CSVDatabase("/", "data.test", "/");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("parent");
        }

        try
        {
            _ = new CSVDatabase("/", "/", "data.test");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("parent");
        }

        try
        {
            _ = new CSVDatabase("/", "/", "/");
            throw new Exception("Should have failed by now!");
        }
        catch (ArgumentException e)
        {
            e.ParamName.Should().Be("parent");
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
}
