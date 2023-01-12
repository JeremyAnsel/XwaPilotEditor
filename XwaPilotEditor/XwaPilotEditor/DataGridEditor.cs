using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace XwaPilotEditor
{
    public class DataGridEditor : DataGrid
    {
        public DataGridEditor()
        {
            CanUserSortColumns = false;
            LoadingRow += DataGrid_LoadingRow;
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGrid element = (DataGrid)sender;
            int index = e.Row.GetIndex();
            string header;

            if (element.Tag is Type type && type.IsEnum)
            {
                string name = Enum.GetName(type, index);
                header = index.ToString() + "-" + name;
            }
            else if (element.Tag is IList<string> list)
            {
                string name = list.ElementAtOrDefault(index);
                header = index.ToString() + "-" + name;
            }
            else
            {
                header = index.ToString();
            }

            e.Row.Header = header;
        }
    }
}
