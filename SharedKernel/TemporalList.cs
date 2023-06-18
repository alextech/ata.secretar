using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedKernel
{
    public class TemporalList<T> : List<T> where T : ITemporal, new()
    {
        public T AsOf(DateTimeOffset date)
        {
            T foundItem = default;
            IOrderedEnumerable<T> ordered = this.OrderBy(i => i.AsOf);

            foreach (T item in ordered)
            {
                if (item.AsOf.Date <= date)
                {
                    foundItem = item;
                }
                else
                {
                    break;
                }
            }

            if (foundItem == null)
            {
                foundItem = new T();
            }

            return foundItem;
        }
    }
}