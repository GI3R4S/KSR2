using System.Collections.Generic;

namespace Toolkit
{
    public interface IMembershipFunction
    {
        List<double> Parameters { get; set; }
        double GetMembership(double x);
    }
}