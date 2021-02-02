using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace SpotNetCore.Implementation
{
    public sealed class CyclicLimitedList<T> : ObservableCollection<T>
    {
        private int _activeIndex = -1;
        private readonly int _limit;

        public int ActiveIndex => _activeIndex;

        public CyclicLimitedList(int limit = 10)
        {
            _limit = limit;
            CollectionChanged += OnCollectionChanged;
        }

        public CyclicLimitedList(IEnumerable<T> collection, int limit = 10) : base(collection)
        {
            _limit = limit;
            CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems == null)
                    return;
                
                var count = e.NewStartingIndex;
                foreach (var eNewItem in e.NewItems)
                {
                    var persistedItems = this.Where(x => x.Equals((T) eNewItem));
                    if (persistedItems.Count() > 1)
                    {
                        RemoveAt(count);
                        MoveItem(IndexOf((T) eNewItem), Count - 1);
                    }

                    count++;
                }

                while (Count > _limit)
                    RemoveAt(0);
            }
        }

        public T GetCurrent()
        {
            if (Count == 0) throw new InvalidOperationException("List is empty");

            return base[_activeIndex < Count - 1 ? _activeIndex >= 0 ? _activeIndex : 0 : Count - 1];
        }

        public T GetNext()
        {
            if (Count == 0) throw new InvalidOperationException("List is empty");

            return this[_activeIndex + 1 < Count ? _activeIndex + 1 : 0];
        }

        public T GetPrevious()
        {
            if (Count == 0) throw new InvalidOperationException("List is empty");

            return this[_activeIndex - 1 >= 0 ? _activeIndex - 1 : Count - 1];
        }

        private new T this[int index]
        {
            get
            {
                var value = base[index];
                _activeIndex = index;
                return value;
            }
        }
    }
}