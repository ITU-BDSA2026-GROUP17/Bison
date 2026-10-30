namespace Bison.Database.Exceptions;

public class UserDoesNotExistException(string userId) : Exception($"User with ID {userId} does not exist")
{
    public readonly string UserId = userId;
}
