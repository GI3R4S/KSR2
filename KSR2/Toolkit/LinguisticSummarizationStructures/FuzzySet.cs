using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toolkit.LinguisticSummarizationStructures;

namespace Toolkit
{
    public class FuzzySet
    {
        private Dictionary<Record, double> ResultMembership = new Dictionary<Record, double>();

        private Dictionary<Record, double> LocalAllRecordsMembership = new Dictionary<Record, double>();
        private ClassicalSet<Record> AllRecords = new ClassicalSet<Record>();
        public LinguisticVariable LinguisticVariable { get; set; } = new LinguisticVariable();

        public FuzzySet Qualificator;
        public FuzzySet AnotherSummarizator;

        public Func<List<double>, double> OperatorFunction { get; set; }
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

        public double GlobalCardinalNumber
        {
            get
            {
                return ResultMembership.Sum(pair => pair.Value);
            }
        }

        public double GetDegreeOfTruth(LinguisticVariable quantifier, ref string aSummarization, bool aIsAllChosen)
        {
            List<double> degrees = new List<double>();

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

            r = quantifier.MembershipFunction.GetMembership(r);
            degrees.Add(r);

            // T_2
            double t2 = 1;
            if (AnotherSummarizator == null)
            {
                t2 = t2 - (supportCount / allRecordsCount);
            }
            else
            {
                t2 = t2 - Math.Pow((supportCount / allRecordsCount) * (AnotherSummarizator.Support().Count / AnotherSummarizator.AllRecords.Elements.Count), 2);
            }
            degrees.Add(t2);


            if (Qualificator != null)
            {
                //T_3
                List<Record> qualificatorSupport = Qualificator.Support();
                int intersectCount = new ClassicalSet<Record>(Support()).Intersect(qualificatorSupport).Count;
                double t3 = intersectCount / qualificatorSupportCount;
                degrees.Add(t3);
            }

            // T_4
            double t4 = supportCount / allRecordsCount;
            if (AnotherSummarizator != null)
            {
                t4 *= AnotherSummarizator.Support().Count / AnotherSummarizator.AllRecords.Elements.Count;
            }
            degrees.Add(t4);

            // T_5 
            double t5 = 0;
            t5 = 2 * Math.Pow(0.5, AnotherSummarizator != null ? 2 : 1);
            degrees.Add(t5);

            if (aIsAllChosen)
            {
                // T_6
                double distance = quantifier.Parameters.Last() -
                                         quantifier.Parameters.First();
                degrees.Add(1 - distance / AllRecords.Elements.Count);

                // T_7
                var quantifierSet = Enumerable.Range((int)quantifier.Parameters.First(),
                    (int)quantifier.Parameters.Last() - 1);
                double quantifierCardinalNumber = quantifierSet.Sum(i => quantifier.MembershipFunction.GetMembership(i));
                degrees.Add(Math.Min(1, quantifierCardinalNumber / distance));

                // T_8

                double t8 = supportCount / allRecordsCount;
                if (AnotherSummarizator != null)
                {
                    t8 *= AnotherSummarizator.Support().Count / AnotherSummarizator.AllRecords.Elements.Count;
                }
                degrees.Add(t8);

                if (Qualificator != null)
                {
                    // T_9
                    double t9 = 1 - (qualificatorSupportCount / allRecordsCount);
                    degrees.Add(t9);


                    // Meassure T_10
                    double t10 = 1 - qualificatorSupportCount / qualificatorAllRecordsCount;
                    degrees.Add(t10);

                    // Meassure T_11
                    degrees.Add(2 * Math.Pow(0.5, 1));
                    if (degrees.Any(val => double.IsNaN(val) || double.IsInfinity(val)))
                    {
                    }
                }
            }

            if (AnotherSummarizator != null)
            {
                string relation = AnotherSummarizator.RelationType == "AND" ? "i" : "lub";
                aSummarization = $"{quantifier.Name} wpisów ma parametr: {LinguisticVariable.Name} {relation} parametr: {AnotherSummarizator.LinguisticVariable.Name} [{degrees.Average():N3}]";
            }
            else if (Qualificator != null)
            {
                aSummarization = $"{quantifier.Name} wpisów posiadając parametr: {Qualificator.LinguisticVariable.Name} posiadało parametr: {LinguisticVariable.Name} [{degrees.Average():N3}]";
            }
            else
            {
                aSummarization = $"{quantifier.Name} wpisów posiada parametr: {LinguisticVariable.Name} [{degrees.Average():N3}]";
            }


            if (degrees.Any(val => double.IsNaN(val) || double.IsInfinity(val) || val > 1))
            {

            }
            ResultMembership.Clear();
            double avg = degrees.Average();
            if(avg.Equals(0.5))
            {

            }
            return avg;
        }

    }
}