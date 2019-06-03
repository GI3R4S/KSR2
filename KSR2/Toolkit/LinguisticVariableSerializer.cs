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
            List<LinguisticVariable> items = new List<LinguisticVariable>();
            using (StreamReader streamReader = new StreamReader(aPath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<LinguisticVariable>));
                items = (List<LinguisticVariable>)xmlSerializer.Deserialize(streamReader);

            }
            foreach (var item in items)
            {
                item.MembershipFunction.Parameters = item.Parameters;
            }

            foreach (var item in items)
            {
                if (item.MemberToExtract != "")
                {
                    var getterMethodInfo = typeof(Record).GetProperty(item.MemberToExtract).GetGetMethod();
                    var entity = Expression.Parameter(typeof(Record));
                    var getterCall = Expression.Call(entity, getterMethodInfo);
                    var castToObject = Expression.Convert(getterCall, typeof(double));
                    var lambda = Expression.Lambda(castToObject, entity);

                    item.Extractor = (Func<Record, double>)lambda.Compile();
                }
            }
            return items;

        }
    }
}
