using Cygnus2_0.General.Times;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cygnus2_0
{
    public class TotalSumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tareas = value as IEnumerable<object>;
            if (tareas == null)
                return "$0.00";

            double sum = 0;

            foreach (var u in tareas)
            {
                sum += ((TareaHoja)u).Total;
            }

            return sum.ToString("c");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
