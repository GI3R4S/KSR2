using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toolkit.AffiliationFunctions
{
    public class TriangularFunction : IMembershipFunction
    {
        public List<double> Parameters { get; set; }

        public double GetMembership(double x)
        {
            double a = Parameters[0];
            double b = Parameters[0];
            double c = Parameters[0];
   
            if (Math.Abs(x - b) < .00001) return 1;

            if (x > a && x < b) return 1.0 / (b - a) * x + 1.0 - 1.0 / (b - a) * b;

            if (x > b && x < c) return 1.0 / (b - c) * x + 1.0 - 1.0 / (b - c) * b;

            return 0;
        }

        public TriangularFunction(List<double> aArgs)
        {
            Debug.Assert(aArgs.Count == 3);
            Parameters = aArgs;
        }
    }
}