using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toolkit.AffiliationFunctions
{
    public class TriangularFunction : IMembershipFunction
    {
        public TriangularFunction(List<double> aArgs)
        {
            if (aArgs.Count != 3)
            {
                Console.Write("Alert!!!");
            }
            Debug.Assert(aArgs.Count == 3);
            Parameters = aArgs;
        }

        public TriangularFunction()
        {
        }

        public List<double> Parameters { get; set; } = new List<double>();

        public double GetMembership(double x)
        {
            double a = Parameters[0];
            double b = Parameters[1];
            double c = Parameters[2];

            if (Math.Abs(x - b) < .00001)
            {
                return 1;
            }

            if (x > a && x < b)
            {
                return 1.0 / (b - a) * x + 1.0 - 1.0 / (b - a) * b;
            }

            if (x > b && x < c)
            {
                return 1.0 / (b - c) * x + 1.0 - 1.0 / (b - c) * b;
            }

            return 0;
        }


    }
}