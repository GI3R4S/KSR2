using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Toolkit.AffiliationFunctions;

namespace Toolkit
{
    public class LinguisticVariable
    {
        public string Name { get; set; }
        public MembershipFunctionTypes MembershipFunctionType
        {
            get
            {
                return _MembershipFunctionType;
            }
            set
            {
                _MembershipFunctionType = value;
                switch (MembershipFunctionType)
                {
                    case MembershipFunctionTypes.TriangularFunction:
                        {
                            MembershipFunction = new TriangularFunction();
                            break;
                        }
                    case MembershipFunctionTypes.TrapezoidalFunction:
                        {
                            MembershipFunction = new TrapezoidalFunction();
                            break;
                        }
                    case MembershipFunctionTypes.DiscreteFunction:
                        {
                            MembershipFunction = new DiscreteFunction();
                            break;
                        }
                }
            }
        }
        private List<double> _Parameters = new List<double>();
        public List<double> Parameters
        {
            get
            {
                return _Parameters;
            }
            set
            {
                _Parameters = value;
                MembershipFunction.Parameters = Parameters;
            }
        }
        public string MemberToExtract { get; set; }
        [XmlIgnore]
        private MembershipFunctionTypes _MembershipFunctionType;

        [XmlIgnore]
        public IMembershipFunction MembershipFunction { get; set; }

        [XmlIgnore]
        public bool IsQuantifier
        {
            get
            {
                return MemberToExtract != "" ? true : false;
            }
        }
        [XmlIgnore]
        public Func<Record, double> Extractor { get; set; }

        //[OnDeserialized]
        //internal void OnSerializingMethod(StreamingContext context)
        //{
        //    MembershipFunction.Parameters = Parameters;
        //    if (MemberToExtract != "")
        //    {
        //        var getterMethodInfo = typeof(Record).GetProperty(MemberToExtract).GetGetMethod();
        //        var entity = Expression.Parameter(typeof(Record));
        //        var getterCall = Expression.Call(entity, getterMethodInfo);
        //        var castToObject = Expression.Convert(getterCall, typeof(double));
        //        var lambda = Expression.Lambda(castToObject, entity);

        //        Extractor = (Func<Record, double>)lambda.Compile();
        //    }
        //}
    }
}
