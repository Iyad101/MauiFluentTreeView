// LeafToVisibilityConverter.cs
#region Usings
using System.Globalization;
#endregion

namespace MauiFluentTreeView.Converters
{
    /// <summary>
    /// Converts a boolean value, typically representing if a node is a "leaf" (has no children),
    /// to a boolean indicating visibility. Returns false (hidden) if it's a leaf, true (visible) otherwise.
    /// This is often used to hide the expand/collapse icon for leaf nodes.
    /// </summary>
    public class LeafToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts a boolean representing 'IsLeaf' to a boolean indicating visibility.
        /// If the value is true (it's a leaf node), it returns false (hidden).
        /// If the value is false (it's not a leaf node), it returns true (visible).
        /// </summary>
        /// <param name="value">The boolean value indicating if the node is a leaf.</param>
        /// <param name="targetType">The type of the target property, expected to be boolean.</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>False if <paramref name="value"/> is true (is a leaf), true otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If it's a leaf, hide the expand icon.
            return value is bool isLeaf && isLeaf
                ? false // If true (is a leaf), return false (hidden)
                : true;  // If false (not a leaf), return true (visible)
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
