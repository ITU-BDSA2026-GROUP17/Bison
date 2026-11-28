namespace Bison.Database.Exceptions;

public class PostDoesNotExistException(int postId) : Exception($"Post with ID {postId} does not exist")
{
    public readonly int PostId = postId;
}
