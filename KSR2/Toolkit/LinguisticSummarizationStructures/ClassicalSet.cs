using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toolkit.LinguisticSummarizationStructures
{
    public class ClassicalSet<T>
    {
        public List<T> Elements { get; set; } = new List<T>();

        public ClassicalSet(List <T> aElements)
        {
            Elements = aElements;
        }

        public List<T> Complement(List<T> aUniverse)
        {
            bool isWholeContained = true;
            foreach(T item in Elements)
            {
                if(!aUniverse.Contains(item))
                {
                    isWholeContained = false;
                    break;
                }
            }
            Debug.Assert(isWholeContained);

            List<T> filtered = aUniverse.Except(Elements).ToList();
            return filtered;
        }

        public List<T> Sum(List<T> aAnother)
        {
            List<T> filtered = new List<T>();
            filtered.AddRange(Elements);
            filtered.AddRange(aAnother);
            filtered = filtered.Distinct().ToList();
            return filtered;
        }

        public List<T> Diffrence(List<T> aAnother)
        {
            return Elements.Except(aAnother).ToList();
        }
        public List<T> Intersect(List<T> aAnother)
        {
            return Elements.Intersect(aAnother).ToList();
        }

        public int CharacteristicFunction(T aItem)
        {
            return Elements.Contains(aItem) ? 1 : 0;
        }
    }
}
