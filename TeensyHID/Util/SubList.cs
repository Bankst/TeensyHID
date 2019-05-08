using System;
using System.Collections;
using System.Collections.Generic;

namespace TeensyHID.Util
{
    public class SubList<T> : IList<T>
    {
        #region Fields

        private readonly int _startIndex;
        private readonly int _endIndex;
        private readonly IList<T> _source;

        #endregion

        public SubList(IList<T> source, int startIndex, int count)
        {
            _source = source;
            _startIndex = startIndex;
            Count = count;
            _endIndex = _startIndex + Count - 1;
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            if (item != null)
            {
                for (var i = _startIndex; i <= _endIndex; i++)
                {
                    if (item.Equals(_source[i]))
                        return i;
                }
            }
            else
            {
                for (var i = _startIndex; i <= _endIndex; i++)
                {
                    if (_source[i] == null)
                        return i;
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                    return _source[index + _startIndex];
                throw new IndexOutOfRangeException("index");
            }
            set
            {
                if (index >= 0 && index < Count)
                    _source[index + _startIndex] = value;
                else
                    throw new IndexOutOfRangeException("index");
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = _source[i + _startIndex];
            }
        }

        public int Count { get; }

        public bool IsReadOnly => true;

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = _startIndex; i < _endIndex; i++)
            {
                yield return _source[i];
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
