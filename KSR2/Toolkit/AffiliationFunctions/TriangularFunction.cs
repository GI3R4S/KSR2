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

            if (x > a && x <= b)
            {
                return (x - a) / (b - a);
            }
            else if (x > b && x <= c)
            {
                return (c - x) / (c - b);
            }
            else
                return 0;
        }


    }
}