using System;
using System.Collections.Generic;
using System.Linq;
using RhythmFramework.Options.Enum;
using RhythmFramework.Options.Game;
using RhythmFramework.Options.Interface;
using RhythmFramework.Ranges;

namespace RhythmFramework.Options.Game;

/// <summary>
/// A class used for the purpose of making <see cref="GameOption"/>.
/// </summary>
public class GameOptionBuilder: IOptionBuilder<GameOptionBuilder>
{
    /// <summary>
    /// The option that is being built.
    /// </summary>
    protected GameOption Option = new();
    
    /// <inheritdoc />
    public GameOptionBuilder Name(string name)
    {
        Option.name = name;
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder Key(string key)
    {
        Option.key = key;
        return this;
    }
    
    /// <inheritdoc />
    public GameOptionBuilder Description(string description)
    {
        Option.Description = description;
        return this;
    }

    /// <summary>
    /// Set the category of the option.
    /// </summary>
    /// <param name="category">The category to use.</param>
    /// <returns>This <see cref="GameOptionBuilder"/> instance.</returns>
    public GameOptionBuilder Category(OptionCategory category)
    {
        Option.AppearingMode = category;
        return this;
    }
    
    /// <inheritdoc />
    public GameOptionBuilder DefaultIndex(int defaultIndex)
    {
        Option.DefaultIndex = defaultIndex;
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder Values(IEnumerable<object> values)
    {
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder Values(int defaultIndex, params object[] values)
    {
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values.Select(v => new OptionValue(v)));
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder Values(IEnumerable<OptionValue> values)
    {
        Option.Values.AddRange(values);
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder Value(Func<OptionValue.OptionValueBuilder, OptionValue> valueBuilder)
    {
        Option.Values.Add(valueBuilder(new OptionValue.OptionValueBuilder()));
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder Value(object value)
    {
        Option.Values.Add(new OptionValue(value));
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder AddBoolean(bool defaultValue = true)
    {
        Option.Values.Add(new OptionValue(defaultValue, defaultValue ? "On" : "Off"));
        Option.Values.Add(new OptionValue(!defaultValue, !defaultValue ? "On" : "Off"));
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder AddFloatRange(float start, float stop, float step)
    {
        return AddFloatRange(start, stop, step, 0);
    }

    /// <inheritdoc />
    public GameOptionBuilder AddIntRange(int start, int stop, int step)
    {
        return AddIntRange(start, stop, step, 0);
    }

    /// <summary>
    /// Adds a list of floats to the option.
    /// </summary>
    /// <param name="start">The float to start at.</param>
    /// <param name="stop">The float to end at.</param>
    /// <param name="step"></param>
    /// <param name="defaultIndex">The index this option </param>
    /// <returns>This <see cref="GameOptionBuilder"/> instance.</returns>
    public GameOptionBuilder AddFloatRange(float start, float stop, float step, int defaultIndex)
    {
        var values = new FloatRangeGen(start, stop, step).AsEnumerable()
            .Select(v => new OptionValue(v));
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values);
        return this;
    }

    /// <summary>
    /// Adds a list of ints to the option.
    /// </summary>
    /// <param name="start">The int to start at.</param>
    /// <param name="stop">The int to end at.</param>
    /// <param name="step"></param>
    /// <param name="defaultIndex">The index this option </param>
    /// <returns>This <see cref="GameOptionBuilder"/> instance.</returns>
    public GameOptionBuilder AddIntRange(int start, int stop, int step = 1, int defaultIndex = 0)
    {
        var values = new IntRangeGen(start, stop, step).AsEnumerable()
            .Select(v => new OptionValue(v));
        Option.DefaultIndex = defaultIndex;
        Option.Values.AddRange(values);
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder Bind(Action<object> changeConsumer)
    {
        Option.BindEvent(changeConsumer);
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder BindInt(Action<int> changeConsumer)
    {
        Bind(ve => changeConsumer((int)ve));
        return this;
    }

    /// <inheritdoc />
    public GameOptionBuilder BindBool(Action<bool> changeConsumer)
    {
        Bind(ve => changeConsumer((bool)ve));
        return this;
    }
    
    /// <inheritdoc />
    public GameOptionBuilder BindFloat(Action<float> changeConsumer)
    {
        Bind(ve => changeConsumer((float)ve));
        return this;
    }
    
    /// <inheritdoc />
    public GameOptionBuilder BindString(Action<string> changeConsumer)
    {
        Bind(ve => changeConsumer((string)ve));
        return this;
    }

    /// <inheritdoc />
    public void ClearValues()
    {
        Option.Values.Clear();
    }

    /// <inheritdoc />
    public TR Build<TR>() where TR : Option
    {
        return (TR) (Option) Build();
    }

    /// <summary>
    /// Build the game option.
    /// </summary>
    /// <returns>The <see cref="GameOption"/> that was built.</returns>
    public GameOption Build()
    {
        return Option;
    }
    
    // ewwwww
    IOptionBuilder IOptionBuilder.Name(string name)
    {
        return Name(name);
    }

    IOptionBuilder IOptionBuilder.Key(string key)
    {
        return Key(key);
    }

    IOptionBuilder IOptionBuilder.Description(string description)
    {
        return Description(description);
    }
    
    IOptionBuilder IOptionBuilder.DefaultIndex(int defaultIndex)
    {
        return DefaultIndex(defaultIndex);
    }
    
    IOptionBuilder IOptionBuilder.Values(IEnumerable<object> values)
    {
        return Values(values);
    }

    IOptionBuilder IOptionBuilder.Values(int defaultIndex, params object[] values)
    {
        return Values(defaultIndex, values);
    }

    IOptionBuilder IOptionBuilder.Values(IEnumerable<OptionValue> values)
    {
        return Values(values);
    }
    
    IOptionBuilder IOptionBuilder.Value(Func<OptionValue.OptionValueBuilder, OptionValue> valueBuilder)
    {
        return Value(valueBuilder);
    }

    IOptionBuilder IOptionBuilder.Value(object value)
    {
        return Value(value);
    }
    
    IOptionBuilder IOptionBuilder.AddBoolean(bool defaultValue)
    {
        return AddBoolean(defaultValue);
    }

    IOptionBuilder IOptionBuilder.AddFloatRange(float start, float stop, float step)
    {
        return AddFloatRange(start, stop, step);
    }

    IOptionBuilder IOptionBuilder.AddIntRange(int start, int stop, int step)
    {
        return AddIntRange(start, stop, step);
    }

    IOptionBuilder IOptionBuilder.Bind(Action<object> changeConsumer)
    {
        return Bind(changeConsumer);
    }

    IOptionBuilder IOptionBuilder.BindInt(Action<int> changeConsumer)
    {
        return BindInt(changeConsumer);
    }

    IOptionBuilder IOptionBuilder.BindBool(Action<bool> changeConsumer)
    {
        return BindBool(changeConsumer);
    }

    IOptionBuilder IOptionBuilder.BindFloat(Action<float> changeConsumer)
    {
        return BindFloat(changeConsumer);
    }

    IOptionBuilder IOptionBuilder.BindString(Action<string> changeConsumer)
    {
        return BindString(changeConsumer);
    }
}