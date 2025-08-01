// ExpandIconConverter.cs
#region Usings
using MauiIcons.Material;
using System.Globalization;
#endregion

namespace MauiFluentTreeView.Converters
{
    /// <summary>
    /// Converts a boolean value indicating expansion state to a MauiIcon for the expand/collapse indicator.
    /// Returns a "dropdown arrow" icon when expanded, and a "right arrow" icon when collapsed.
    /// </summary>
    public class ExpandIconConverter : IValueConverter
    {
        #region IValueConverter Implementation
        /// <summary>
        /// Converts a boolean 'expanded' state to a corresponding MaterialIcons icon.
        /// </summary>
        /// <param name="value">A boolean indicating whether the node is expanded (true) or collapsed (false).</param>
        /// <param name="targetType">The type of the target property, expected to be an icon type.</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>A <see cref="MauiIcons.Core.Base.BaseIcon"/> representing the expanded or collapsed state, or null if the value is not a boolean.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool expanded)
            {
                if (expanded)
                {
                    return new MauiIcons.Core.Base.BaseIcon() { Icon = MaterialIcons.ArrowDropDown };
                }
                else
                {
                    return new MauiIcons.Core.Base.BaseIcon() { Icon = MaterialIcons.ArrowRight }; ;
                }
            }
            return null;
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
