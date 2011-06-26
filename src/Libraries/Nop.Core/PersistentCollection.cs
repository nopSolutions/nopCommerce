using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core
{
    public class PersistentCollection<T> : IPersistentCollection<T>
         where T : class
    {
        private readonly ICollection<T> _actual;
        private Action<ICollection<T>> _afterAdd;
        private Action<ICollection<T>> _afterRemove;
        private Func<ICollection<T>, T, bool> _beforeAdd;
        private Func<ICollection<T>, T, bool> _beforeRemove;

        public PersistentCollection(ICollection<T> actual)
        {
            _actual = actual;
        }

        /// <summary>
        ///     perform actions on one or more list items after an item is
        ///     added.
        /// </summary>
        public Action<ICollection<T>> AfterAdd
        {
            get { return _afterAdd ?? (_afterAdd = l => { }); }
            set { _afterAdd = value; }
        }

        /// <summary>
        ///     perform actions on one or more list items after an item is
        ///     removed.
        /// </summary>
        public Action<ICollection<T>> AfterRemove
        {
            get { return _afterRemove ?? (_afterRemove = l => { }); }
            set { _afterRemove = value; }
        }

        /// <summary>
        ///     perform a check on the item being added before adding it.
        ///     Return true if it should be added, false if it should not be
        ///     added.
        /// </summary>
        public Func<ICollection<T>, T, bool> BeforeAdd
        {
            get { return _beforeAdd ?? (_beforeAdd = (l, x) => true); }
            set { _beforeAdd = value; }
        }

        /// <summary>
        ///     perform a check on the item being removed before removing
        ///     it. Return true if it should be removed, false if it should not
        ///     be removed.
        /// </summary>
        public Func<ICollection<T>, T, bool> BeforeRemove
        {
            get { return _beforeRemove ?? (_beforeRemove = (l, x) => true); }
            set { _beforeRemove = value; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _actual.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public int Count
        {
            get { return _actual.Count; }
        }
        public bool IsReadOnly
        {
            get { return _actual.IsReadOnly; }
        }

        public void Add(T item)
        {
            if (BeforeAdd(this, item))
            {
                _actual.Add(item);
                AfterAdd(this);
            }
        }

        public void Clear()
        {
            while (_actual.Any())
            {
                Remove(_actual.First());
            }
        }

        public bool Contains(T item)
        {
            return _actual.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _actual.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (BeforeRemove(this, item))
            {
                bool toReturn = _actual.Remove(item);
                AfterRemove(this);
                return toReturn;
            }
            return true;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            var copy = new T[_actual.Count];
            _actual.CopyTo(copy, 0);
            Array.Copy(copy, 0, array, index, _actual.Count);
        }

        int ICollection.Count
        {
            get { return _actual.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
    }
}
