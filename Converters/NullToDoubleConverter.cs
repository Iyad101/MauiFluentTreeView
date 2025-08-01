// NullToDoubleConverter.cs
#region Usings
using System.Globalization;
#endregion

namespace MauiFluentTreeView.Converters
{
    /// <summary>
    /// Converts a string value to a double, returning <see cref="NullOrEmptyValue"/> if the string is null or empty,
    /// and <see cref="NonEmptyValue"/> otherwise. Useful for controlling dimensions based on content presence.
    /// </summary>
    public class NullToDoubleConverter : IValueConverter
    {
        #region Properties
        /// <summary>
        /// Gets or sets the double value to return when the input string is null or empty.
        /// Default is 0.
        /// </summary>
        public double NullOrEmptyValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the double value to return when the input string is not null or empty.
        /// Default is 40.
        /// </summary>
        public double NonEmptyValue { get; set; } = 40;
        #endregion

        #region IValueConverter Implementation
        /// <summary>
        /// Converts a string value to a double based on whether the string is null or empty.
        /// </summary>
        /// <param name="value">The object value to convert, expected to be a string.</param>
        /// <param name="targetType">The type of the target property, expected to be a double.</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns><see cref="NullOrEmptyValue"/> if the string is null or empty, otherwise <see cref="NonEmptyValue"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? NullOrEmptyValue : NonEmptyValue;
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
