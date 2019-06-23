using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Toolkit
{
    public class FuzzySet
    {
        public static List<string> DegreesLabels = new List<string>()
        {
            "T_1",
            "T_2",
            "T_3",
            "T_4",
            "T_5",
            "T_6",
            "T_7",
            "T_8",
            "T_9",
            "T_10",
            "T_11"
        };

        public static ClassicalSet<Record> DB = new ClassicalSet<Record>();
        public static int CountOfAllRecordsInDb = 58065;

        private Dictionary<Record, double> ResultMembership = new Dictionary<Record, double>();

        private Dictionary<Record, double> LocalAllRecordsMembership = new Dictionary<Record, double>();
        private ClassicalSet<Record> AllRecords = new ClassicalSet<Record>();
        public LinguisticVariable LinguisticVariable { get; set; } = new LinguisticVariable();

        public FuzzySet Qualifier;
        public FuzzySet AnotherSummarizator;

        public string RelationType { get; set; } = "";

        public FuzzySet(List<Record> aRecords, LinguisticVariable aLinguisticVariable, string aRelationType = "")
        {
            AllRecords = new ClassicalSet<Record>() { Elements = aRecords };
            LinguisticVariable = aLinguisticVariable;
            RelationType = aRelationType;

            foreach (Record record in AllRecords.Elements)
            {
                GetAffilationForRecord(record);
            }
        }

        public double GetAffilationForRecord(Record item)
        {
            if (LocalAllRecordsMembership.ContainsKey(item))
            {
                return LocalAllRecordsMembership[item];
            }

            Debug.Assert(LinguisticVariable.IsQuantifier() == false);
            double membership = LinguisticVariable.MembershipFunction.GetMembership(LinguisticVariable.Extractor(item));

            LocalAllRecordsMembership[item] = membership;
            return membership;
        }

        public List<Record> Support()
        {
            List<Record> filteredElements = AllRecords.Elements.Where(record => GetAffilationForRecord(record) > 0).ToList();
            return filteredElements;
        }

        #region Unused
        public List<Record> Core()
        {
            List<Record> filteredElements = Support().Where(record => GetAffilationForRecord(record) > 1).ToList();
            return filteredElements;
        }

        public double Height()
        {
            double height = Support().Max(record => GetAffilationForRecord(record));
            return height;
        }

        public List<Record> AlphaCut(double aMinivalValue)
        {
            Debug.Assert(aMinivalValue >= 0 && aMinivalValue <= 100);
            List<Record> filteredElements = Support().Where(record => GetAffilationForRecord(record) > aMinivalValue).ToList();
            return filteredElements;
        }

        public bool IsEmpty()
        {
            return Support().All(record => GetAffilationForRecord(record) == 0);
        }
        public double GetAffilationComplementForRecord(Record item)
        {
            return 1 - GetAffilationForRecord(item);
        }
        #endregion

        private void RefreshMap()
        {
            foreach (Record record in AllRecords.Elements)
            {
                GetAffilationForRecord(record);
            }
        }
        public double LocalCardinalNumber
        {
            get
            {
                RefreshMap();
                return LocalAllRecordsMembership.Sum(pair => pair.Value);
            }
        }

        public double GetSurfaceArea()
        {
            double height = 1;
            return LinguisticVariable.MembershipFunctionType == MembershipFunctionTypes.TriangularFunction ? (LinguisticVariable.Parameters[2] - LinguisticVariable.Parameters[0]) * height : ((LinguisticVariable.Parameters[3] - LinguisticVariable.Parameters[0]) * (LinguisticVariable.Parameters[2] - LinguisticVariable.Parameters[1])) * height / 2;
        }

        public double GlobalCardinalNumber
        {
            get
            {
                return ResultMembership.Sum(pair => pair.Value);
            }
        }

        public Dictionary<string, double> GetDegrees(LinguisticVariable aQuantifier)
        {
            Dictionary<string, double> degrees = new Dictionary<string, double>();

            if (AnotherSummarizator != null && (AnotherSummarizator.RelationType.Equals("AND") || AnotherSummarizator.RelationType.Equals("OR")))
            {
                for (int i = 0; i < AllRecords.Elements.Count; i++)
                {

                    if (AnotherSummarizator.RelationType == "AND")
                    {
                        double firstAffilation = GetAffilationForRecord(AllRecords.Elements[i]);
                        double secondAffilation = AnotherSummarizator.GetAffilationForRecord(AllRecords.Elements[i]);
                        ResultMembership.Add(AllRecords.Elements[i], firstAffilation > secondAffilation ? secondAffilation : firstAffilation);
                    }
                    else if (AnotherSummarizator.RelationType == "OR")
                    {
                        double firstAffilation = GetAffilationForRecord(AllRecords.Elements[i]);
                        double secondAffilation = AnotherSummarizator.GetAffilationForRecord(AllRecords.Elements[i]);
                        ResultMembership.Add(AllRecords.Elements[i], firstAffilation > secondAffilation ? firstAffilation : secondAffilation);
                    }
                }
            }
            else if (Qualifier != null)
            {
                Qualifier.RefreshMap();
                AllRecords = new ClassicalSet<Record>(Qualifier.Support());
                LocalAllRecordsMembership.Clear();
                RefreshMap();

            }
            else if (Qualifier == null && AnotherSummarizator == null)
            {

            }

            double supportCount = Support().Count;
            double allRecordsCount = AllRecords.Elements.Count;
            double qualifierSupportCount = Qualifier != null ? Qualifier.Support().Count : 0;
            double qualifierAllRecordsCount = Qualifier != null ? Qualifier.AllRecords.Elements.Count : 0;
            double anotherSummarizatorSupportCount = AnotherSummarizator != null ? AnotherSummarizator.Support().Count : 0;
            double anotherSummarizatorAllRecordsCount = AnotherSummarizator != null ? AnotherSummarizator.AllRecords.Elements.Count : 0;
            double complexSummarizationSupportCount;
            double complexSummarizationCardinality;

            if (AnotherSummarizator != null)
            {
                complexSummarizationSupportCount = ResultMembership.Count(p => p.Value != 0);
                complexSummarizationCardinality = GlobalCardinalNumber;
            }
            if (AnotherSummarizator != null && AnotherSummarizator.RelationType.Equals("OR"))
            {

            }

            // T_1 Degree of truth
            double r = 0;
            if (AnotherSummarizator != null && Qualifier == null)
            {
                r = GlobalCardinalNumber;
            }
            else if (AnotherSummarizator == null && Qualifier == null)
            {
                r = LocalCardinalNumber;
            }
            else if (Qualifier != null)
            {
                for (int i = 0; i < AllRecords.Elements.Count; i++)
                {
                    Record currentRecord = AllRecords.Elements[i];

                    double sumMem = GetAffilationForRecord(currentRecord);
                    double qualMem = Qualifier.GetAffilationForRecord(currentRecord);
                    r += sumMem > qualMem ? qualMem : sumMem;
                }
            }

            if (aQuantifier.Absoluteness.IsAbsolute)
            {
                double membership = aQuantifier.MembershipFunction.GetMembership(r);
                r = membership;
            }
            else
            {
                if (Qualifier != null)
                {
                    double relation = r / Qualifier.LocalCardinalNumber;
                    r = aQuantifier.MembershipFunction.GetMembership(relation);
                }
                else
                {
                    double relation = r / CountOfAllRecordsInDb;
                    r = aQuantifier.MembershipFunction.GetMembership(relation);
                }
            }
            degrees.Add(DegreesLabels[0], r);

            // T_2 Imprecision of summarizator

            var t2t8Min = DB.Elements.Min(p => LinguisticVariable.Extractor(p));
            var t2t8Max = DB.Elements.Max(p => LinguisticVariable.Extractor(p));
            var t2t8Dst = t2t8Max - t2t8Min;
            var t2t8Surface = LinguisticVariable.GetSurfaceArea();

            var t2t8AnotherMin = AnotherSummarizator != null ? DB.Elements.Min(p => AnotherSummarizator.LinguisticVariable.Extractor(p)) : 0;
            var t2t8AnotherMax = AnotherSummarizator != null ? DB.Elements.Max(p => AnotherSummarizator.LinguisticVariable.Extractor(p)) : 0;
            var t2t8AnotherDst = t2t8AnotherMax - t2t8AnotherMin;
            var t2t8AnotherSurface = AnotherSummarizator != null ? AnotherSummarizator.LinguisticVariable.GetSurfaceArea() : 0;

            double t2 = (LinguisticVariable.Parameters.Last() - LinguisticVariable.Parameters.First()) / t2t8Dst;

            if (AnotherSummarizator != null)
            {
                t2 *= (AnotherSummarizator.LinguisticVariable.Parameters.Last() - AnotherSummarizator.LinguisticVariable.Parameters.First()) / t2t8AnotherDst;
                t2 = Math.Pow(t2, 0.5);
            }
            t2 = 1 - t2;
            degrees.Add(DegreesLabels[1], t2);

            //T_3 Degree of covering
            if (Qualifier != null)
            {
                List<Record> qualifierSupport = Qualifier.Support();
                int intersectCount = new ClassicalSet<Record>(Support()).Intersect(qualifierSupport).Count;
                double t3 = intersectCount / qualifierSupportCount;
                degrees.Add(DegreesLabels[2], t3);
            }
            else
            {
                double t3 = 0.0;
                if (AnotherSummarizator == null)
                {
                    t3 = 1.0 * supportCount / CountOfAllRecordsInDb;
                }
                else
                {
                    int countOfSupport = ResultMembership.Count(pair => pair.Value > 0);
                    t3 = 1.0 * countOfSupport / CountOfAllRecordsInDb;
                }
                degrees.Add(DegreesLabels[2], t3);
            }

            // T_4 Degree of appropriateness

            double t4 = supportCount / CountOfAllRecordsInDb;
            if (AnotherSummarizator != null)
            {
                t4 *= (anotherSummarizatorSupportCount / anotherSummarizatorAllRecordsCount);
            }

            t4 -= degrees["T_3"];
            t4 = Math.Abs(t4);

            degrees.Add(DegreesLabels[3], t4);

            // T_5 
            double t5 = 0;
            t5 = 2 * Math.Pow(0.5, AnotherSummarizator != null ? 2 : 1);
            degrees.Add(DegreesLabels[4], t5);

            // T_6

            double distance = aQuantifier.Parameters.Last() -
                                     aQuantifier.Parameters.First();
            double totalRange = aQuantifier.Absoluteness.IsAbsolute ? CountOfAllRecordsInDb : 1;
            degrees.Add(DegreesLabels[5], 1 - distance / totalRange);

            // T_7
            double distance2 = aQuantifier.GetSurfaceArea();
            double totalRange2 = aQuantifier.Absoluteness.IsAbsolute ? CountOfAllRecordsInDb : 1;
            degrees.Add(DegreesLabels[6], 1 - distance2 / totalRange2);

            // T_8
            double t8 = t2t8Surface / t2t8Dst;
            if (AnotherSummarizator != null)
            {
                t8 *= t2t8AnotherSurface / t2t8AnotherDst;
                t8 = Math.Pow(t8, 1f / 2);
            }
            degrees.Add(DegreesLabels[7], 1 - t8);

            if (Qualifier != null)
            {

                var t9t10Min = DB.Elements.Min(p => Qualifier.LinguisticVariable.Extractor(p));
                var t9t10Max = DB.Elements.Max(p => Qualifier.LinguisticVariable.Extractor(p));
                var t9t10Dst = t9t10Max - t9t10Min;
                var t9t10Surface = Qualifier.LinguisticVariable.GetSurfaceArea();

                // T_9 Imprecision of qualifier
                double t9 = 1 - ((Qualifier.LinguisticVariable.Parameters.Last() - Qualifier.LinguisticVariable.Parameters.First()) / t9t10Dst);
                degrees.Add(DegreesLabels[8], t9);


                // T_10 Cardinality of qualifier
                double t10 = 1 - t9t10Surface / t9t10Dst;

                degrees.Add(DegreesLabels[9], t10);

                // T_11 Length of qualifier
                degrees.Add(DegreesLabels[10], 2 * Math.Pow(0.5, 1));
            }

            ResultMembership.Clear();
            return degrees;
        }

    }
}