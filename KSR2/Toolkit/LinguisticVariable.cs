using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.AffiliationFunctions;

namespace Toolkit
{
    public class LinguisticVariable
    {
        public string Name { get; set; }
        public MembershipFunctionTypes MembershipFunctionType { get; set; }
        public List<double> Parameters { get; set; }
        public string MemberToExtract { get; set; }
    }
}
