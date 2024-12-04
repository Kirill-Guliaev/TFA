namespace TFA.Domain.Authorization;

public class IntentionManagerException : Exception
{
    public IntentionManagerException() : base("Intention not allowed")
    {
    }
}
