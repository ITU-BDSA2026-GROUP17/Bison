namespace Bison.CLI.Client.Tests;

using FluentAssertions;

public class CommentObservationsTest
{

    [Fact]
    public void CommentsOnInvalidObservationsShouldFail()
    {
        var output = Program.TryComment(2147483647, "Test comment");

        output.Should().Be("Observation id 2147483647 does not exist");
    }
}
