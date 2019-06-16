using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Toolkit;

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

        public Absoluteness Absoluteness { get; set; } = new Absoluteness();

        public bool IsQuantifier()
        {
            return MemberToExtract == "" ? true : false;
        }

        public double GetSurfaceArea()
        {
            double height = 1;
            return MembershipFunctionType == MembershipFunctionTypes.TriangularFunction ? (Parameters[2] - Parameters[0]) * height : ((Parameters[3] - Parameters[0]) * (Parameters[2] - Parameters[1])) * height / 2;
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
