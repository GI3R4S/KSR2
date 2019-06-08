using System;
using System.Collections.Generic;
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
                MapEnumToMembershipFunctionInstance();
            }
        }

        public List<double> Parameters = new List<double>();
        public string MemberToExtract { get; set; }

        public bool IsQuantifier()
        {
            return MemberToExtract == "" ? true : false;
        }

        [XmlIgnore]
        public IMembershipFunction MembershipFunction { get; set; }

        [XmlIgnore]
        public Func<Record, double> Extractor { get; set; }

        #region PrivateMembers
        [XmlIgnore]
        private MembershipFunctionTypes _MembershipFunctionType;

        private void MapEnumToMembershipFunctionInstance()
        {
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
        #endregion
    }
}
