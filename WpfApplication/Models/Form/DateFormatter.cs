using System;
using System.Windows.Data;

namespace WpfApplication.Models.Form {
    public class DateFormatter : IValueConverter {
        /// <summary>
        /// This converts the DateTime object to the string to display.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert( object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture ) {
            // Retrieve the format string and use it to format the value.
            //double formatString = ( double ) parameter[ 0 ] + 100;
            // If the format string is null or empty, simply call ToString()
            // on the value.
            double return_value = 0;
            if ( value == null || parameter == null ) {
                return return_value;
            }
            var param = parameter.ToString();
            if ( param.Length > 0 ) {
                return_value = ( double ) value * System.Convert.ToDouble( param ) / 100;
            }
            return return_value;
        }

        /// <summary>
        /// No need to implement converting back on a one-way binding 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack( object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture ) {
            throw new NotImplementedException();
        }
    }
}