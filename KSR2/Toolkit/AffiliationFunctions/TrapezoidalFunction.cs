using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toolkit.AffiliationFunctions
{
    public class TrapezoidalFunction : IMembershipFunction
    {
        public List<double> Parameters { get; set; }

        public double GetMembership(double x)
        {
            double a = Parameters[0];
            double b = Parameters[1];
            double c = Parameters[2];
            double d = Parameters[3];

            if (x >= b && x <= c) return 1;

            if (x > a && x < b) return 1.0 / (b - a) * x + 1.0 - 1.0 / (b - a) * b;

            if (x > c && x < d) return 1.0 / (c - d) * x + 1.0 - 1.0 / (c - d) * c;

            return 0;
        }

        public TrapezoidalFunction(List<double> aArgs)
        {
            Debug.Assert(aArgs.Count == 4);
            Parameters = aArgs;
        }
    }
}