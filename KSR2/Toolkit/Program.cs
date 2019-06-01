using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Toolkit
{
    public class Program
    {
        public static void Main(string [] args)
        {
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<LinguisticVariable>));
            LinguisticVariable linguisticVariable = new LinguisticVariable
            {
                MembershipFunctionType = AffiliationFunctions.MembershipFunctionTypes.TrapezoidalFunction,
                Name = "Nazwa",
                MemberToExtract = "Member",
                Parameters = new List<double>() { 1, 2.2222, 3.312312, 4 }
            };
            LinguisticVariable linguisticVariable2 = new LinguisticVariable
            {
                MembershipFunctionType = AffiliationFunctions.MembershipFunctionTypes.TriangularFunction,
                Name = "Nazwa",
                MemberToExtract = "Member",
                Parameters = new List<double>() { 1.11, 2, 3.421, 4 }
            };

            List<LinguisticVariable> variables = new List<LinguisticVariable>() { linguisticVariable, linguisticVariable2 };

            StreamWriter stream = new StreamWriter("out.xml");
            xmlSerializer.Serialize(stream, variables);
        }
    }
}
