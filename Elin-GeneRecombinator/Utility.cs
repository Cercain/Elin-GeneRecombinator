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
            public double rawWeight;
            public double accumulatedWeight;
            public T item;
        }

        private List<Entry> entries = new List<Entry>();
        private double accumulatedWeight;
        private Random rand = new Random();

        public void AddEntry(T item, double weight)
        {
            accumulatedWeight += weight;
            entries.Add(new Entry { item = item, accumulatedWeight = accumulatedWeight, rawWeight = weight });
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
            Console.WriteLine($"[GeneRecomb][Debug] Popping");
            foreach (Entry entry in entries.ToList())
            {
                if (entry.accumulatedWeight >= r)
                {
                    entries.Remove(entry);
                    Reweight();
                    return entry.item;
                }
            }
            return default(T); //should only happen when there are no entries
        }

        private void Reweight()
        {
            var newentrylist = new List<Entry>();
            accumulatedWeight = 0;
            foreach(Entry entry in entries)
            {
                accumulatedWeight += entry.rawWeight;
                newentrylist.Add(new Entry { item = entry.item, accumulatedWeight = accumulatedWeight, rawWeight = entry.rawWeight});
            }
            entries= newentrylist;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Entry entry in entries)
            {
                sb.Append($"item:{entry.item.ToString()} acWeight:{entry.accumulatedWeight}");
            }
            return sb.ToString();
        }
    }
}
