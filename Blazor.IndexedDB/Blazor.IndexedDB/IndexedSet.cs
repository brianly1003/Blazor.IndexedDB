﻿using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Blazor.IndexedDB
{
    public class IndexedSet<T> : ICollection<T>, IEnumerable<T> where T : new()
    {
        /// <summary>
        /// The internal stored items
        /// </summary>
        private readonly IList<IndexedEntity<T>> internalItems;
        /// <summary>
        /// The type T primary key, only != null if at least once requested by remove
        /// </summary>
        private PropertyInfo primaryKey;

        // ToDo: Remove PK dependency
        public IndexedSet(IEnumerable<T> records, PropertyInfo primaryKey)
        {
            this.primaryKey = primaryKey;
            this.internalItems = new List<IndexedEntity<T>>();

            if (records == null)
            {
                return;
            }

            Debug.WriteLine($"{nameof(IndexedEntity)} - Construct - Add records");

            foreach (var item in records)
            {
                var indexedItem = new IndexedEntity<T>(item)
                {
                    State = EntityState.Unchanged
                };

                this.internalItems.Add(indexedItem);
            }

            Debug.WriteLine($"{nameof(IndexedEntity)} - Construct - Add records DONE");
        }

        public bool IsReadOnly => false;

        public int Count => this.internalItems.Count();

        public void Add(T item)
        {
            if (!this.internalItems.Select(x => x.Instance).Contains(item))
            {
                Debug.WriteLine($"{nameof(IndexedEntity)} - Added item of type {typeof(T).Name}");

                this.internalItems.Add(new IndexedEntity<T>(item)
                {
                    State = EntityState.Added
                });
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            foreach (var item in this)
            {
                this.Remove(item);
            }
        }

        public bool Contains(T item)
        {
            return Enumerable.Contains(this, item);
        }

        public bool Remove(T item)
        {
            var internalItem = this.internalItems.FirstOrDefault(x => x.Instance.Equals(item));

            if (internalItem != null)
            {
                internalItem.State = EntityState.Deleted;

                return true;
            }
            // If reference was lost search for pk, increases the required time
            else
            {
                Debug.WriteLine("Searching for equality with PK");

                var value = this.primaryKey.GetValue(item);

                internalItem = this.internalItems.FirstOrDefault(x => this.primaryKey.GetValue(x.Instance).Equals(value));

                if (internalItem != null)
                {
                    Debug.WriteLine($"Found item with id {value}");

                    internalItem.State = EntityState.Deleted;

                    return true;
                }
            }

            Debug.WriteLine("Could not find internal stored item");
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.internalItems.Select(x => x.Instance).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var enumerator = this.GetEnumerator();

            return enumerator;
        }

        // ToDo: replace change tracker with better alternative 
        internal IEnumerable<IndexedEntity> GetChanged()
        {
            foreach (var item in this.internalItems)
            {
                item.DetectChanges();

                if (item.State == EntityState.Unchanged)
                {
                    continue;
                }

                Debug.WriteLine("Item yield");
                yield return item;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Not enough elements after arrayIndex in the destination array.");

            for (int i = 0; i < Count; ++i)
                array[i + arrayIndex] = this.internalItems[i].Instance;
        }
    }
}
