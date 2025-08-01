// IsSingleSelectionConverter.cs
#region Usings
using MauiFluentTreeView.Models;
using System.Globalization;
#endregion

namespace MauiFluentTreeView.Converters
{
    /// <summary>
    /// Converts a <see cref="TreeSelectionMode"/> enum value to a boolean,
    /// indicating if the selection mode is <see cref="TreeSelectionMode.Single"/>.
    /// </summary>
    public class IsSingleSelectionConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts a <see cref="TreeSelectionMode"/> to a boolean.
        /// </summary>
        /// <param name="value">The <see cref="TreeSelectionMode"/> to convert.</param>
        /// <param name="targetType">The type of the target property, expected to be boolean.</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>True if the mode is <see cref="TreeSelectionMode.Single"/>, otherwise false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is TreeSelectionMode mode && mode == TreeSelectionMode.Single;

        /// <summary>
        /// Not supported as this converter is intended for one-way binding.
        /// </summary>
        /// <exception cref="NotSupportedException">Always throws a <see cref="NotSupportedException"/>.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
        #endregion
    }
}
