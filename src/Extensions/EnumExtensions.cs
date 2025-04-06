﻿using System;
using System.Collections.Generic;

namespace RhythmFramework.Extensions;

/// <summary>
/// An extension class for <see cref="Enum"/>.
/// </summary>
public static class EnumExtensions
{
    private struct ExtendedEnumData(int offset)
    {
        public readonly int ValueOffset = offset;
        public readonly List<string> Enums = new();
    }

    private static readonly Dictionary<Type, ExtendedEnumData> ExtendedData = new();

    /// <summary>
    /// ToString but for Extended enums. Only use on enums that are ints.
    /// </summary>
    /// <typeparam name="T">The enum type to search.</typeparam>
    /// <param name="en">The extended enum to get the string for.</param>
    /// <returns>The string name of the enum.</returns>
    public static string ToStringExtended<T>(this T en) where T : Enum
    {
        return GetExtendedName<T>(Convert.ToInt32(en));
    }

    /// <summary>
    /// Retrieves an array of the values of the constants in a specified enumeration.
    /// </summary>
    /// <typeparam name="T">The <see cref="Enum"/> to get values for.</typeparam>
    /// <returns>An array that contains the ints of <b>all</b> definitions in an <see cref="Enum"/>.</returns>
    public static int[] GetValues<T>() where T : Enum
    {
        List<int> possibleTypes = new List<int>();
        Array values = Enum.GetValues(typeof(T));
        for (int i = 0; i < values.Length; i++)
        {
            possibleTypes.Add((int)values.GetValue(i));
        }

        if (!ExtendedData.ContainsKey(typeof(T))) return possibleTypes.ToArray();
        ExtendedEnumData data = ExtendedData[typeof(T)];
        for (int i = 0; i < data.Enums.Count; i++)
        {
            possibleTypes.Add(data.ValueOffset + i);
        }

        return possibleTypes.ToArray();
    }


    /// <summary>
    /// Extends an enum, same effect could be achieved by casting an int, however this has a system to keep track of multiple enum additions from different mods to prevent conflicts
    /// </summary>
    /// <typeparam name="T">The enum to extend.</typeparam>
    /// <param name="extendName">What the enum would be called.</param>
    /// <returns>An enum with a name that is named after the string.</returns>
    public static T ExtendEnum<T>(string extendName) where T : Enum
    {
        if (!ExtendedData.TryGetValue(typeof(T), out ExtendedEnumData dat))
        {
            dat = new ExtendedEnumData(256); //Just so nothing conflicts and mods don't break when the game updates. 
            ExtendedData.Add(typeof(T), dat);
        }

        if (dat.Enums.Contains(extendName))
        {
            RhythmFrameworkPlugin.Logger.LogWarning("Attempted to register duplicate extended enum:" + extendName + "!");
            return GetFromExtendedName<T>(extendName);
        }

        dat.Enums.Add(extendName);
        return (T)(object)(dat.ValueOffset + (dat.Enums.Count - 1));
    }

    /// <summary>
    /// A helper function to check if an enum exists yet.
    /// </summary>
    /// <param name="extendName">The name to check.</param>
    /// <typeparam name="T">The <see cref="Enum"/> to search.</typeparam>
    /// <returns>A boolean denoting whether the enum exists.</returns>
    public static bool ExtendedEnumExists<T>(string extendName) where T : Enum
    {
        if (!ExtendedData.TryGetValue(typeof(T), out ExtendedEnumData dat)) return false; // This enum hasn't even been extended yet.
        return dat.Enums.Contains(extendName);
    }

    /// <summary>
    /// A version of Enum.GetName but with support for extended enums.
    /// </summary>
    /// <typeparam name="T">The enum to search.</typeparam>
    /// <param name="val">The enum offset.</param>
    /// <returns>A string of the enum's name.</returns>
	public static string GetExtendedName<T>(int val) where T : Enum
	{
		string? theName = Enum.GetName(typeof(T), val);
        if (theName == null)
		{
            if (ExtendedData.TryGetValue(typeof(T), out ExtendedEnumData dat))
                return dat.Enums[val - dat.ValueOffset];
            
			return val.ToString();
        }
		return theName;

    }

    /// <summary>
    /// Get an extended enum from a name.
    /// </summary>
    /// <param name="name">The name of the specific enum.</param>
    /// <typeparam name="T">The enum to search.</typeparam>
    /// <returns>An enum of type T with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Throws when an enum with the target name doesn't exist or the enum hasn't been extended.</exception>
    public static T GetFromExtendedName<T>(string name) where T : Enum
    {
        if (Enum.IsDefined(typeof(T), name))
            return (T)Enum.Parse(typeof(T), name);
        
        bool success = ExtendedData.TryGetValue(typeof(T), out ExtendedEnumData value);
        if (!success) throw new KeyNotFoundException();
        
        int index = value.Enums.FindIndex(x => x == name);
        if (index == -1) throw new KeyNotFoundException();
        
        return (T)(object)(value.ValueOffset + index);
    }
}