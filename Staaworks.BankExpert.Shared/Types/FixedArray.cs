using System.Collections;
using System.Linq;

namespace Staaworks.BankExpert.Shared.Types
{
    public class FixedArray<T> : IEnumerable
    {
        private readonly T[] array;
        private readonly int _maxLength;
        private int index = 0;
        private readonly T tnull;

        public FixedArray(int length, T tnull)
        {
            array = new T[length];
            _maxLength = length;
            this.tnull = tnull;
        }

        public void Add(T el)
        {
            if (index < _maxLength - 1)
            {
                array[index++] = el;
            }
            else
            {
                IndexToOldest();
                array[index] = el;
            }
        }


        private void IndexToOldest()
        {
            if (index == _maxLength - 1) // last element
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }


        private void RemoveOldestOccurrence(T[] labels)
        {
            var start = index;
            do
            {
                IndexToOldest();
                if (labels.Contains(array[index]))
                {
                    array[index] = tnull;
                    break;
                }
            }
            while (index != start);
        }



        public T GetMostPopularLabel()
        {
            // select pairs of label and counts from the array
            var groups = array.Where(e => !e.Equals(tnull)).GroupBy(e => e).Select(g => (label: g.Key, popularity: g.Count())).OrderBy(p => p.popularity).ToArray();

            if (groups.Length == 0)
            {
                return tnull;
            }
            else if (groups.Length == 1)
            {
                // only one label, just return it
                return groups[0].label;
            }
            else
            {
                var mostPopularCount = groups[0].popularity;
                var mostPopularLabels = groups.TakeWhile(el => el.popularity == mostPopularCount).Select(el => el.label).ToArray();
                if (mostPopularLabels.Length > 1)
                {
                    // we have equals, let us remove the oldest entry from the array that is either of both and try again
                    RemoveOldestOccurrence(mostPopularLabels);
                    return GetMostPopularLabel();
                }
                else
                {
                    return mostPopularLabels[0];
                }
            }
        }


        public T this[int index]
        {
            get => array[index];
        }
        
        IEnumerator IEnumerable.GetEnumerator () => array.GetEnumerator();
    }
}
