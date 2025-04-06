using System.Collections.Generic;

namespace RhythmFramework.Ranges;

/// <summary>
/// An interface for range generators.
/// </summary>
/// <typeparam name="T">The type this range generators.</typeparam>
public interface IRange<out T>
{
    /// <summary>
    /// Creates a range of values of the specified type.
    /// </summary>
    /// <returns>A list of values.</returns>
    IEnumerable<T> AsEnumerable();
}