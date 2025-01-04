namespace TFA.Domain.Authorization;

public interface IIntentionManager
{
    bool IsAllowed<TIntetion>(TIntetion intetion) where TIntetion : struct;
}

internal static class IntetionManagerExtension
{
    public static void ThrowIfForbidden<TIntetion>(this IIntentionManager intentionManager, TIntetion intetion)
        where TIntetion:struct
    {
        if(!intentionManager.IsAllowed(intetion))
        {
            throw new IntentionManagerException();
        }
    }
}