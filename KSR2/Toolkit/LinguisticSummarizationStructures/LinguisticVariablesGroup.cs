using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit
{
    public class LinguisticVariablesGroup
    {
        private readonly List<LinguisticVariable> GroupOfLinguisticVariables = new List<LinguisticVariable>();
        public LinguisticVariable LinguisticVariable { get; set; }
        public LinguisticVariablesGroup Child { get; set; }
        public Func<List<double>, double> RelationToChild { get; set; }
        public string RelationName { get; set; }

        // konwertuje postać grafu do postaci wektora
        public List<LinguisticVariable> Flatten()
        {
            // jeśli lista zawieranych zmiennych jest niepusta
            if (GroupOfLinguisticVariables.Any())
                return GroupOfLinguisticVariables;

            // jeśli jest pusta, to rozwijaj węzły poprzez odwoływanie się do Child każdej grupy.
            GroupOfLinguisticVariables.Add(LinguisticVariable);
            LinguisticVariablesGroup child = Child;
            while (child != null)
            {
                GroupOfLinguisticVariables.Add(child.LinguisticVariable);
                child = child.Child;
            }

            return GroupOfLinguisticVariables;
        }
    }
}