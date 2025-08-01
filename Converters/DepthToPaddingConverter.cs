// DepthToPaddingConverter.cs
#region Usings
using System.Diagnostics;
using System.Globalization;
#endregion

namespace MauiFluentTreeView.Converters
{
    /// <summary>
    /// Converts an integer depth value into a <see cref="Thickness"/> for UI padding,
    /// typically used to indent tree view nodes based on their hierarchy level.
    /// </summary>
    public class DepthToPaddingConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts an integer depth to a Thickness value, applying a horizontal indent.
        /// Each level of depth adds 20 units of padding to the left.
        /// </summary>
        /// <param name="value">The integer depth of the node.</param>
        /// <param name="targetType">The type of the target property, expected to be Thickness.</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>A <see cref="Thickness"/> with left padding based on the depth, or a default Thickness(0) if the value is not an integer.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int depth)
            {
                return new Thickness(depth * 20, 0, 0, 0); // Horizontal indent per level
            }
            // Log if an unexpected type is received, as this indicates a binding issue elsewhere.
            Debug.WriteLine($"WARNING: DepthToPaddingConverter received unexpected value type: {value?.GetType().Name} (Value: {value})");
            return new Thickness(0); // Return a default thickness
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
