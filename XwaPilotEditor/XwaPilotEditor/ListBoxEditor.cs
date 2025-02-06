using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

        public Type ArrayType { get; private set; }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue is not null && newValue.GetType().IsArray)
            {
                ArrayType = newValue.GetType().GetElementType();

                if (newValue is int[] collectionInt32)
                {
                    var source = new Collection<ListBoxEditorItem<int>>();
                    int index = 0;
                    foreach (var value in newValue)
                    {
                        source.Add(new ListBoxEditorItem<int>(collectionInt32, index));
                        index++;
                    }
                    ItemsSource = source;
                }
                else if (newValue is byte[] collectionByte)
                {
                    var source = new Collection<ListBoxEditorItem<byte>>();
                    int index = 0;
                    foreach (var value in newValue)
                    {
                        source.Add(new ListBoxEditorItem<byte>(collectionByte, index));
                        index++;
                    }
                    ItemsSource = source;
                }
                else
                {
                    throw new ArgumentNullException(nameof(newValue));
                }

                return;
            }

            if (newValue is not null)
            {
                if (ArrayType is not null && ArrayType.IsEnum)
                {
                    SetItemTemplate(ArrayType);
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
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("TemplatedParent.(ItemsControl.AlternationIndex)")
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
                textBox.SetBinding(TextBox.TextProperty, new Binding("NewValue")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                grid.AppendChild(textBox);
            }
            else
            {
                var comboBox = new FrameworkElementFactory(typeof(EnumComboBox));
                comboBox.SetValue(Grid.ColumnProperty, 1);
                comboBox.SetValue(EnumComboBox.EnumTypeProperty, enumType);
                comboBox.SetBinding(ComboBox.SelectedIndexProperty, new Binding("NewValue")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
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
