using System;
using System.Collections.Generic;


namespace RhythmFramework.Ranges;

/// <summary>
/// A range generator for floats.
/// </summary>
public class FloatRangeGen: IRange<float>
{
    private float start;
    private float end;
    private float step;

    /// <summary>
    /// Instantiates a <see cref="FloatRangeGen"/>.
    /// </summary>
    /// <param name="start">The float to start.</param>
    /// <param name="end">The float to end.</param>
    /// <param name="step">The increment of each value until the end.</param>
    public FloatRangeGen(float start, float end, float step = 1)
    {
        this.start = start;
        this.end = end;
        this.step = step;
    }

    /// <summary>
    /// Converts the range into a list of floats.
    /// </summary>
    /// <returns>A list of floats from a specified start to the end.</returns>
    public IEnumerable<float> AsEnumerable()
    {
        List<float> values = new();
        for (float i = start; i < end || Math.Abs(end - i) < 0.005f ; i += step) values.Add(Convert.ToSingle(Math.Round(Convert.ToDecimal(i), 2)));
        return values;
    }
}