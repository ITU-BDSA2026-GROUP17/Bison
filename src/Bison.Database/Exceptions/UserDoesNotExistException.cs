namespace Bison.Database.Exceptions;

public class UserDoesNotExistException(int userId) : Exception($"User with ID {userId} does not exist")
{
    public readonly int UserId = userId;
}
