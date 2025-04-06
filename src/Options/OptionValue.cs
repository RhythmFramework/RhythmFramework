using System;

namespace RhythmFramework.Options;

/// <summary>
/// A class representing the value of an object.
/// </summary>
public class OptionValue
{
    internal object Value { get; private set; } = null!;
    internal string? Text;
    
    private OptionValue() {}

    internal OptionValue(object value)
    {
        Value = value ?? throw new NullReferenceException("Value cannot be null.");
    }
    
    internal OptionValue(object value, string? text = null)
    {
        Value = value ?? throw new NullReferenceException("Value cannot be null.");
        Text = text;
    }

    /// <summary>
    /// Returns the display text of this option value.
    /// </summary>
    /// <returns>A string of the visible value shown to the player.</returns>
    public string GetText()
    {
        return Text ?? Value.ToString();
    }
    
    /// <summary>
    /// A builder for <see cref="OptionValue"/>.
    /// </summary>
    public class OptionValueBuilder
    {
        /// <summary>
        /// The actual value of this option.
        /// </summary>
        protected object InnerValue = null!;
        
        /// <summary>
        /// The text shown to the player.
        /// </summary>
        protected string? TextOptional;
        
        /// <summary>
        /// Change the text of the OptionValue.
        /// </summary>
        /// <param name="text">The text to set.</param>
        /// <returns>This <see cref="OptionValueBuilder"/> instance.</returns>
        public OptionValueBuilder Text(string text)
        {
            TextOptional = text;
            return this;
        }

        /// <summary>
        /// Change the value of the OptionValue.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>This <see cref="OptionValueBuilder"/> instance.</returns>
        public OptionValueBuilder Value(object value)
        {
            InnerValue = value;
            return this;
        }

        /// <summary>
        /// Build the OptionValue.
        /// </summary>
        /// <returns>The built <see cref="OptionValue"/>.</returns>
        public OptionValue Build()
        {
            return new OptionValue
            {
                Text = TextOptional,
                Value = InnerValue
            };
        }
    }
}