using System;
using System.Collections;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{
    public class NodeGraphToolItemCollection : IList<NodeGraphToolItemData>
    {

        public NodeGraphToolItemCollection(string tag = "non_category")
        {
            _tag = tag;
            _collection = new List<NodeGraphToolItemData>();
        }

        private string _tag;
        public string TAG
        {
            get { return _tag; }
            set { _tag = value; }
        }

        private List<NodeGraphToolItemData> _collection;

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public NodeGraphToolItemData this[int index]
        {
            get { return _collection[index]; }
            set { _collection[index] = value; }
        }

        public int IndexOf(NodeGraphToolItemData item)
        {
            return _collection.IndexOf(item);
        }

        public void Insert(int index, NodeGraphToolItemData item)
        {
            _collection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
        }

        public void Add(NodeGraphToolItemData item)
        {
            _collection.Add(item);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(NodeGraphToolItemData item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(NodeGraphToolItemData[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(NodeGraphToolItemData item)
        {
            return _collection.Remove(item);
        }

        public IEnumerator<NodeGraphToolItemData> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

    }
}
