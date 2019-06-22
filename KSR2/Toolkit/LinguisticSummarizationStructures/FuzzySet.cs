using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toolkit;

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

        public static int CountOfAllRecordsInDb = 58065;

        private Dictionary<Record, double> ResultMembership = new Dictionary<Record, double>();

        private Dictionary<Record, double> LocalAllRecordsMembership = new Dictionary<Record, double>();
        private ClassicalSet<Record> AllRecords = new ClassicalSet<Record>();
        public LinguisticVariable LinguisticVariable { get; set; } = new LinguisticVariable();

        public FuzzySet Qualificator;
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
            else if (Qualificator != null)
            {
                Qualificator.RefreshMap();
                AllRecords = new ClassicalSet<Record>(Qualificator.Support());
                LocalAllRecordsMembership.Clear();
                RefreshMap();

            }
            else if (Qualificator == null && AnotherSummarizator == null)
            {

            }

            double supportCount = Support().Count;
            double allRecordsCount = AllRecords.Elements.Count;
            double qualificatorSupportCount = Qualificator != null ? Qualificator.Support().Count : 0;
            double qualificatorAllRecordsCount = Qualificator != null ? Qualificator.AllRecords.Elements.Count : 0;
            double anotherSummarizatorSupportCount = AnotherSummarizator != null ? AnotherSummarizator.Support().Count : 0;
            double anotherSummarizatorAllRecordsCount = AnotherSummarizator != null ? AnotherSummarizator.AllRecords.Elements.Count : 0;
            double complexSummarizationSupportCount;
            double complexSummarizationCardinality;

            if (AnotherSummarizator != null)
            {
                complexSummarizationSupportCount = ResultMembership.Count(p => p.Value != 0);
                complexSummarizationCardinality = GlobalCardinalNumber;
            }
            if(AnotherSummarizator != null && AnotherSummarizator.RelationType.Equals("OR"))
            {

            }
            // T_1
            double r = 0;
            if (AnotherSummarizator != null && Qualificator == null)
            {
                r = GlobalCardinalNumber;
            }
            else if (AnotherSummarizator == null && Qualificator == null)
            {
                r = LocalCardinalNumber;
            }
            else if (Qualificator != null)
            {
                for (int i = 0; i < AllRecords.Elements.Count; i++)
                {
                    Record currentRecord = AllRecords.Elements[i];

                    double sumMem = GetAffilationForRecord(currentRecord);
                    double qualMem = Qualificator.GetAffilationForRecord(currentRecord);
                    r += sumMem > qualMem ? qualMem : sumMem;
                }
            }

            if(aQuantifier.Absoluteness.IsAbsolute)
            {
                double membership = aQuantifier.MembershipFunction.GetMembership(r);
                r = membership;
            }
            else
            {
                double relation = r / CountOfAllRecordsInDb;
                r = aQuantifier.MembershipFunction.GetMembership(relation);
            }
            degrees.Add(DegreesLabels[0], r);

            // T_2
            double t2 = 1;
            if (AnotherSummarizator == null)
            {
                t2 -= (supportCount / allRecordsCount);
            }
            else
            {
                double multiplication = (supportCount / allRecordsCount) * (anotherSummarizatorSupportCount / anotherSummarizatorAllRecordsCount);
                t2 -= Math.Pow(multiplication, (1f / 2));
            }
            degrees.Add(DegreesLabels[1], t2);

            //T_3
            if (Qualificator != null)
            {
                List<Record> qualificatorSupport = Qualificator.Support();
                int intersectCount = new ClassicalSet<Record>(Support()).Intersect(qualificatorSupport).Count;
                double t3 = intersectCount / qualificatorSupportCount;
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
                    t3 =  1.0 * countOfSupport / CountOfAllRecordsInDb;
                }
                degrees.Add(DegreesLabels[2], t3);
            }

            // T_4
            double t4 = 0;
            if(AnotherSummarizator == null)
            {
                t4 = supportCount / allRecordsCount; ;
            }
            else
            {
                t4 = supportCount / allRecordsCount * (anotherSummarizatorSupportCount / anotherSummarizatorAllRecordsCount);
            }

            if(Qualificator != null)
            {
                t4 -= degrees["T_3"];
            }

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
            double t8 = LocalCardinalNumber / allRecordsCount;
            if (AnotherSummarizator != null)
            {
                t8 *= AnotherSummarizator.LocalCardinalNumber / anotherSummarizatorAllRecordsCount;
                t8 = Math.Pow(t8, 1f / 2);
            }
            degrees.Add(DegreesLabels[7], 1 - t8);

            if (Qualificator != null)
            {
                // T_9
                double t9 = 1 - (qualificatorSupportCount / qualificatorAllRecordsCount);
                degrees.Add(DegreesLabels[8], t9);


                // Meassure T_10
                double t10 = 1 - Qualificator.LocalCardinalNumber / qualificatorAllRecordsCount;
                degrees.Add(DegreesLabels[9], t10);

                // Meassure T_11
                degrees.Add(DegreesLabels[10], 2 * Math.Pow(0.5, 1));
            }

            ResultMembership.Clear();
            return degrees;
        }

    }
}