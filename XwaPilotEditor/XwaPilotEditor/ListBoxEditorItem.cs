using System;

namespace XwaPilotEditor
{
    public class ListBoxEditorItem<T>
    {
        private T[] _data;

        private int _index;

        public ListBoxEditorItem(T[] data, int index)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _data = data;
            _index = index;
        }

        public override string ToString()
        {
            if (_index >= _data.Length)
            {
                return string.Empty;
            }

            return _data[_index].ToString();
        }

        public T NewValue
        {
            get
            {
                if (_index >= _data.Length)
                {
                    return default;
                }

                return _data[_index];
            }

            set
            {
                if (_index >= _data.Length)
                {
                    return;
                }

                _data[_index] = value;
            }
        }
    }
}
