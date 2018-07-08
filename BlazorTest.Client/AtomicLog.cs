using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorTest.Client
{
    internal sealed class AtomicLog<T>
    {
        private readonly List<T> items = new List<T>();
        private Action<IReadOnlyList<T>> onItemsAdded;

        public void Add(T item)
        {
            Action<IReadOnlyList<T>> handler;

            lock (items)
            {
                items.Add(item);
                handler = onItemsAdded;
            }

            handler?.Invoke(new[] { item });
        }

        public void AddRange(params T[] items) => AddRange((IEnumerable<T>)items);

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var buffered = items.ToList();
            if (buffered.Count == 0) return;

            Action<IReadOnlyList<T>> handler;

            lock (this.items)
            {
                this.items.AddRange(buffered);
                handler = onItemsAdded;
            }

            handler?.Invoke(buffered);
        }

        public IDisposable Subscribe(Action<IReadOnlyList<T>> onItemsAdded)
        {
            if (onItemsAdded == null) throw new ArgumentNullException(nameof(onItemsAdded));

            T[] previousEntries;
            lock (items)
            {
                previousEntries = items.ToArray();
                this.onItemsAdded += onItemsAdded;
            }

            if (previousEntries.Length != 0) onItemsAdded.Invoke(previousEntries);

            return On.Dispose(() =>
            {
                lock (items)
                {
                    this.onItemsAdded -= onItemsAdded;
                }
            });
        }
    }
}
