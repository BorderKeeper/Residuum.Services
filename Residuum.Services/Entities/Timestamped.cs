using System;

namespace Residuum.Services.Entities
{
    public class Timestamped<T>
    {
        public T Item { get; set; }

        public DateTime LastUpdated { get; set; }

        public Timestamped()
        {
            LastUpdated = DateTime.Now;
        }

        public Timestamped(T item)
        {
            Item = item;
            LastUpdated = DateTime.Now;
        }
    }
}