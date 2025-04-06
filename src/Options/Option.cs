using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RhythmFramework.Options;

/// <summary>
/// An option that necessarily doesn't have to be in game.
/// </summary>
public class Option
{
    internal string name = null!;
    internal string? key;

    internal int? Index = null!;
    /// <summary>
    /// The default index of this option.
    /// </summary>
    public int DefaultIndex { get; internal set; }
    /// <summary>
    /// The description of this option.
    /// </summary>
    public string? Description;

    internal List<OptionValue> Values = new();
    /// <summary>
    /// The current value of this option.
    /// </summary>
    protected OptionValue? Value = null!;

    /// <summary>
    /// The callback for value changes.
    /// </summary>
    protected Action<object>? ChangeConsumer;
    
    /// <summary>
    /// Returns the name of this option.
    /// </summary>
    /// <returns>Returns a name as <see cref="string"/>.</returns>
    public string Name() => name;
    /// <summary>
    /// Returns the key of this option.
    /// </summary>
    /// <returns>Returns the key as <see cref="string"/>. If null, returns the name as <see cref="string"/>.</returns>
    public string Key() => key ?? Name();
    
    /// <summary>
    /// Returns the default <see cref="OptionValue"/>.
    /// </summary>
    /// <returns>Returns a <see cref="OptionValue"/> at the default index.</returns>
    public OptionValue GetDefault()
    {
        return Values[EnforceIndexConstraint(DefaultIndex, true)];
    }
    
    /// <summary>
    /// Gets the raw <see cref="OptionValue"/> of this option.
    /// </summary>
    /// <returns>An <see cref="OptionValue"/> instance.</returns>
    public OptionValue GetRawValue()
    {
        if (Value != null) return Value;
        if (!Index.HasValue) return GetDefault();
        Value ??= Values[EnforceIndexConstraint(Index.Value, true)];
        return Value;
    }

    /// <summary>
    /// Gets the underlying <see cref="object"/> of the current <see cref="OptionValue"/>.
    /// </summary>
    /// <returns>The <see cref="object"/> of the current <see cref="OptionValue"/>.</returns>
    public object GetValue() => GetRawValue().Value;

    /// <summary>
    /// Converts the underlying <see cref="object"/> to type T.
    /// </summary>
    /// <typeparam name="T">The type to cast to.</typeparam>
    /// <returns><see cref="object"/> casted to type T.</returns>
    public T GetValue<T>() => (T)GetRawValue().Value;

    /// <summary>
    /// Gets the display text of the current option.
    /// </summary>
    /// <returns>A <see cref="string"/> of the displayed text.</returns>
    public string GetValueText() => GetRawValue().GetText();

    internal object SetValue(OptionValue value)
    {
        Index = Values.IndexOf(value);
        if (Index.Value == -1) Index = DefaultIndex;
        Value = value;
        if (ChangeConsumer != null) ChangeConsumer(value.Value);
        return value.Value;
    }

    /// <summary>
    /// Set the value of this option.
    /// </summary>
    /// <param name="index">The value index to switch to.</param>
    /// <param name="triggerEvent">Whether to trigger the callback bound to this option.</param>
    /// <returns>If the passed index argument was out of bounds, it will return an int that is in bounds. Otherwise, just returns what was passed.</returns>
    public int SetValue(int index, bool triggerEvent = true)
    {
        Index = EnforceIndexConstraint(index);
        Value = Values[index];
        if (triggerEvent && ChangeConsumer != null) ChangeConsumer(Value.Value);
        return Index.Value;
    }

    /// <summary>
    /// Forcefully set the value of this option.
    /// If the value does not exist, create a new option at the end of the list.
    /// </summary>
    /// <param name="value"></param>
    public void SetHardValue(object value)
    {
        OptionValue? targetValue = Values.FirstOrDefault(ov => ov.Value == value);
        if (targetValue == null)
        {
            Values.Add(new OptionValue(value));
            targetValue = Values.Last();
        }
        SetValue(targetValue);
    }
    
    /// <summary>
    /// Change the default index of this option.
    /// </summary>
    /// <param name="index">The new default index.</param>
    public void SetDefaultIndex(int index)
    {
        DefaultIndex = EnforceIndexConstraint(index);
    }

    /// <summary>
    /// Bind an event to this option.
    /// </summary>
    /// <param name="changeConsumer">The callback to run.</param>
    public void BindEvent(Action<object> changeConsumer)
    {
        if  (ChangeConsumer == null) ChangeConsumer = changeConsumer;
        else if (!ChangeConsumer.GetInvocationList().Contains(changeConsumer)) ChangeConsumer += changeConsumer;
    }
    
    /// <summary>
    /// Remove a callback bound to this option.
    /// </summary>
    /// <param name="changeConsumer">The callback to remove.</param>
    public void UnbindEvent(Action<object> changeConsumer)
    {
        if  (ChangeConsumer == null) return;
        ChangeConsumer -= changeConsumer;
    }

    /// <summary>
    /// Run the bound events with the specified value.
    /// </summary>
    /// <param name="value">The value to pass.</param>
    public void RunCallback(object value)
    {
        ChangeConsumer?.Invoke(value);
    }
    
    /// <summary>
    /// Changes the passed index to be within boundaries.
    /// </summary>
    /// <param name="index">The index to check.</param>
    /// <param name="throwError">Will throw errors if this is true.</param>
    /// <returns></returns>
    /// <exception cref="ConstraintException">If throwError is true, will throw if the index is out of bounds or if the list is empty.</exception>
    protected int EnforceIndexConstraint(int index, bool throwError = false)
    {
        if (Values.Count == 0)
            throw new ConstraintException($"Index fails constraint because no values exist! (Option={Key()})");
        if (index >= Values.Count)
        {
            if (!throwError)
                throw new ConstraintException($"Index is greater than value count! ({index} >= {Values.Count})");
            index = 0;
        } 
        else if (index < 0)
        {
            if (!throwError)
                throw new ConstraintException($"Index is less than zero! ({index} < 0)");
            index = Values.Count - 1;
        }

        return index;
    }
}