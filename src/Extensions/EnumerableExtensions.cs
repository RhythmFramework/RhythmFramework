using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RhythmFramework.Extensions;

/// <summary>
/// An extension class for <see cref="IEnumerable{T}"/>
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Executes an action for each element in this sequence.
    /// </summary>
    /// <param name="source">A sequence of values.</param>
    /// <param name="action">The action to execute on each element.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <exception cref="ArgumentNullException">source or mapper functions are null.</exception>
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (action == null) throw new ArgumentNullException(nameof(action));
        IEnumerator<TSource> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
            action(enumerator.Current);
        enumerator.Dispose();
    }

    /// <summary>
    /// Executes an action for each element in this sequence.
    /// </summary>
    /// <param name="source">A sequence of values.</param>
    /// <param name="action">The action to execute on each element.</param>
    /// <typeparam name="TSource">The type of elements of the source.</typeparam>
    /// <exception cref="ArgumentNullException">source or mapper functions are null.</exception>
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (action == null) throw new ArgumentNullException(nameof(action));
        int i = 0;
        IEnumerator<TSource> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
            action(enumerator.Current, i++);
        enumerator.Dispose();
    }  
}