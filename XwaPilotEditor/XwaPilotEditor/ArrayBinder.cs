using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XwaPilotEditor
{
    public sealed class ArrayBinder<T> : IBindingList, ITypedList where T : struct
    {
        private sealed class ArrayColumn : PropertyDescriptor
        {
            public ArrayBinder<T> Owner { get; }
            public int ColumnIndex { get; }

            private static string GetHeaderName(ArrayBinder<T> owner, int index)
            {
                if (owner._headerType is null)
                {
                    return $"[{index}]";
                }
                else
                {
                    return $"[{index}-{Enum.GetName(owner._headerType, index)}]";
                }
            }

            public ArrayColumn(ArrayBinder<T> owner, int index)
                : base(GetHeaderName(owner, index), null)
            {
                Owner = owner;
                ColumnIndex = index;
            }

            public override bool IsReadOnly => !Owner.AllowEdit;
            public override Type ComponentType => typeof(ArrayRow);
            public override Type PropertyType => typeof(T);

            public override object GetValue(object component)
            {
                if (component is not ArrayRow row || row.Owner != Owner)
                    throw new ArgumentException();

                return Owner._array[row.RowIndex, ColumnIndex];
            }

            public override void SetValue(object component, object value)
            {
                if (IsReadOnly)
                    throw new InvalidOperationException();

                if (component is not ArrayRow row || row.Owner != Owner)
                    throw new ArgumentException();

                if (value is string stringValue)
                {
                    if (int.TryParse(stringValue, out int r))
                    {
                        Owner._array[row.RowIndex, ColumnIndex] = (T)(object)r;
                    }
                }
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override void ResetValue(object component)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class ArrayRow
        {
            public ArrayBinder<T> Owner { get; }
            public int RowIndex { get; }

            public ArrayRow(ArrayBinder<T> owner, int index)
            {
                Owner = owner;
                RowIndex = index;
            }

            // Workaround: WPF does not honor the PropertyDescriptor GetValue/SetValue implementation, but instead binds directly against the indexer
            public object this[int ColumnIndex]
            {
                get => Owner._array[RowIndex, ColumnIndex];
                set
                {
                    if (!Owner.AllowEdit)
                        throw new InvalidOperationException();

                    if (value is string stringValue)
                    {
                        if (int.TryParse(stringValue, out int r))
                        {
                            Owner._array[RowIndex, ColumnIndex] = (T)(object)r;
                        }
                    }
                }
            }

            public object this[string ColumnName]
            {
                get
                {
                    int ColumnIndex = int.Parse(ColumnName.Substring(0, ColumnName.IndexOf('-')));
                    return this[ColumnIndex];
                }

                set
                {
                    int ColumnIndex = int.Parse(ColumnName.Substring(0, ColumnName.IndexOf('-')));
                    this[ColumnIndex] = value;
                }
            }
        }

        private readonly T[,] _array;
        private readonly ArrayRow[] _rows;
        private readonly PropertyDescriptorCollection _columns;
        internal readonly Type _headerType;

        public ArrayBinder(T[,] source, bool allowEdit, Type headerType)
        {
            this.AllowEdit = allowEdit;

            _array = source;
            _headerType = headerType;

            _rows = new ArrayRow[source.GetLength(0)];
            for (int i = 0; i < _rows.Length; i++)
                _rows[i] = new ArrayRow(this, i);

            var columns = new ArrayColumn[source.GetLength(1)];
            for (int i = 0; i < columns.Length; i++)
                columns[i] = new ArrayColumn(this, i);

            _columns = new PropertyDescriptorCollection(columns, true);
        }

        public object this[int index]
        {
            get => _rows[index];
            set => throw new NotSupportedException();
        }

        public bool AllowNew => false; // adding rows is not possible on a 2d array
        public bool AllowEdit { get; set; }
        public bool AllowRemove => false; // removing rows is not possible on a 2d array
        public bool SupportsChangeNotification => false;
        public bool SupportsSearching => false;
        public bool SupportsSorting => false;
        public bool IsSorted => false;
        public PropertyDescriptor SortProperty => null;
        public ListSortDirection SortDirection => ListSortDirection.Ascending;
        public bool IsReadOnly => true; // we allow editing row values (AllowEdit) not editing the list itself
        public bool IsFixedSize => true; // adding/removing rows is not possible on a 2d array
        public int Count => _rows.Length;
        public object SyncRoot => this;
        public bool IsSynchronized => false;

        public event ListChangedEventHandler ListChanged { add { } remove { } }

        public int Add(object value) => throw new NotSupportedException();
        public void AddIndex(PropertyDescriptor property) { }
        public object AddNew() => throw new NotSupportedException();
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction) => throw new NotSupportedException(); // SupportsSorting is false
        public void Clear() => throw new NotSupportedException();
        public bool Contains(object value) => IndexOf(value) >= 0;
        public void CopyTo(Array array, int index) => _rows.CopyTo(array, index);
        public int Find(PropertyDescriptor property, object key) => throw new NotSupportedException(); // SupportsSorting is false
        public IEnumerator GetEnumerator() => _rows.GetEnumerator();

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (listAccessors != null && listAccessors.Length != 0)
                throw new NotSupportedException();

            return _columns;
        }

        public string GetListName(PropertyDescriptor[] listAccessors) => null;

        public int IndexOf(object value)
        {
            if (value is ArrayRow row && row.Owner == this)
                return row.RowIndex;

            return -1;
        }

        public void Insert(int index, object value) => throw new NotSupportedException();
        public void Remove(object value) => throw new NotSupportedException();
        public void RemoveAt(int index) => throw new NotSupportedException();
        public void RemoveIndex(PropertyDescriptor property) { }
        public void RemoveSort() { }
    }
}
