using System;
using Unity.Collections;

namespace Core.Misc.WorldMap.PathFinding
{
    public struct NativePriorityQueue<T>
        where T : unmanaged, IEquatable<T>, IComparable, IComparable<T>
    {
        private NativeList<T> baseContainer;
        public NativeList<T> BaseContainer => baseContainer;


        public NativePriorityQueue(Allocator allocator) =>
            this.baseContainer = new NativeList<T>(allocator);

        public NativePriorityQueue(int initialCapacity, Allocator allocator) =>
            this.baseContainer = new NativeList<T>(initialCapacity, allocator);

        public void SetBaseContainer(NativeList<T> baseContainer) => this.baseContainer = baseContainer;

        public T this[int key]
        {
            get => this.baseContainer[key];
            set => this.baseContainer[key] = value;
        }

        /// <summary>
        /// Pop the element with lowest value.
        /// </summary>
        public T Pop()
        {
            int removedIndex = this.baseContainer.Length - 1;
            T removedElement = this.baseContainer[removedIndex];
            this.baseContainer.RemoveAt(removedIndex);

            return removedElement;

        }

        public bool TryPop(out T item)
        {
            if (this.baseContainer.IsEmpty)
            {
                item = default;
                return false;
            }

            item = this.Pop();
            return true;

        }

        public void Add(T newItem)
        {
            int length = this.baseContainer.Length;

            if (length == 0)
            {
                this.baseContainer.Add(newItem);
                return;
            }

            for (int i = 0; i < length; i++)
            {
                T element = this.baseContainer[i];

                int comparator = newItem.CompareTo(element);
                bool newItemHasLowerValue = comparator == -1;

                if (newItemHasLowerValue) continue;

                this.baseContainer.InsertRange(i, 1);
                this.baseContainer[i] = newItem;
                return;

            }

            this.baseContainer.Add(newItem);

        }

        public bool Contains(T item)
        {
            int length = this.baseContainer.Length;
            for (int i = 0; i < length; i++)
            {
                T element = this.baseContainer[i];

                if (!element.Equals(item)) continue;
                
                return true;

            }

            return false;

        }
        
        public bool Contains(T item, out int firstIndex)
        {
            int length = this.baseContainer.Length;
            for (int i = 0; i < length; i++)
            {
                T element = this.baseContainer[i];

                if (!element.Equals(item)) continue;
                
                firstIndex = i;
                return true;

            }

            firstIndex = -1;
            return false;

        }

        public void Clear() => this.baseContainer.Clear();

        public void Dispose() => this.baseContainer.Dispose();

    }

}
