namespace TFA.Domain.Exceptions;

public class ForumNotFoundException:Exception
{
    public ForumNotFoundException(Guid id):base($"Forum with id {id} was not found")
    {
        
    }
}
