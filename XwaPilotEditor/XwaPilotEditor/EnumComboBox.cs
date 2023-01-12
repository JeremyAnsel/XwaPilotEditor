using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace XwaPilotEditor
{
    public class EnumComboBox : ComboBox
    {
        static EnumComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumComboBox), new FrameworkPropertyMetadata(typeof(EnumComboBox)));
        }

        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }

        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumComboBox), new PropertyMetadata(OnEnumTypeChange));

        private static void OnEnumTypeChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = sender as EnumComboBox;

            if (e.NewValue != null)
            {
                comboBox.ItemsSource = Enum.GetValues((Type)e.NewValue);
            }
        }

        public EnumComboBox()
            : base()
        {
            this.IsEditable = true;
            this.IsReadOnly = true;
            this.AddHandler(TextBox.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(this.Button_Click));
            this.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.Button_Click));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = true;
        }
    }
}
