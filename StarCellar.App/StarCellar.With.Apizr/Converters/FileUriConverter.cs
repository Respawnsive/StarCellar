using System.Globalization;

namespace StarCellar.With.Apizr.Converters
{
    internal class FileUriConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            !string.IsNullOrWhiteSpace(value?.ToString()) ? $"{Constants.BaseAddress}/{value}" : null;

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
