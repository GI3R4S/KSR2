using System.Collections.Generic;

namespace Toolkit.AffiliationFunctions
{
    public interface IMembershipFunction
    {
        List<double> Parameters { get; set; }
        double GetMembership(double x);
    }
}