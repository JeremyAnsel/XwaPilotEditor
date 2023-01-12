using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace XwaPilotEditor
{
    public sealed class ArrayBinderConverter : IValueConverter
    {
        public static readonly ArrayBinderConverter Default = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool allowEdit = true;

            Type headerType = null;
            if (parameter is Type type && type.IsEnum)
            {
                headerType = type;
            }

            if (value is int[,] arrayInt)
            {
                return new ArrayBinder<int>(arrayInt, allowEdit, headerType);
            }

            if (value is byte[,] arrayByte)
            {
                return new ArrayBinder<byte>(arrayByte, allowEdit, headerType);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
