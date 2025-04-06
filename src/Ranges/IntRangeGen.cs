using System;
using System.Collections.Generic;

namespace RhythmFramework.Ranges;

/// <summary>
/// A range generator for ints.
/// </summary>
public class IntRangeGen: IRange<int>
{
    private int start;
    private int endInclusive;
    private int step;

    /// <summary>
    /// Instantiates a <see cref="IntRangeGen"/>.
    /// </summary>
    /// <param name="start">The float to start.</param>
    /// <param name="endInclusive">The int to end at. (Is included in the final list).</param>
    /// <param name="step">The increment of each value until the end.</param>
    public IntRangeGen(int start, int endInclusive, int step = 1)
    {
        this.start = start;
        this.endInclusive = endInclusive;
        this.step = step;
    }

    /// <summary>
    /// Converts the range into a list of ints.
    /// </summary>
    /// <returns>A list of ints from a specified start to the end.</returns>
    public IEnumerable<int> AsEnumerable()
    {
        List<int> values = new();
        for (int i = start; i <= endInclusive; i += step) values.Add(Convert.ToInt32(Math.Round(Convert.ToDecimal(i), 2)));
        return values;
    }
}