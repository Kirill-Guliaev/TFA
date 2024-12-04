namespace TFA.Domain.Exceptions;

public class ForumNotFoundException: DomainException
{
    public ForumNotFoundException(Guid id):base(410, $"Forum with id {id} was not found")
    {
        
    }
}
