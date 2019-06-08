using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toolkit.LinguisticSummarizationStructures;

namespace Toolkit
{
    public class FuzzySet
    {
        private LinguisticVariablesGroup linguisticVariablesGroup;
        private Dictionary<Record, double> membershipMap = new Dictionary<Record, double>();
        private ClassicalSet<Record> allElements = new ClassicalSet<Record>();
        private ClassicalSet<Record> elements = new ClassicalSet<Record>();
        private FuzzySet qualificator;


        // konstruktor FuzzySet - przyjmuje listę rekordów oraz jedną zmienną lingwistyczną
        public FuzzySet(List<Record> aRecords, LinguisticVariable aLinguisticVariable)
        {
            elements.Elements = aRecords.ToList();
            linguisticVariablesGroup =
                new LinguisticVariablesGroup
                {
                    LinguisticVariable = aLinguisticVariable
                };
        }

        // konstruktor FuzzySet - przyjmuje listę rekordów oraz grupę zmiennych lingwistycznych
        private FuzzySet(List<Record> aRecords, LinguisticVariablesGroup aLinguisticVariablesGroup)
        {
            aRecords = aRecords.ToList();
            linguisticVariablesGroup = aLinguisticVariablesGroup;
        }

        // właściwość kwalifikatora
        public FuzzySet Qualificator
        {
            get
            {
                return qualificator;
            }
            set
            {
                qualificator = value;
                allElements.Elements = elements.Elements.ToList();

                List<Record> support = qualificator.Support();
                elements.Elements = support;
            }
        }

        // zwraca stopień przynależności do zbioru rozmytego dla zadanego rekordu - operator [] dla FuzzySet
        public double GetAffilationForRecord(Record item)
        {
            // jeśli wartość stopnia przynależnci była już obliczona
            if (membershipMap.ContainsKey(item))
            {
                return membershipMap[item];
            }

            // jeśli wartość stopnia przynależności nie była jeszcze obliczana
            double membership = GetMembership(linguisticVariablesGroup);
            membershipMap[item] = membership;
            return membership;

            //definicja rekurencji obliczającej stopień przynależności.
            double GetMembership(LinguisticVariablesGroup group)
            {
                // jeśli dziecko istnieje to oblicz przynależnśc rekurencyjnie dla pozostałych potomków
                if (group.Child != null)
                {
                    double childMembership = GetMembership(group.Child);
                    return group.RelationToChild(new List<double> { childMembership, GetOwnMembership() });
                }

                // po osiągnięciu maksymalnej głębokości rekurencji:
                return GetOwnMembership();

                double GetOwnMembership()
                {
                    return group.LinguisticVariable.MembershipFunction.GetMembership(group.LinguisticVariable.IsQuantifier()
                        ? group.LinguisticVariable.MembershipFunction.GetMembership(elements.Elements.Count)
                        : group.LinguisticVariable.Extractor(item));
                }
            }
        }

        public List<Record> Support()
        {
            List<Record> filteredElements = elements.Elements.Where(record => GetAffilationForRecord(record) > 0).ToList();
            return filteredElements;
        }

        #region Unused
        public List<Record> Core()
        {
            List<Record> filteredElements = elements.Elements.Where(record => GetAffilationForRecord(record) > 1).ToList();
            return filteredElements;
        }

        public double Height()
        {
            double height = elements.Elements.Max(record => GetAffilationForRecord(record));
            return height;
        }

        public List<Record> AlphaCut(double aMinivalValue)
        {
            Debug.Assert(aMinivalValue >= 0 && aMinivalValue <= 100);
            List<Record> filteredElements = elements.Elements.Where(record => GetAffilationForRecord(record) > aMinivalValue).ToList();
            return filteredElements;
        }

        public bool IsEmpty()
        {
            return elements.Elements.All(record => GetAffilationForRecord(record) == 0);
        }
        public double GetAffilationComplementForRecord(Record item)
        {
            return 1 - GetAffilationForRecord(item);
        }
        #endregion

        public double CardinalNumber()
        {
            double sum = membershipMap.Sum(pair => pair.Value);
            return sum;
        }

        public bool HasOr
        {
            get
            {
                LinguisticVariablesGroup current = linguisticVariablesGroup;
                while (current != null)
                {
                    if (current.RelationName == "lub")
                    {
                        return true;
                    }
                    current = current.Child;
                }

                return false;
            }
        }

        // operator & reprezenujący operację AND
        public static FuzzySet operator &(FuzzySet first, FuzzySet other)
        {
            var group = new LinguisticVariablesGroup
            {
                LinguisticVariable = other.linguisticVariablesGroup.LinguisticVariable,
                Child = first.linguisticVariablesGroup,
                RelationToChild = list => list.Min(),
                RelationName = "i"
            };
            return new FuzzySet(first.elements.Elements, group);
        }

        // operator | reprezenujący operację OR
        public static FuzzySet operator |(FuzzySet first, FuzzySet other)
        {
            var group = new LinguisticVariablesGroup
            {
                LinguisticVariable = other.linguisticVariablesGroup.LinguisticVariable,
                Child = first.linguisticVariablesGroup,
                RelationToChild = list => list.Max(),
                RelationName = "lub"
            };
            return new FuzzySet(first.elements.Elements, group);
        }

        // reprezentacja słowna FuzzySetu, a tak właściwie to podsumowanie lingwistyczne
        public override string ToString()
        {
            string output = "";

            // dodaj środkową część zdania
            if (Qualificator != null)
            {
                output += $"będących {Qualificator.linguisticVariablesGroup.LinguisticVariable.Name} wystąpiły ";
            }
            // dodaj tylko spację
            else
            {
                output = " ";
            }

            LinguisticVariablesGroup currentGroup = linguisticVariablesGroup;
            //iteruj po grupie zmiennych lingwistycznych
            while (currentGroup != null)
            {
                output += $"{currentGroup.LinguisticVariable.Name} ";
                //zejdź poziom niżej do dziecka
                if (currentGroup.Child != null)
                {
                    output += $"{currentGroup.RelationName} ";
                    currentGroup = currentGroup.Child;
                }
                else
                {
                    break;
                }
            }

            return output;
        }

        public double DegreeOfTruth(LinguisticVariable quantifier)
        {
            List<double> degrees = new List<double>
            {

                // Meassure T_1
                CardinalNumber() / elements.Elements.Count
            };

            // Meassure T_2
            double t2 = 1d;
            var allVariables = linguisticVariablesGroup.Flatten();
            foreach (LinguisticVariable variable in allVariables)
            {
                FuzzySet tempSet = new FuzzySet(elements.Elements, variable);
                double factor = tempSet.Support().Count / (double)elements.Elements.Count;
                t2 *= factor;
            }

            degrees.Add(1 - Math.Pow(t2, t2 / allVariables.Count));

            // Meassure T_3
            double t3 = Support().Count() / (double)elements.Elements.Count;
            degrees.Add(t3);

            // Meassure T_4
            double t4 = 1d;
            foreach (var variable in allVariables)
            {
                var tempSet = new FuzzySet(elements.Elements, variable);
                t4 *= tempSet.Support().Count() / (double)elements.Elements.Count - t3;
            }

            degrees.Add(Math.Abs(t4));

            // Meassure T_5
            degrees.Add(2 * Math.Pow(0.5, allVariables.Count));

            // Meassure T_6
            var quantifierElements = quantifier.Parameters.Last() -
                                     quantifier.Parameters.First();
            degrees.Add(1 - quantifierElements / elements.Elements.Count);

            // Meassure T_7
            var quantifierSet = Enumerable.Range((int)quantifier.Parameters.First(),
                (int)quantifier.Parameters.Last() - 1);
            var quantifierCardinalNumber = quantifierSet.Sum(i => quantifier.MembershipFunction.GetMembership(i));
            degrees.Add(Math.Min(1, quantifierCardinalNumber / quantifierElements));

            // Meassure T_8
            var sc = Support().Count();
            var f = 1 + elements.Elements.Count / (double)(allElements.Elements ?? elements.Elements).Count;
            degrees.Add(quantifier.MembershipFunction.GetMembership(sc * f));

            if (Qualificator != null)
            {
                // Meassure T_9
                degrees.Add(1 - Qualificator.Support().Count / (double)allElements.Elements.Count);

                // Meassure T_10
                degrees.Add(1 - Qualificator.CardinalNumber() / allElements.Elements.Count);

                // Meassure T_11
                degrees.Add(2 * Math.Pow(0.5, Qualificator.linguisticVariablesGroup.Flatten().Count));
            }

            return degrees.Average();
        }
    }
}