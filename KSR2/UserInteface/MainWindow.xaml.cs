using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Toolkit;

namespace UserInteface
{
    public partial class MainWindow : Window
    {
        List<SummarizationResult> Summarizations { get; set; } = new List<SummarizationResult>();

        public MainWindow()
        {
            InitializeComponent();
            OnLoaded();
        }

        private void OnLoaded()
        {
            #region Ładowanie Plików
            List<Record> data = XlsxReader.ReadXlsx("..\\..\\..\\Resources\\weatherAUS.xlsx");
            List<LinguisticVariable> variables = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\linguisticVariables.xml");
            List<LinguisticVariable> quantifiers = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\linguisticQuantifiers.xml");
            List<LinguisticVariable> qualificators = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\linguisticQualificators.xml");
            #endregion

            var groups = variables.GroupBy(variable => variable.MemberToExtract).ToList();
            List<FuzzySet> sets = new List<FuzzySet>();
            foreach (var group in groups)
            {
                var movedGroupings = new List<Tuple<string, List<LinguisticVariable>>>
                {
                    new Tuple<string, List<LinguisticVariable>>(group.Key, group.ToList())
                };
                movedGroupings.AddRange(groups.Where(linguisticVariables => linguisticVariables != group).Take(2)
                    .Select(g => new Tuple<string, List<LinguisticVariable>>(g.Key, g.ToList())));
                sets = sets.Concat(GetSets(data, movedGroupings)).ToList();
            }

            var i = 0;
            List<double> qualities = new List<double>();
            foreach (var fuzzySet in sets)
            {
                Process(fuzzySet);
                fuzzySet.Qualificator = new FuzzySet(data, qualificators[i++]);
                if (i == qualificators.Count)
                {
                    i = 0;
                }
                Process(fuzzySet);

                void Process(FuzzySet set)
                {
                    SummarizationResult summarizationResult = new SummarizationResult();
                    string bestResult = "";
                    double quality = 0;
                    foreach (var quantifier in quantifiers)
                    {
                        double degreeOfTruth = set.DegreeOfTruth(quantifier);
                        var summarization = $"{quantifier.Name} wpisów wskazuje {set} [Jakość: {degreeOfTruth:N3}]";
                        if (degreeOfTruth > quality)
                        {
                            bestResult = summarization;
                            quality = degreeOfTruth;
                        }

                        summarizationResult.AllSummarizations.Add(summarization);
                    }
                    qualities.Add(quality);
                    summarizationResult.BestSummarization = bestResult;
                    Summarizations.Add(summarizationResult);
                }
            }
            Results.ItemsSource = Summarizations;
        }

        private List<FuzzySet> GetSets(IEnumerable<Record> data,
            IEnumerable<Tuple<string, List<LinguisticVariable>>> vars)
        {
            var sets = new List<FuzzySet>();
            var i = 1;
            vars = vars.Take(1);
            foreach (var group in vars)
            {
                if (!sets.Any())
                {
                    foreach (var linguisticVariable in group.Item2)
                    {
                        sets.Add(new FuzzySet(data, linguisticVariable));
                    }
                }
                else
                {
                    List<FuzzySet> combo = new List<FuzzySet>();
                    foreach (var fuzzySet in sets)
                    {
                        var j = 0;
                        foreach (var linguisticVariable in group.Item2)
                        {
                            if (j < sets.Count - 1 || fuzzySet.HasOr)
                            {
                                combo.Add(fuzzySet & new FuzzySet(data, linguisticVariable));
                            }
                            else
                            {
                                combo.Add(fuzzySet | new FuzzySet(data, linguisticVariable));
                            }

                            j++;
                        }
                    }
                    sets = combo;
                }
                i++;
            }
            return sets;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = "txt";

            var res = saveFileDialog.ShowDialog();
            if(res.HasValue && res.Value)
            {
                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach(SummarizationResult summarizationResult in Summarizations)
                    {
                        streamWriter.WriteLine("Best summarization:\r\n\t-" + summarizationResult.BestSummarization + "\r\n\r\nAll summarizations:\r\n");
                        int i = 1;
                        foreach (string summarizationItem in summarizationResult.AllSummarizations)
                        {
                            streamWriter.WriteLine(i + ". " + summarizationItem);
                            ++i;
                        }
                        streamWriter.WriteLine("\r\n\r\n\r\n##################################################################\r\n\r\n\r\n");
                    }
                }
            }
        }
    }

}
