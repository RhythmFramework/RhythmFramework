using System.Text;

namespace RhythmFramework.Extensions;

/// <summary>
/// An extension class for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Inserts a space before every capital letter except the first.
    /// </summary>
    /// <param name="input">The <see cref="string"/> to perform on.</param>
    /// <returns>A new <see cref="string"/> with spaces added.</returns>
    public static string WithSpaces(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        StringBuilder result = new StringBuilder();
        
        foreach (char c in input)
        {
            if (char.IsUpper(c) && result.Length > 0)
            {
                result.Append(' ');
            }
            result.Append(c);
        }

        return result.ToString();
    }
}