using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toolkit.AffiliationFunctions
{
    public class DiscreteFunction : IMembershipFunction
    {
        private Dictionary<int, double> Membership = new Dictionary<int, double>();
        public List<double> Parameters { get; set; } 

        public double GetMembership(double x)
        {
            return Membership[(int) x];
        }

        public DiscreteFunction(List<double> aArgs)
        {
            Debug.Assert(aArgs.Count >= 2 && aArgs.Count % 2 == 1);
            Parameters = aArgs;

            for (int i = 0; i < aArgs.Count; i += 2)
            {
                Membership[(int)aArgs[i]] = aArgs[i + 1];
            }
        }
    }
}