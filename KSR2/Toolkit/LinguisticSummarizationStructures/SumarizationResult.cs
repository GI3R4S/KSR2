using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit
{
    public class SummarizationResult
    {
        public List<string> Summaries
        {
            get
            {
                return Pairs.OrderByDescending(p => p.Value).Select(p => p.Key).ToList();
            }
        }
        public List<KeyValuePair<string, double>> Pairs { get; set; }  = new List<KeyValuePair<string, double>>();
    }
}
