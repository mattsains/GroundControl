using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl
{
    class Heap<Item>
    {
        List<HeapEntry<Item>> heap=new List<HeapEntry<Item>>();

        public IEnumerator<HeapEntry<Item>> GetEnumerator() { return heap.GetEnumerator(); }
        public int Count { get { return heap.Count; } }

        public void Push(Item item, int priority)
        {
            HeapEntry<Item> n=new HeapEntry<Item>(priority,item);
            for (int i = 0; i < heap.Count; i++)
            {
                if (heap[i].CompareTo(n) > 0)
                {
                    heap.Insert(i, n);
                    return;
                }
            }
            heap.Add(n);
        }
        public Item Peak()
        {
            HeapEntry<Item> gone = heap[0];
            return gone.Item;
        }
        public Item Pop()
        {
            HeapEntry<Item> gone = heap[0];
            heap.Remove(gone);
            return gone.Item;
        }
        public bool Contains(Item item)
        {
            foreach (HeapEntry<Item> entry in heap)
                if (entry.Item.Equals(item)) return true;
            return false;
        }
        public void Remove(Item item)
        {
            foreach (HeapEntry<Item> entry in heap)
                if (entry.Item.Equals(item))
                {
                    heap.Remove(entry);
                    return;
                }
        }
    }
    class HeapEntry<I>
    {
        public int Priority { get; set; }
        public I Item { get; set; }

        public HeapEntry(int priority, I item)
        {
            this.Priority = priority;
            this.Item = item;
        }
        public int CompareTo(HeapEntry<I> that)
        {
            return this.Priority - that.Priority;
        }
        public override string ToString()
        {
            return string.Format("{0} - {1}", Priority, Item);
        }
    }
}
