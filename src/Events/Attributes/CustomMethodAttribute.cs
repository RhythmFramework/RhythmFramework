using System;

namespace RhythmFramework.Events.Attributes;

/// <summary>
/// An attribute that allows you to register the decorated function to the CallCustomMethod event.
/// An easier alternative to creating custom events for those who don't want to do the effort.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class CustomMethodAttribute(MethodInvocation invocation = MethodInvocation.Both): Attribute
{
    /// <summary>
    /// Specifies when this event will run.
    /// </summary>
    public MethodInvocation Invocation { get; } = invocation;
}

/// <summary>
/// An enum denoting when a custom event should run.
/// </summary>
public enum MethodInvocation
{
    /// <summary>
    /// This function will run both on PreBar and on Bar.
    /// </summary>
    Both,
    
    /// <summary>
    /// This function will only run on PreBar.
    /// </summary>
    OnlyPreBar,
    
    /// <summary>
    /// This function will only run on Bar.
    /// </summary>
    OnlyOnBar,
}