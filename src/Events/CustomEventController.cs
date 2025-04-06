using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using RDLevelEditor;
using RhythmFramework.Extensions;

namespace RhythmFramework.Events;

/// <summary>
/// A controller for handling custom events.
/// </summary>
public static class CustomEventController
{
    // RDLevelEditor.SelectLevelEventPanel
    // RDLevelEditor.TabSection_Actions
    internal static readonly Dictionary<string, Type> CustomEvents = new();
    internal static readonly List<Assembly> RegisteredAssemblies = new();

    internal static void AddCustomEventsToList(List<string> events)
    {
        CustomEvents.Keys.ForEach(events.Add);
    }
    
    /// <summary>
    /// Check an <see cref="Assembly"/> for custom events and register them.
    /// </summary>
    /// <param name="assembly">The assembly to register.</param>
    public static void Register(Assembly assembly)
    {
        if (RegisteredAssemblies.Contains(assembly)) return;

        RegisteredAssemblies.Add(assembly);
        assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(CustomEvent)))
            .ForEach(RegisterCustomEvent);
    }

    private static void RegisterCustomEvent(Type eventType)
    {
        if (eventType.GetCustomAttribute<LevelEventInfoAttribute>() == null) 
            throw new NotSupportedException($"{eventType.FullName} does not have a LevelInfoAttribute! This is required for RD to parse the event correctly.");

        string eventName = eventType.Name.Replace("LevelEvent_", ""); // Remove start.
        CustomEvents[eventName] = eventType;
    }

    /// <summary>
    /// Get a <see cref="Type"/> from an event name.
    /// Can be used to see if an event already exists to prevent overriding.
    /// </summary>
    /// <param name="eventName">The name to search.</param>
    /// <returns>A <see cref="Type"/> or null.</returns>
    public static Type? EventTypeFromName(string eventName)
    {
        if (CustomEvents.TryGetValue(eventName, out Type customEventType)) return customEventType;
        return null;
    }
}