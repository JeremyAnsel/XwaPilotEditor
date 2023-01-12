using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace XwaPilotEditor
{
    public class TemplatedParentRowHeaderConverter : IValueConverter
    {
        public static readonly TemplatedParentRowHeaderConverter Default = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;

            if (parameter is FrameworkElement element)
            {
                if (element.Tag is Type type && type.IsEnum)
                {
                    string name = Enum.GetName(type, index);
                    return index.ToString() + "-" + name;
                }

                if (element.Tag is IList<string> list)
                {
                    string name = list.ElementAtOrDefault(index);
                    return index.ToString() + "-" + name;
                }
            }

            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
