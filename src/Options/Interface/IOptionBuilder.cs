using System;
using System.Collections.Generic;

namespace RhythmFramework.Options.Interface;

/// <summary>
/// The interface for option builders that create configurable options with various types and values.
/// Provides a fluent API for creating and configuring options.
/// </summary>
public interface IOptionBuilder
{
    /// <summary>
    /// Sets the display name of the option.
    /// </summary>
    /// <param name="name">The human-readable name that will be displayed in the UI.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder Name(string name);

    /// <summary>
    /// Sets the unique identifier key for the option.
    /// </summary>
    /// <param name="key">The unique string key used to reference this option programmatically.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder Key(string key);

    /// <summary>
    /// Sets the description text for the option.
    /// </summary>
    /// <param name="description">A detailed description of what the option controls or affects.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder Description(string description);
    
    /// <summary>
    /// Sets the default selected index for this option.
    /// </summary>
    /// <param name="defaultIndex">The zero-based index of the default value in the values collection.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder DefaultIndex(int defaultIndex);

    /// <summary>
    /// Sets the collection of possible values for this option.
    /// </summary>
    /// <param name="values">An enumerable collection of objects representing the possible values.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder Values(IEnumerable<object> values);

    /// <summary>
    /// Sets the default index and collection of possible values for this option.
    /// </summary>
    /// <param name="defaultIndex">The zero-based index of the default value in the values array.</param>
    /// <param name="values">An array of objects representing the possible values.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder Values(int defaultIndex, params object[] values);

    /// <summary>
    /// Sets the collection of possible values for this option using OptionValue objects.
    /// </summary>
    /// <param name="values">An enumerable collection of OptionValue objects.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder Values(IEnumerable<OptionValue> values);

    /// <summary>
    /// Adds a single value to the option using a builder function.
    /// </summary>
    /// <param name="valueBuilder">A function that builds and returns an OptionValue.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder Value(Func<OptionValue.OptionValueBuilder, OptionValue> valueBuilder);

    /// <summary>
    /// Adds a single value to the option.
    /// </summary>
    /// <param name="value">The object to add as a possible value.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder Value(object value);
    
    /// <summary>
    /// Configures this option as a boolean toggle with a default value.
    /// </summary>
    /// <param name="defaultValue">The default state of the boolean option (true or false).</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder AddBoolean(bool defaultValue = true);

    /// <summary>
    /// Configures this option as a floating-point range with specified bounds and step.
    /// </summary>
    /// <param name="start">The minimum value of the range.</param>
    /// <param name="stop">The maximum value of the range.</param>
    /// <param name="step">The increment between each possible value in the range.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder AddFloatRange(float start, float stop, float step);

    /// <summary>
    /// Configures this option as an integer range with specified bounds and step.
    /// </summary>
    /// <param name="start">The minimum value of the range.</param>
    /// <param name="stop">The maximum value of the range.</param>
    /// <param name="step">The increment between each possible value in the range (defaults to 1).</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder AddIntRange(int start, int stop, int step = 1);

    /// <summary>
    /// Binds a callback action to be invoked when the option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as an object.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder Bind(Action<object> changeConsumer);

    /// <summary>
    /// Binds a callback action to be invoked when the integer option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as an integer.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder BindInt(Action<int> changeConsumer);

    /// <summary>
    /// Binds a callback action to be invoked when the boolean option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as a boolean.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder BindBool(Action<bool> changeConsumer);

    /// <summary>
    /// Binds a callback action to be invoked when the floating-point option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as a float.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder BindFloat(Action<float> changeConsumer);

    /// <summary>
    /// Binds a callback action to be invoked when the string option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as a string.</param>
    /// <returns>This <see cref="IOptionBuilder"/> instance.</returns>
    public IOptionBuilder BindString(Action<string> changeConsumer);
    
    /// <summary>
    /// Clear the values previously added to this option.
    /// </summary>
    public void ClearValues();
    
    /// <summary>
    /// Build to a specific option subclass.
    /// </summary>
    /// <typeparam name="TR">The subclass to build to.</typeparam>
    /// <returns>A new instance of a subclass of <see cref="Option"/> configured according to the builder settings.</returns>
    public TR Build<TR>() where TR: Option;
}

/// <summary>
/// The interface for custom option builders that provide a strongly-typed fluent API.
/// Extends the base IOptionBuilder interface with type-specific method return types.
/// </summary>
/// <typeparam name="T">The custom builder type that implements this interface.</typeparam>
public interface IOptionBuilder<out T> : IOptionBuilder where T : IOptionBuilder
{
    /// <summary>
    /// Sets the display name of the option.
    /// </summary>
    /// <param name="name">The human-readable name that will be displayed in the UI.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T Name(string name);

    /// <summary>
    /// Sets the unique identifier key for the option.
    /// </summary>
    /// <param name="key">The unique string key used to reference this option programmatically.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T Key(string key);
    
    /// <summary>
    /// Sets the description text for the option.
    /// </summary>
    /// <param name="description">A detailed description of what the option controls or affects.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T Description(string description);
    
    /// <summary>
    /// Sets the default selected index for this option.
    /// </summary>
    /// <param name="defaultIndex">The zero-based index of the default value in the values collection.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T DefaultIndex(int defaultIndex);
    
    /// <summary>
    /// Sets the collection of possible values for this option.
    /// </summary>
    /// <param name="values">An enumerable collection of objects representing the possible values.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T Values(IEnumerable<object> values);

    /// <summary>
    /// Sets the default index and collection of possible values for this option.
    /// </summary>
    /// <param name="defaultIndex">The zero-based index of the default value in the values array.</param>
    /// <param name="values">An array of objects representing the possible values.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T Values(int defaultIndex, params object[] values);

    /// <summary>
    /// Sets the collection of possible values for this option using OptionValue objects.
    /// </summary>
    /// <param name="values">An enumerable collection of OptionValue objects.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T Values(IEnumerable<OptionValue> values);
    
    /// <summary>
    /// Adds a single value to the option using a builder function.
    /// </summary>
    /// <param name="valueBuilder">A function that builds and returns an OptionValue.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T Value(Func<OptionValue.OptionValueBuilder, OptionValue> valueBuilder);

    /// <summary>
    /// Adds a single value to the option.
    /// </summary>
    /// <param name="value">The object to add as a possible value.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T Value(object value);
    
    /// <summary>
    /// Configures this option as a boolean toggle with a default value.
    /// </summary>
    /// <param name="defaultValue">The default state of the boolean option (true or false).</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T AddBoolean(bool defaultValue = true);
    
    /// <summary>
    /// Configures this option as a floating-point range with specified bounds and step.
    /// </summary>
    /// <param name="start">The minimum value of the range.</param>
    /// <param name="stop">The maximum value of the range.</param>
    /// <param name="step">The increment between each possible value in the range.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T AddFloatRange(float start, float stop, float step);

    /// <summary>
    /// Configures this option as an integer range with specified bounds and step.
    /// </summary>
    /// <param name="start">The minimum value of the range.</param>
    /// <param name="stop">The maximum value of the range.</param>
    /// <param name="step">The increment between each possible value in the range.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T AddIntRange(int start, int stop, int step);

    /// <summary>
    /// Binds a callback action to be invoked when the option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as an object.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T Bind(Action<object> changeConsumer);

    /// <summary>
    /// Binds a callback action to be invoked when the integer option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as an integer.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T BindInt(Action<int> changeConsumer);

    /// <summary>
    /// Binds a callback action to be invoked when the boolean option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as a boolean.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T BindBool(Action<bool> changeConsumer);

    /// <summary>
    /// Binds a callback action to be invoked when the floating-point option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as a float.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T BindFloat(Action<float> changeConsumer);

    /// <summary>
    /// Binds a callback action to be invoked when the string option value changes.
    /// </summary>
    /// <param name="changeConsumer">The action to execute when the value changes, receiving the new value as a string.</param>
    /// <returns>This <see cref="IOptionBuilder{T}"/> instance for method chaining.</returns>
    public new T BindString(Action<string> changeConsumer);
}