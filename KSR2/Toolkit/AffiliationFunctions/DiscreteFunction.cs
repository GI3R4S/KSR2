using System.Collections.Generic;
using System.Diagnostics;

namespace Toolkit.AffiliationFunctions
{
    public class DiscreteFunction : IMembershipFunction
    {
        public DiscreteFunction(List<double> aArgs)
        {
            Debug.Assert(aArgs.Count >= 2 && aArgs.Count % 2 == 1);
            Parameters = aArgs;

            for (int i = 0; i < aArgs.Count; i += 2)
            {
                Membership[aArgs[i]] = aArgs[i + 1];
            }
        }

        public DiscreteFunction()
        {
        }

        private Dictionary<double, double> Membership = new Dictionary<double, double>();
        public List<double> Parameters { get; set; } = new List<double>();

        public double GetMembership(double x)
        {
            if(Membership.ContainsKey(x))
            {
                return Membership[x];
            }
            else
            {
                return 0;
            }
        }
    }
}