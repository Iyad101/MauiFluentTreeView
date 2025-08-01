// BoolToStripColorConverter.cs
#region Usings
using System.Globalization;
#endregion

namespace MauiFluentTreeView.Converters
{
    /// <summary>
    /// Converts a boolean flag to a <see cref="Color"/>.
    /// Returns <see cref="Colors.Orange"/> when the flag is true, and <see cref="Colors.Transparent"/> when false.
    /// </summary>
    public class BoolToStripColorConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts a boolean flag to a Color.
        /// </summary>
        /// <param name="value">The boolean flag to convert.</param>
        /// <param name="targetType">The type of the target property, expected to be a Color.</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns><see cref="Colors.Orange"/> if <paramref name="value"/> is true, <see cref="Colors.Transparent"/> if false or not a boolean.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool flag)
            {
                if (flag)
                    return Colors.Orange;       // When true
                else
                    return Colors.Transparent;  // When false
            }

            return Colors.Transparent; // Fallback for non-boolean values
        }

        /// <summary>
        /// Not implemented as this converter is intended for one-way binding.
        /// </summary>
        /// <exception cref="NotImplementedException">Always throws a <see cref="NotImplementedException"/>.</exception>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}