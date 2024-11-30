using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elin_GeneRecombinator
{
    public class WeightedRandomBag<T>
    {
        private struct Entry
        {
            public double accumulatedWeight;
            public T item;
        }

        private List<Entry> entries = new List<Entry>();
        private double accumulatedWeight;
        private Random rand = new Random();

        public void AddEntry(T item, double weight)
        {
            accumulatedWeight += weight;
            entries.Add(new Entry { item = item, accumulatedWeight = accumulatedWeight });
        }
        public bool Any() 
        {  
            return entries.Any(); 
        }

        public T GetRandom()
        {
            double r = rand.NextDouble() * accumulatedWeight;

            foreach (Entry entry in entries)
            {
                if (entry.accumulatedWeight >= r)
                {
                    return entry.item;
                }
            }
            return default(T); //should only happen when there are no entries
        }

        public T PopRandom()
        {
            double r = rand.NextDouble() * accumulatedWeight;

            foreach (Entry entry in entries.ToList())
            {
                if (entry.accumulatedWeight >= r)
                {
                    entries.Remove(entry);
                    return entry.item;
                }
            }
            return default(T); //should only happen when there are no entries
        }
    }
}
