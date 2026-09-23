using Bison.Models;

using FluentAssertions;

using FsCheck;
using FsCheck.Xunit;

namespace Bison.Database.Tests;

public class SavingRetrievingTest
{
    internal static void CreateFileIfNotExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            return;
        }

        var dir = Path.GetDirectoryName(filePath);
        ArgumentNullException.ThrowIfNull(dir);
        Directory.CreateDirectory(dir);
        File.Create(filePath);
    }

    internal static void DeleteFileIfExists(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    [Fact]
    public static void SavingAndRetrievingWorksIfFileExists()
    {
        var obsPath = Path.GetTempFileName();
        var comPath = Path.GetTempFileName();
        var obsIdPath = Path.GetTempFileName();
        var permutationPath = Path.GetTempFileName();
        CSVDatabase db = new(obsPath, comPath, obsIdPath, permutationPath);

        var observation = new ObservationRecord
        {
            Author = "Test Person 1",
            Observation = "WOAH WOAH",
            Location = "Nowhere",
            Timestamp = 0 // January 1st, 1970 at 00:00:00 UTC
        };

        observation.Id = db.StoreObservation(observation);

        db.ReadObservations().Any(obs => obs.Equals(observation)).Should().BeTrue();
        DeleteFileIfExists(obsPath);
        DeleteFileIfExists(comPath);
        DeleteFileIfExists(obsIdPath);
        DeleteFileIfExists(permutationPath);
    }

    [Fact]
    public static void SavingAndRetrievingWorksIfFileDoesNotExists()
    {
        var obsPath = Path.GetTempFileName();
        var comPath = Path.GetTempFileName();
        var obsIdPath = Path.GetTempFileName();
        var permutationPath = Path.GetTempFileName();
        DeleteFileIfExists(obsPath);
        DeleteFileIfExists(comPath);
        DeleteFileIfExists(obsIdPath);
        CSVDatabase db = new(obsPath, comPath, obsIdPath, permutationPath);
        db.ReadObservations().Count().Should().Be(0);

        var observation = new ObservationRecord
        {
            Author = "Test Person 1",
            Observation = "WOAH WOAH",
            Location = "Nowhere",
            Timestamp = 0 // January 1st, 1970 at 00:00:00 UTC
        };

        observation.Id = db.StoreObservation(observation);

        var arr = db.ReadObservations().ToArray();
        arr[0].Should().Be(observation);

        DeleteFileIfExists(obsPath);
        DeleteFileIfExists(comPath);
        DeleteFileIfExists(obsIdPath);
        DeleteFileIfExists(permutationPath);
    }

    [Fact]
    public static void SavingAndRetrievingWorksForMultipleItems()
    {
        var obsPath = Path.GetTempFileName();
        var comPath = Path.GetTempFileName();
        var obsIdPath = Path.GetTempFileName();
        var permutationPath = Path.GetTempFileName();
        CSVDatabase db = new(obsPath, comPath, obsIdPath, permutationPath);
        db.ReadObservations().Count().Should().Be(0);

        var obs1 = new ObservationRecord
        {
            Author = "Test Person 1",
            Observation = "WOAH WOAH",
            Location = "Nowhere",
            Timestamp = 0 // January 1st, 1970 at 00:00:00 UTC
        };
        var obs2 = new ObservationRecord
        {
            Author = "Test Person 2",
            Observation = "A heron!!",
            Location = "By DR Byen",
            Timestamp = 1788264000 // September 1st, 2026 at 12:00:00 UTC
        };

        obs1.Id = db.StoreObservation(obs1);
        obs2.Id = db.StoreObservation(obs2);

        var arr = db.ReadObservations().ToArray();
        arr[0].Should().Be(obs1);
        arr[1].Should().Be(obs2);

        DeleteFileIfExists(obsPath);
        DeleteFileIfExists(comPath);
        DeleteFileIfExists(obsIdPath);
        DeleteFileIfExists(permutationPath);
    }

    [Property]
    public static void SavingAndRetrievingWorksForRandomData(NonEmptyString author, NonEmptyString observation, NonEmptyString location)
    {
        var obsPath = Path.GetTempFileName();
        var comPath = Path.GetTempFileName();
        var obsIdPath = Path.GetTempFileName();
        var permutationPath = Path.GetTempFileName();
        CSVDatabase db = new(obsPath, comPath, obsIdPath, permutationPath);
        db.ReadObservations().Count().Should().Be(0);

        var obs = new ObservationRecord
        {
            Author = author.ToString(),
            Observation = observation.ToString(),
            Location = location.ToString(),
            Timestamp = 0 // January 1st, 1970 at 00:00:00 UTC
        };

        obs.Id = db.StoreObservation(obs);

        var arr = db.ReadObservations().ToArray();
        arr[0].Should().Be(obs);

        DeleteFileIfExists(obsPath);
        DeleteFileIfExists(comPath);
        DeleteFileIfExists(obsIdPath);
        DeleteFileIfExists(permutationPath);
    }

    [Property]
    public static void CommentsOnInvalidObservationsShouldFail(int observationId)
    {
        var obsPath = Path.GetTempFileName();
        var comPath = Path.GetTempFileName();
        var obsIdPath = Path.GetTempFileName();
        var permutationPath = Path.GetTempFileName();
        CSVDatabase db = new(obsPath, comPath, obsIdPath, permutationPath);
        db.ReadObservations().Count().Should().Be(0);

        try
        {
            db.StoreComment(new CommentRecord()
            {
                Author = "Test Person",
                Comment = "Test Comment",
                ObservationId = observationId,
                Timestamp = 0
            });
        }
        catch (ObservationDoesNotExist e)
        {
            e.Message.Should().Be($"Observation with id {observationId} does not exist");
        }

        db.ReadCommentsForObservation(observationId).Count().Should().Be(0);

        DeleteFileIfExists(obsPath);
        DeleteFileIfExists(comPath);
        DeleteFileIfExists(obsIdPath);
        DeleteFileIfExists(permutationPath);
    }
}
