// NullToBoolConverter.cs
#region Usings
using System.Globalization;
#endregion

namespace MauiFluentTreeView.Converters
{
    /// <summary>
    /// Converts any object value to a boolean.
    /// If the value is a boolean, it's returned directly. Otherwise, it returns true.
    /// The original comment "Returns false if null, true otherwise" seems to be incorrect based on implementation.
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts an object value to a boolean.
        /// If the input value is a boolean, it is returned directly. For any other non-null type, it returns true.
        /// This converter's name and original comment ("Returns false if null, true otherwise")
        /// suggest an intent to check for nullability, but the current implementation returns
        /// `true` for any non-boolean non-null value, and the boolean value itself if it's a boolean.
        /// It will not return `false` for null.
        /// </summary>
        /// <param name="value">The object value to convert.</param>
        /// <param name="targetType">The type of the target property, expected to be boolean.</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>The boolean value if the input is boolean, otherwise true.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is already a boolean, return it directly.
            if (value is bool b)
                return b; // true => true, false => false

            // For any other value (including null or non-boolean objects), return true.
            // This behavior deviates from a typical "NullToBool" converter which would return false for null.
            return true;
        }

        /// <summary>
        /// Not implemented as this converter is intended for one-way binding.
        /// </summary>
        /// <exception cref="NotImplementedException">Always throws a <see cref="NotImplementedException"/>.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
        #endregion
    }
}
