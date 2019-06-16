using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toolkit
{
    public class TriangularFunction : IMembershipFunction
    {
        public TriangularFunction(List<double> aArgs)
        {
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

            double val = 0;

            if (x > a && x <= b)
            {
                val = (x - a) / (b - a);
            }
            else if (x > b && x <= c)
            {
                val = (c - x) / (c - b);
            }
            else
                val = 0;
            if(val > 1)
            {

            }
            return val;

        }


    }
}