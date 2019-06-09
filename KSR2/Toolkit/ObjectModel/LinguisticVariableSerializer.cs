using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Toolkit
{
    public static class LinguisticVariableSerializer
    {
        public static void Serialize(string aPath, List<LinguisticVariable> aLinguisticVariables)
        {
            using (StreamWriter streamWriter = new StreamWriter(aPath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<LinguisticVariable>));
                xmlSerializer.Serialize(streamWriter, aLinguisticVariables);
            }
        }

        public static List<LinguisticVariable> Deserialize(string aPath)
        {
            List<LinguisticVariable> linguisticVariables = new List<LinguisticVariable>();
            using (StreamReader streamReader = new StreamReader(aPath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<LinguisticVariable>));
                linguisticVariables = (List<LinguisticVariable>)xmlSerializer.Deserialize(streamReader);

            }


            foreach (LinguisticVariable item in linguisticVariables)
            {
                item.MembershipFunction.Parameters = item.Parameters;
            }
            PrepareColumnDataExtractor(linguisticVariables);

            return linguisticVariables;

        }

        private static void PrepareColumnDataExtractor(List<LinguisticVariable> aLingusticVariables)
        {
            foreach (LinguisticVariable linguisticVariable in aLingusticVariables)
            {
                if (linguisticVariable.MemberToExtract != "")
                {
                    var getterMethodInfo = typeof(Record).GetProperty(linguisticVariable.MemberToExtract).GetGetMethod();
                    var entity = Expression.Parameter(typeof(Record));
                    var getterCall = Expression.Call(entity, getterMethodInfo);
                    LambdaExpression lambda = Expression.Lambda(Expression.Convert(getterCall, typeof(double)), entity);

                    linguisticVariable.Extractor = (Func<Record, double>)lambda.Compile();
                }
            }
        }
    }
}
