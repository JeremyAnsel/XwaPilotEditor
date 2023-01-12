using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace XwaPilotEditor
{
    public class ListBoxEditor : ListBox
    {
        public ListBoxEditor()
        {
            AlternationCount = int.MaxValue;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            SetValue(Grid.IsSharedSizeScopeProperty, true);
            SetValue(VirtualizingPanel.IsVirtualizingProperty, false);
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue is not null)
            {
                Type type = newValue.GetType().GetElementType();

                if (type.IsEnum)
                {
                    SetItemTemplate(type);
                }
                else
                {
                    SetItemTemplate(null);
                }
            }
        }

        private void SetItemTemplate(Type enumType)
        {
            var grid = new FrameworkElementFactory(typeof(Grid));

            var column1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column1.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto));
            column1.SetValue(ColumnDefinition.SharedSizeGroupProperty, "Column1");
            grid.AppendChild(column1);

            var column2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            grid.AppendChild(column2);

            var textBlock = new FrameworkElementFactory(typeof(TextBlock));
            textBlock.SetValue(Grid.ColumnProperty, 0);
            textBlock.SetValue(TextBlock.TextProperty, new Binding("TemplatedParent.(ItemsControl.AlternationIndex)")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                Converter = TemplatedParentRowHeaderConverter.Default,
                ConverterParameter = this
            });
            grid.AppendChild(textBlock);

            if (enumType is null)
            {
                var textBox = new FrameworkElementFactory(typeof(TextBox));
                textBox.SetValue(Grid.ColumnProperty, 1);
                textBox.SetValue(TextBox.TextProperty, new Binding("."));
                grid.AppendChild(textBox);
            }
            else
            {
                var comboBox = new FrameworkElementFactory(typeof(EnumComboBox));
                comboBox.SetValue(Grid.ColumnProperty, 1);
                comboBox.SetValue(EnumComboBox.EnumTypeProperty, enumType);
                comboBox.SetValue(EnumComboBox.TextProperty, new Binding("."));
                grid.AppendChild(comboBox);
            }

            var template = new DataTemplate
            {
                VisualTree = grid
            };

            ItemTemplate = template;
        }
    }
}
