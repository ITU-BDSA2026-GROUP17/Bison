using Bison.Models;

using FluentAssertions;

using FsCheck;
using FsCheck.Xunit;

namespace Bison.Database.Tests;

public class SavingRetrievingTest
{
    static void CreateFileIfNotExists(string filePath)
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

    static void DeleteFileIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public static void SavingAndRetreivingWorksIfFileExists()
    {
        CreateFileIfNotExists("data/test/saving_test.csv");
        CreateFileIfNotExists("data/test/saving_test.txt");
        CSVDatabase<ObservationRecord> observationDB = CSVDatabase<ObservationRecord>.GetInstance("data/test/saving_test.csv");

        SimpleCounter obsIdCounter = new("data/test/saving_test.txt");
        var observation = new ObservationRecord
        {
            Id = obsIdCounter.NextNumber(),
            Author = "Test Person 1",
            Observation = "WOAH WOAH",
            Location = "Nowhere",
            Timestamp = 0 // January 1st, 1970 at 00:00:00 UTC
        };

        observationDB.Store(observation);

        observationDB.Read().Any(obs => obs.Equals(observation)).Should().BeTrue();
    }

    [Fact]
    public static void SavingAndRetreivingWorksIfFileDoesNotExists()
    {
        DeleteFileIfExists("data/test/saving_test.csv");
        DeleteFileIfExists("data/test/saving_test.txt");
        CSVDatabase<ObservationRecord> observationDB = CSVDatabase<ObservationRecord>.GetInstance("data/test/saving_test.csv");
        observationDB.Read().Count().Should().Be(0);

        SimpleCounter obsIdCounter = new("data/test/saving_test.txt");
        var observation = new ObservationRecord
        {
            Id = obsIdCounter.NextNumber(),
            Author = "Test Person 1",
            Observation = "WOAH WOAH",
            Location = "Nowhere",
            Timestamp = 0 // January 1st, 1970 at 00:00:00 UTC
        };

        observationDB.Store(observation);

        var arr = observationDB.Read().ToArray();
        arr[0].Should().Be(observation);
    }

    [Fact]
    public static void SavingAndRetreivingWorksForMultipleItems()
    {
        DeleteFileIfExists("data/test/saving_test.csv");
        DeleteFileIfExists("data/test/saving_test.txt");
        CSVDatabase<ObservationRecord> observationDB = CSVDatabase<ObservationRecord>.GetInstance("data/test/saving_test.csv");
        observationDB.Read().Count().Should().Be(0);

        SimpleCounter obsIdCounter = new("data/test/saving_test.txt");
        var obs1 = new ObservationRecord
        {
            Id = obsIdCounter.NextNumber(),
            Author = "Test Person 1",
            Observation = "WOAH WOAH",
            Location = "Nowhere",
            Timestamp = 0 // January 1st, 1970 at 00:00:00 UTC
        };
        var obs2 = new ObservationRecord
        {
            Id = obsIdCounter.NextNumber(),
            Author = "Test Person 2",
            Observation = "A heron!!",
            Location = "By DR Byen",
            Timestamp = 1788264000 // September 1st, 2026 at 12:00:00 UTC
        };

        observationDB.Store(obs1);
        observationDB.Store(obs2);

        var arr = observationDB.Read().ToArray();
        arr[0].Should().Be(obs1);
        arr[1].Should().Be(obs2);
    }

    [Property]
    public static void SavingAndRetreivingWorksForRandomData(NonEmptyString author, NonEmptyString observation, NonEmptyString location)
    {
        DeleteFileIfExists("data/test/saving_test.csv");
        DeleteFileIfExists("data/test/saving_test.txt");
        CSVDatabase<ObservationRecord> observationDB = CSVDatabase<ObservationRecord>.GetInstance("data/test/saving_test.csv");
        observationDB.Read().Count().Should().Be(0);

        SimpleCounter obsIdCounter = new("data/test/saving_test.txt");
        var obs = new ObservationRecord
        {
            Id = obsIdCounter.NextNumber(),
            Author = author.ToString(),
            Observation = observation.ToString(),
            Location = location.ToString(),
            Timestamp = 0 // January 1st, 1970 at 00:00:00 UTC
        };

        observationDB.Store(obs);

        var arr = observationDB.Read().ToArray();
        arr[0].Should().Be(obs);
    }
}
