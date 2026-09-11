namespace Bison.CLI.Client.Tests;

using Bison.Database;
using Bison.Models;
using Bison.Utilities;

using FluentAssertions;

public class UnitTests
{
    static (CSVDatabase<ObservationRecord>, CSVDatabase<CommentRecord>, SimpleCounter) SetupTestDatabase()
    {
        if (Directory.Exists("data/test"))
        {
            DirectoryInfo di = new("data/test");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        CSVDatabase<ObservationRecord> observationDB = CSVDatabase<ObservationRecord>.GetInstance("data/test/obs_db.csv");
        CSVDatabase<CommentRecord> commentDB = CSVDatabase<CommentRecord>.GetInstance("data/test/com_db.csv");
        SimpleCounter observationIdCounter = new("data/test/obs_id_db.txt");

        observationDB.Store(
            new ObservationRecord
            {
                Id = observationIdCounter.NextNumber(),
                Author = "lrec",
                Observation = "Eurasien jay",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        commentDB.Store(
            new CommentRecord
            {
                ObservationId = 0,
                Author = "mawb",
                Comment = "I do think it's a jay of somekind",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        commentDB.Store(
            new CommentRecord
            {
                ObservationId = 0,
                Author = "lrec",
                Comment = "I think it might be a bird",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        observationDB.Store(
            new ObservationRecord
            {
                Id = observationIdCounter.NextNumber(),
                Author = "mawb",
                Observation = "Ghost",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        commentDB.Store(
            new CommentRecord
            {
                ObservationId = 1,
                Author = "pask",
                Comment = "You should be medicated",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        commentDB.Store(
            new CommentRecord
            {
                ObservationId = 1,
                Author = "mawb",
                Comment = "Well yes, but for a different reason",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        observationDB.Store(
            new ObservationRecord
            {
                Id = observationIdCounter.NextNumber(),
                Author = "toov",
                Observation = "Magnus",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        commentDB.Store(
            new CommentRecord
            {
                ObservationId = 2,
                Author = "mawb",
                Comment = "Please don't observe me like this.",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        commentDB.Store(
            new CommentRecord
            {
                ObservationId = 2,
                Author = "toov",
                Comment = "ok ig",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        observationDB.Store(
            new ObservationRecord
            {
                Id = observationIdCounter.NextNumber(),
                Author = "pask",
                Observation = "Saw a group of crows",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );

        observationDB.Store(
            new ObservationRecord
            {
                Id = observationIdCounter.NextNumber(),
                Author = "lrec",
                Observation = "Peanut Butter Baby",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        commentDB.Store(
            new CommentRecord
            {
                ObservationId = 4,
                Author = "toov",
                Comment = "Ahh",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        return (observationDB, commentDB, observationIdCounter);
    }

    [Fact]
    public static void UnitTest()
    {
        var (observationDB, commentDB, observationIdCounter) = SetupTestDatabase();

        var commentsForFirstObservation = Program.FilterComments(0, commentDB.Read()).ToArray();
        commentsForFirstObservation.Length.Should().Be(2);
        var commentsForSecondObservation = Program.FilterComments(1, commentDB.Read()).ToArray();
        commentsForSecondObservation.Length.Should().Be(2);
        var commentsForThirdObservation = Program.FilterComments(2, commentDB.Read()).ToArray();
        commentsForThirdObservation.Length.Should().Be(2);
        var commentsForFourthObservation = Program.FilterComments(3, commentDB.Read()).ToArray();
        commentsForFourthObservation.Length.Should().Be(0);
        var commentsForFifthObservation = Program.FilterComments(4, commentDB.Read()).ToArray();
        commentsForFifthObservation.Length.Should().Be(1);
    }

    [Fact]
    public void CommentsOnInvalidObservationsShouldFail()
    {
        var output = Program.TryComment(2147483647, "Test comment");

        output.Should().Be("Observation id 2147483647 does not exist");
    }
}
