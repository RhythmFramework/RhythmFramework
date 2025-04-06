namespace RhythmFramework.Events.Exception;

/// <summary>
/// An exception that is thrown when there are more than 1 custom methods.
/// </summary>
/// <param name="methodName">The name of the custom method.</param>
public class DuplicateCustomMethodException(string methodName) : System.Exception($"{methodName} is already registered as a custom method. It cannot be registered twice.");