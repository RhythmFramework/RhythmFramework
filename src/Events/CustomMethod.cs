using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using RDLevelEditor;
using RhythmFramework.Extensions;
using RhythmFramework.Events.Attributes;
using RhythmFramework.Events.Exception;

namespace RhythmFramework.Events;

internal static class CustomMethod
{
    internal static readonly Dictionary<string, (MethodInfo, CustomMethodAttribute)> RegisteredMethods = new();
    
    internal static void RegisterCustomMethods(Assembly assembly)
    {
        var decoratedMethods = assembly.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            .Where(m => m.GetCustomAttribute<CustomMethodAttribute>() != null)
            .Select(m => new { Method = m, Attribute = m.GetCustomAttribute<CustomMethodAttribute>() })
            .ToArray();
        
        decoratedMethods.ForEach(kvp =>
        {
            if (!kvp.Method.IsStatic) throw new ArgumentException($"Unable to Register Method {kvp.Method.Name}. Reason: Method of event must be static.");
            if (RegisteredMethods.ContainsKey(kvp.Method.Name)) throw new DuplicateCustomMethodException(kvp.Method.Name);
            RegisteredMethods[kvp.Method.Name] = (kvp.Method, kvp.Attribute!);
        });
    }
}