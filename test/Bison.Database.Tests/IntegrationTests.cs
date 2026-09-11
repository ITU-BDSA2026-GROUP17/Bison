using Bison.Models;

using FluentAssertions;

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

    static void DeleteFileIfExists(string filePath) {
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
        CSVDatabase<ObservationRecord> observationDB = new("data/test/saving_test.csv");
        SimpleCounter obsIdCounter = new("data/test/saving_test.txt");
        var observation = new ObservationRecord
        {
            Id = obsIdCounter.NextNumber(),
            Author = "Test Person 1",
            Observation = "WOAH WOAH",
            Timestamp = 0 // January 1st, 1970 at 00:00:00
        };

        observationDB.Store(observation);

        observationDB.Read().Any(obs => obs.Equals(observation)).Should().BeTrue();
    }

    [Fact]
    public static void SavingAndRetreivingWorksIfFileDoesNotExists() {
        DeleteFileIfExists("data/test/saving_test.csv");
        DeleteFileIfExists("data/test/saving_test.txt");
        
        CSVDatabase<ObservationRecord> observationDB = new("data/test/saving_test.csv");
        observationDB.Read().Count().Should().Be(0);
        
        SimpleCounter obsIdCounter = new("data/test/saving_test.txt");
        var observation = new ObservationRecord
        {
            Id = obsIdCounter.NextNumber(),
            Author = "Test Person 1",
            Observation = "WOAH WOAH",
            Timestamp = 0 // January 1st, 1970 at 00:00:00
        };

        observationDB.Store(observation);

        observationDB.Read().Any(obs => obs.Equals(observation)).Should().BeTrue();
    }
}
