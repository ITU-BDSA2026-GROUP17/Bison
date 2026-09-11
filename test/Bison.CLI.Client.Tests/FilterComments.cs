namespace Bison.CLI.Client.Tests;

using Bison.CLI.Client;
using Bison.Models;
using Bison.Utilities;

using FluentAssertions;

public class FilterComments
{
    static UserInterface SetupTestDatabase()
    {
        UserInterface userInterface = new(
            "data/test/obs_db.csv",
            "data/test/com_db.csv",
            "data/test/obs_id_db.txt"
        );

        userInterface.ObservationDB.Store(
            new ObservationRecord
            {
                Id = userInterface.ObservationIdCounter.NextNumber(),
                Author = "lrec",
                Observation = "Eurasien jay",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        userInterface.CommentDB.Store(
            new CommentRecord
            {
                ObservationId = 0,
                Author = "mawb",
                Comment = "I do think it's a jay of somekind",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        userInterface.CommentDB.Store(
            new CommentRecord
            {
                ObservationId = 0,
                Author = "lrec",
                Comment = "I think it might be a bird",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        
        userInterface.ObservationDB.Store(
            new ObservationRecord
            {
                Id = userInterface.ObservationIdCounter.NextNumber(),
                Author = "mawb",
                Observation = "Ghost",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        userInterface.CommentDB.Store(
            new CommentRecord
            {
                ObservationId = 1,
                Author = "pask",
                Comment = "You should be medicated",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        userInterface.CommentDB.Store(
            new CommentRecord
            {
                ObservationId = 1,
                Author = "mawb",
                Comment = "Well yes, but for a different reason",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        
        userInterface.ObservationDB.Store(
            new ObservationRecord
            {
                Id = userInterface.ObservationIdCounter.NextNumber(),
                Author = "toov",
                Observation = "Magnus",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        userInterface.CommentDB.Store(
            new CommentRecord
            {
                ObservationId = 2,
                Author = "mawb",
                Comment = "Please don't observe me like this.",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        userInterface.CommentDB.Store(
            new CommentRecord
            {
                ObservationId = 2,
                Author = "toov",
                Comment = "ok ig",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );
        
        userInterface.ObservationDB.Store(
            new ObservationRecord
            {
                Id = userInterface.ObservationIdCounter.NextNumber(),
                Author = "pask",
                Observation = "Saw a group of crows",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        
        userInterface.ObservationDB.Store(
            new ObservationRecord
            {
                Id = userInterface.ObservationIdCounter.NextNumber(),
                Author = "lrec",
                Observation = "Peanut Butter Baby",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now),
            }
        );
        userInterface.CommentDB.Store(
            new CommentRecord
            {
                ObservationId = 4,
                Author = "toov",
                Comment = "Ahh",
                Timestamp = DateTimeUtilities.DateTimeToUnixTimeStamp(DateTime.Now)
            }
        );

        return userInterface;
    }

    [Fact]
    public static void UnitTest()
    {
        var ui = SetupTestDatabase();

        var commentsForFirstObservation = UserInterface.FilterComments(0, ui.CommentDB.Read()).ToArray();
        commentsForFirstObservation.Length.Should().Be(2);
        var commentsForSecondObservation = UserInterface.FilterComments(1, ui.CommentDB.Read()).ToArray();
        commentsForSecondObservation.Length.Should().Be(2);
        var commentsForThirdObservation = UserInterface.FilterComments(2, ui.CommentDB.Read()).ToArray();
        commentsForThirdObservation.Length.Should().Be(2);
        var commentsForFourthObservation = UserInterface.FilterComments(3, ui.CommentDB.Read()).ToArray();
        commentsForFourthObservation.Length.Should().Be(0);
        var commentsForFifthObservation = UserInterface.FilterComments(4, ui.CommentDB.Read()).ToArray();
        commentsForFifthObservation.Length.Should().Be(1);
    }
}
