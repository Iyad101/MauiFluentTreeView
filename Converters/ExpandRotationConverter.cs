// ExpandRotationConverter.cs
#region Usings
using System.Globalization;
#endregion

namespace MauiFluentTreeView.Converters
{
    /// <summary>
    /// Converts a boolean value indicating expanded state to a rotation angle (double).
    /// Returns 45.0 degrees if expanded, otherwise 0.0 degrees.
    /// </summary>
    public class ExpandRotationConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts a boolean 'expanded' state to a rotation angle.
        /// </summary>
        /// <param name="value">A boolean indicating whether the node is expanded (true) or collapsed (false).</param>
        /// <param name="targetType">The type of the target property, expected to be a double.</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>45.0 if expanded, 0.0 if collapsed or if the value is not a boolean.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool expanded)
            {
                return expanded ? 45.0 : 0.0;
            }
            return 0.0;
        }

        /// <summary>
        /// Not supported as this converter is intended for one-way binding.
        /// </summary>
        /// <exception cref="NotSupportedException">Always throws a <see cref="NotSupportedException"/>.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
        #endregion
    }
}