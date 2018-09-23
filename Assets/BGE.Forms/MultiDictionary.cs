using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGE.Forms
{
    public class MultiDictionary<TKey, TValue>  // no (collection) base class
    {
        private Dictionary<TKey, List<TValue>> data = new Dictionary<TKey, List<TValue>>();

        public int Count = 0;

        public void Add(TKey k, TValue v)
        {
            // can be a optimized a little with TryGetValue, this is for clarity
            if (data.ContainsKey(k))
                data[k].Add(v);
            else
                data.Add(k, new List<TValue>() { v });
            Count++;
        }

        public TValue Get(TKey k)
        {
            return data[k][0];
        }

        public void Remove(TKey k, TValue v)
        {
            data[k].Remove(v);
            if (data[k].Count == 0)
            {
                data.Remove(k);
            }
            Count--;
        }

        public bool ContainsKey(TKey k)
        {
            return data.ContainsKey(k);
        }
    }
}
