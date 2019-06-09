using System.Collections.Generic;
using System.Diagnostics;

namespace Toolkit.AffiliationFunctions
{
    public class TrapezoidalFunction : IMembershipFunction
    {
        public TrapezoidalFunction(List<double> aArgs)
        {
            Debug.Assert(aArgs.Count == 4);
            Parameters = aArgs;
        }

        public TrapezoidalFunction()
        {
        }

        public List<double> Parameters { get; set; } = new List<double>();

        public double GetMembership(double x)
        {
            double a = Parameters[0];
            double b = Parameters[1];
            double c = Parameters[2];
            double d = Parameters[3];

            double val = 0;

            if (x > b && x <= c)
            {
                val = 1;
            }
            else if (x > a && x <= b)
            {
                val = (x - a) / (b - a);
            }
            else if (x > c && x <= d)
            {
                val = (d - x) / (d - c);
            }
            else
                val = 0;

            return val;
        }
    }
}