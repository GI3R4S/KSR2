﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit
{
    public class SummarizationResult
    {
        public string BestSummarization { get; set; }
        public List<string> AllSummarizations { get; set; } = new List<string>();
    }
}
