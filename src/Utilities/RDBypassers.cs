using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using nn.err;

namespace RhythmFramework.Utilities;

/// <summary>
/// A utils class for using Reflection to gain access to private variables.
/// </summary>
// ReSharper disable InconsistentNaming
public static class RDBypasses
{
    // RD devs love their private fields. A lot of things are private when they don't really need to be 💀.
    // I'd love if they made them public.
    
    /// <summary>
    /// Return the <see cref="FieldInfo"/> of a private variable.
    /// </summary>
    /// <param name="instanceType">The type to search.</param>
    /// <param name="variableName">The name of the target variable.</param>
    /// <param name="includeStatic">Whether to check static fields.</param>
    /// <returns>The variable's <see cref="FieldInfo"/> or null.</returns>
    public static FieldInfo BypassPrivateVariable(Type instanceType, string variableName, bool includeStatic = false)
    {
        BindingFlags bindingFlags = BindingFlags.NonPublic | (includeStatic ? BindingFlags.Static : BindingFlags.Instance);
        FieldInfo? privateField = instanceType.GetField(variableName, bindingFlags);
        if (privateField == null)
        {
            RhythmFrameworkPlugin.Logger.LogWarning($"Could not find {variableName} in {instanceType.FullName}");
            RhythmFrameworkPlugin.Logger.LogWarning($"Stack: {new StackTrace(3).ToString()}");
            return null!;
        } 
        return privateField;
    }        
    
    /// <summary>
    /// Return the <see cref="MethodInfo"/> of a private function.
    /// </summary>
    /// <param name="instanceType">The type to search.</param>
    /// <param name="methodName">The name of the target method.</param>
    /// <param name="includeStatic">Whether to check static methods.</param>
    /// <returns>The method's <see cref="MethodInfo"/> or null.</returns>
    public static MethodInfo BypassPrivateMethod(Type instanceType, string methodName, bool includeStatic = false)
    {
        BindingFlags bindingFlags = BindingFlags.NonPublic | (includeStatic ? BindingFlags.Static : BindingFlags.Instance);
        MethodInfo? privateField = instanceType.GetMethod(methodName, bindingFlags);
        if (privateField == null)
        {
            RhythmFrameworkPlugin.Logger.LogWarning($"Could not find {methodName} in {instanceType.FullName}");
            RhythmFrameworkPlugin.Logger.LogWarning($"Stack Trace: {new StackTrace(3, true).ToString()}");
            return null!;
        } 
        return privateField;
    } 
    
    /// <summary>
    /// Return the <see cref="Type"/> of a private enum.
    /// </summary>
    /// <param name="declaringType">The type to search.</param>
    /// <param name="enumName">The name of the target enum.</param>
    /// <returns>The enum's <see cref="Type"/> or null.</returns>
    public static Type BypassPrivateEnum(Type declaringType, string enumName)
    {
        // Get the FieldInfo for the enum
        Type? enumField = declaringType.GetNestedType(enumName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        if (enumField != null) return enumField;
        
        RhythmFrameworkPlugin.Logger.LogWarning($"Could not find enum '{enumName}' in type '{declaringType.FullName}'.");
        RhythmFrameworkPlugin.Logger.LogWarning($"Stack Trace: {new StackTrace(3, true).ToString()}");
        return null!;
    }

    /// <summary>
    /// Change the value of a readonly field.
    /// </summary>
    /// <param name="target">The object to change the value for.</param>
    /// <param name="fieldName">The name of the readonly variable.</param>
    /// <param name="newValue">The changed value of the field.</param>
    public static void EditReadonlyField(object target, string fieldName, object newValue)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (field == null)
        {
            RhythmFrameworkPlugin.Logger.LogError($"Field '{fieldName}' not found!");
            return;
        }

        // Remove readonly constraint
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            field.SetValue(target, newValue);
        }
        catch (FieldAccessException)
        {
            // Use Unsafe operations to bypass readonly constraint
            var handle = field.FieldHandle;
            var type = Type.GetType("System.RuntimeFieldHandle", true)!;
            var method = type.GetMethod("SetValueDirect", BindingFlags.Instance | BindingFlags.NonPublic);

            method?.Invoke(handle, [target, newValue]);
        }
    }
}