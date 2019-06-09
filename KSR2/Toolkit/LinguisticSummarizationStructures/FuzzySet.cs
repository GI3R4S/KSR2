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
        private ClassicalSet<Record> FilteredRecords = new ClassicalSet<Record>();
        public LinguisticVariable LinguisticVariable { get; set; } = new LinguisticVariable();

        public FuzzySet Qualificator;

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
            FilteredRecords.Elements = Support();
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

        public double GetDegreeOfTruth(LinguisticVariable quantifier, ref string aSummarization)
        {
            List<double> degrees = new List<double>();
            if (RelationType.Equals("AND") || RelationType.Equals("OR"))
            {
                for (int i = 0; i < AllRecords.Elements.Count; i++)
                {

                    if (RelationType == "AND")
                    {
                        double firstAffilation = GetAffilationForRecord(AllRecords.Elements[i]);
                        double secondAffilation = Qualificator.GetAffilationForRecord(AllRecords.Elements[i]);
                        if (firstAffilation > 1 || secondAffilation > 1)
                        {

                        }
                        ResultMembership.Add(AllRecords.Elements[i], firstAffilation > secondAffilation ? secondAffilation : firstAffilation);
                    }
                    else if (RelationType == "OR")
                    {
                        double firstAffilation = GetAffilationForRecord(AllRecords.Elements[i]);
                        double secondAffilation = Qualificator.GetAffilationForRecord(AllRecords.Elements[i]);

                        ResultMembership.Add(AllRecords.Elements[i], firstAffilation > secondAffilation ? firstAffilation : secondAffilation);
                    }
                }
            }
            else
            {
                RefreshMap();
                ResultMembership = LocalAllRecordsMembership;
            }



            double supportCount = Support().Count;
            double allRecordsCount = AllRecords.Elements.Count;
            double qualificatorSupportCount = Qualificator != null ? Qualificator.Support().Count : 0;
            double qualificatorAllRecordsCount = Qualificator != null ? Qualificator.AllRecords.Elements.Count : 0;

            // T_1
            double r = 0;
            if (Qualificator == null)
            {
                r = LocalCardinalNumber;
            }
            else
            {
                r = GlobalCardinalNumber / LocalCardinalNumber;
            }
            double t1 = quantifier.MembershipFunction.GetMembership(r) / Support().Count;
            degrees.Add(t1);

            // T_2

            double t2 = 1;
            if (Qualificator == null)
            {
                t2 = t2 - supportCount / allRecordsCount;
            }
            else
            {
                t2 = t2 - Math.Pow((supportCount / allRecordsCount * qualificatorSupportCount / qualificatorAllRecordsCount), 1 / 2);
            }
            degrees.Add(t2);


            if (Qualificator != null)
            {
                //T_3
                var xd = Qualificator.Support();
                int intersectCount = new ClassicalSet<Record>(Support()).Intersect(xd).Count;
                double t3 = intersectCount / qualificatorSupportCount;
                degrees.Add(t3);
            }

            // T_4

            double t4 = 0;
            double r1 = supportCount / allRecordsCount;
            if (Qualificator != null)
            {
                r1 *= qualificatorSupportCount / qualificatorAllRecordsCount;
            }

            t4 = r1;
            if (degrees.Count == 3)
            {
                t4 = Math.Abs(r1 - degrees[2]);
            }

            degrees.Add(t4);

            // T_5
            double t5 = 2 * Math.Pow(0.5, Qualificator == null ? 1 : 2);
            degrees.Add(t5);


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
            if (Qualificator != null)
            {
                t8 *= qualificatorSupportCount / qualificatorAllRecordsCount;
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
            }


            if (Qualificator == null)
            {
                aSummarization = $"{quantifier.Name} wpisów wykazywało następujący parametr: {LinguisticVariable.Name} [{degrees.Average():N3}]";
            }
            else
            {
                if (RelationType == "AND")
                {
                    aSummarization = $"{quantifier.Name} wpisów mających parametr: {Qualificator.LinguisticVariable.Name} wykazywało następujący parametr: {LinguisticVariable.Name} [{degrees.Average():N3}]";
                }
                if (RelationType == "OR")
                {
                    aSummarization = $"{quantifier.Name} wpisów miało parametr: {Qualificator.LinguisticVariable.Name} lub parametr: {LinguisticVariable.Name} [{degrees.Average():N3}]";
                }
            }
            if (degrees.Any(val => double.IsNaN(val) || double.IsInfinity(val)))
            {
            }
            ResultMembership.Clear();
            double avg = degrees.Average();

            return avg;
        }

    }
}