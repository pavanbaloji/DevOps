using System;
using System.Collections;
using System.Collections.Generic;

namespace Avista.ESB.Admin
{
  
    public class IEnumeratorWrapper<T> : IEnumerator<T>
    {
        protected IEnumerator enumerator_;

        public IEnumeratorWrapper(IEnumerator enumerator)
        {
            enumerator_ = enumerator;
        }

        #region IEnumerator<T> Members

        public virtual T Current
        {
            get { return (T) Item; }
        }

        public virtual bool MoveNext()
        {
            return enumerator_.MoveNext();
        }

        public virtual void Reset()
        {
            enumerator_.Reset();
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return Item; }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            /* no op */
            ;
        }

        #endregion

        protected virtual object Item
        {
            get { return enumerator_.Current; }
        }
    }

    public class IEnumerableWrapper<T> : IEnumerable<T>
    {
        private readonly IEnumerable enumerable_;

        public IEnumerableWrapper(IEnumerable enumerable)
        {
            enumerable_ = enumerable;
        }

        #region IEnumerable<T> Members

        public virtual IEnumerator<T> GetEnumerator()
        {
            return new IEnumeratorWrapper<T>(enumerable_.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<T>).GetEnumerator();
        }

        #endregion
    }

    public abstract class ICollectionBaseWrapper<T> : ICollection<T>, IEnumerable<T>
    {
        public abstract ICollection Collection { get; }

        #region ICollection<T> Members

        public virtual void Add(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual bool Contains(T item)
        {
            foreach (T t_item in Collection)
                if (t_item.Equals(item))
                    return true;

            return false;
        }

        public virtual void CopyTo(T[] array, int index)
        {
            Collection.CopyTo(array, index);
        }

        public virtual int Count
        {
            get { return Collection.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual bool Remove(T item)
        {
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        public virtual IEnumerator<T> GetEnumerator()
        {
            return new IEnumeratorWrapper<T>(Collection.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as ICollection<T>).GetEnumerator();
        }

        #endregion
    }

    public class IReadOnlyCollectionWrapper<T> : ICollectionBaseWrapper<T>
    {
        protected ICollection collection_;

        public IReadOnlyCollectionWrapper(ICollection collection)
        {
            collection_ = collection;
        }

        public override ICollection Collection
        {
            get { return collection_; }
        }

        #region ICollection<T> Members

        private new void Add(T item)
        {
            /* no op */
        }

        private new void Clear()
        {
            /* no op */
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        private new void Remove(T item)
        {
            /* no op */
        }

        #endregion
    }
}
