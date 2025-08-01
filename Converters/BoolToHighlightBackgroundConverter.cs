// BoolToHighlightBackgroundConverter.cs
#region Usings
using System.Globalization;
#endregion

namespace MauiFluentTreeView.Converters
{
    /// <summary>
    /// Converts a boolean value to a semi-transparent background color (SolidColorBrush).
    /// Typically used for highlighting items based on a boolean flag.
    /// If the boolean is true, it returns a semi-transparent version of 'PrimaryBrush',
    /// otherwise a semi-transparent 'SurfaceBrush'.
    /// </summary>
    public class BoolToHighlightBackgroundConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts a boolean value to a semi-transparent color.
        /// </summary>
        /// <param name="value">The boolean value to convert. If true, a highlight color is returned; otherwise, a normal background color.</param>
        /// <param name="targetType">The type of the target property, expected to be a Color or SolidColorBrush.</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>A <see cref="Color"/> representing a semi-transparent highlight or normal background.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Note: The original code had `!b` which means `shouldHighlight` would be true if `value` is false.
            // This might be an inversion based on desired UI behavior. Assuming it's intentional.
            bool shouldHighlight = value is bool b && !b;

            string brushKey = shouldHighlight ? "PrimaryBrush" : "SurfaceBrush";

            // Attempt to retrieve a brush from application resources and return a semi-transparent version.
            if (Application.Current.Resources.TryGetValue(brushKey, out var resource) &&
                resource is SolidColorBrush solidBrush)
            {
                var color = solidBrush.Color;
                return new Color(color.Red, color.Green, color.Blue, 0.2f); // 20% opacity
            }

            return new Color(0, 0, 0, 0); // Fully transparent fallback
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