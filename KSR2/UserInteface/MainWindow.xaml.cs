using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Toolkit;

namespace UserInteface
{
    public partial class MainWindow : Window
    {
        private List<SummarizationResult> Summarizations { get; set; } = new List<SummarizationResult>();

        private List<CheckBox> DegreesCheckboxes = new List<CheckBox>();
        private List<CheckBox> QuantifiersCheckboxes = new List<CheckBox>();
        private List<CheckBox> SummarizatorsCheckboxes = new List<CheckBox>();
        private List<CheckBox> QualificatorsCheckboxes = new List<CheckBox>();
        private List<Record> data = new List<Record>();
        private List<LinguisticVariable> summarizators = new List<LinguisticVariable>();
        private List<LinguisticVariable> quantifiers = new List<LinguisticVariable>();
        private List<LinguisticVariable> qualificators = new List<LinguisticVariable>();
        public MainWindow()
        {
            InitializeComponent();
            data = XlsxReader.ReadXlsx("..\\..\\..\\Resources\\weatherAUS.xlsx");
            summarizators = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\Attributes.xml");
            quantifiers = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\Quantifiers.xml");
            qualificators = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\Attributes.xml");


            for (int i = 0; i < FuzzySet.DegreesLabels.Count; ++i)
            {
                CheckBox checkBox = new CheckBox() { FontSize = 13, IsChecked = true, Content = FuzzySet.DegreesLabels[i], Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), FontWeight = FontWeights.Heavy };
                DegreesCheckboxes.Add(checkBox);
                checkBox.Click += DegreesUpdated;
                Stack_Panel_Check_Boxes.Children.Add(DegreesCheckboxes[i]);
            }

            for (int i = 0; i < quantifiers.Count; ++i)
            {
                CheckBox checkBox = new CheckBox() { FontSize = 13, Content = quantifiers[i].Name, IsEnabled = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), IsChecked = true };
                QuantifiersCheckboxes.Add(checkBox);
                checkBox.Click += QuantificatorsUpdated;
                Stack_Panel_Quantifiers.Children.Add(checkBox);
            }

            for (int i = 0; i < qualificators.Count; ++i)
            {
                CheckBox checkBox = new CheckBox() { FontSize = 13, Content = qualificators[i].Name, IsEnabled = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), };
                QualificatorsCheckboxes.Add(checkBox);
                checkBox.Click += QualificatorsUpdated;
                Stack_Panel_Qualificators.Children.Add(checkBox);
            }

            for (int i = 0; i < summarizators.Count; ++i)
            {
                CheckBox checkBox = new CheckBox() { FontSize = 13, Content = summarizators[i].Name, IsEnabled = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), };
                SummarizatorsCheckboxes.Add(checkBox);
                Stack_Panel_Sumarizators.Children.Add(checkBox);
                checkBox.Click += SummarizationsUpdated;
            }
            Generate_Summarizations.IsEnabled = false;
            Button_Save.IsEnabled = false;
            EXP_Qualifiers.IsEnabled = false;
        }

        private void GenerateSummarizations()
        {
            Summarizations.Clear();
            Results.ItemsSource = null;
            // Collect chosen quantificators
            List<LinguisticVariable> chosenQuantyficators = new List<LinguisticVariable>();
            for (int i = 0; i < Stack_Panel_Quantifiers.Children.Count; ++i)
            {
                if (((CheckBox)Stack_Panel_Quantifiers.Children[i]).IsChecked.Value)
                {
                    chosenQuantyficators.Add(quantifiers[i]);
                }
            }

            // Collect chosen summarizators
            List<FuzzySet> chosenSummarizators = new List<FuzzySet>();
            for (int i = 0; i < Stack_Panel_Sumarizators.Children.Count; ++i)
            {
                if (((CheckBox)Stack_Panel_Sumarizators.Children[i]).IsChecked.Value)
                {
                    chosenSummarizators.Add(new FuzzySet(data, summarizators[i]));
                }
            }

            // Collect chosen qualificator
            FuzzySet chosenQualificator = null;
            for (int i = 0; i < Stack_Panel_Qualificators.Children.Count; ++i)
            {
                if (((CheckBox)Stack_Panel_Qualificators.Children[i]).IsChecked.Value)
                {
                    chosenQualificator = new FuzzySet(data, qualificators[i]);
                    break;
                }
            }


            if (CB_Simple.IsChecked.Value)
            {
                Process(chosenSummarizators[0], chosenQuantyficators);
            }
            else if (CB_Complex.IsChecked.Value)
            {
                chosenSummarizators[0].AnotherSummarizator = chosenSummarizators[1];
                chosenSummarizators[0].AnotherSummarizator.RelationType = "AND";
                Process(chosenSummarizators[0], chosenQuantyficators);
                chosenSummarizators[0].AnotherSummarizator.RelationType = "OR";
                Process(chosenSummarizators[0], chosenQuantyficators);
            }
            else if (CB_Qualificator.IsChecked.Value)
            {
                chosenSummarizators[0].Qualificator = chosenQualificator;
                Process(chosenSummarizators[0], chosenQuantyficators);
            }


            Results.ItemsSource = Summarizations;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "txt"
            };

            var res = saveFileDialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (SummarizationResult summarizationResult in Summarizations)
                    {
                        int i = 0;
                        foreach(string summary in summarizationResult.Summaries)
                        {
                            string[] parts = summary.Split('\n');
                            string part1 = parts[0];
                            string part2 = parts[1];
                            string part3 = parts[2];

                            streamWriter.WriteLine(part1);
                            streamWriter.WriteLine(part2);
                            streamWriter.WriteLine(part3);

                            streamWriter.WriteLine();
                            if(i == 0)
                            {
                                streamWriter.Write("\r\n");
                            }
                            i++;
                        }
                        streamWriter.WriteLine("\r\n\r\n\r\n##################################################################\r\n\r\n\r\n");
                    }
                }
            }
        }

        private void Process(FuzzySet set, List<LinguisticVariable> aQuantifiers)
        {
            List<double> qualities = new List<double>();
            SummarizationResult summarizationResult = new SummarizationResult();
            foreach (LinguisticVariable quantifier in aQuantifiers)
            {
                string summarization = GetDescription(set, quantifier).Key;
                double degreeOfTruth = GetDescription(set, quantifier).Value;
                summarizationResult.Pairs.Add(new KeyValuePair<string, double>(summarization, degreeOfTruth));
            }
            Summarizations.Add(summarizationResult);
        }
        private KeyValuePair<string, double> GetDescription(FuzzySet aFuzzySet, LinguisticVariable aQuantifier)
        {
            Dictionary<string, double> degrees = aFuzzySet.GetDegrees(aQuantifier);
            var filteredDegrees = degrees.Where(pair => DegreesCheckboxes.Select(cb => cb.Content).Contains(pair.Key)).ToList();
            double average = filteredDegrees.Select(pair => pair.Value).Average();

            string summarization = "";
            if (aFuzzySet.AnotherSummarizator != null)
            {
                string relation = aFuzzySet.AnotherSummarizator.RelationType == "AND" ? "i" : "lub";
                summarization = $"{aQuantifier.Name} wpisów ma parametr: {aFuzzySet.LinguisticVariable.Name} {relation} parametr: {aFuzzySet.AnotherSummarizator.LinguisticVariable.Name} [{average:N3}]";
            }
            else if (aFuzzySet.Qualificator != null)
            {
                summarization = $"{aQuantifier.Name} wpisów posiadając parametr: {aFuzzySet.Qualificator.LinguisticVariable.Name} posiadało parametr: {aFuzzySet.LinguisticVariable.Name} [{average:N3}]";
            }
            else
            {
                summarization = $"{aQuantifier.Name} wpisów posiada parametr: {aFuzzySet.LinguisticVariable.Name} [{average:N3}]";
            }

            summarization += "\r\n";

            foreach (CheckBox checkBox in DegreesCheckboxes.Where(cb => cb.IsChecked.Value))
            {
                try
                {
                    var searchedPair = filteredDegrees.First(pair => pair.Key.Equals(checkBox.Content));
                    summarization += $"{checkBox.Content}\t";

                }
                catch
                {
                    summarization += $"{checkBox.Content}\t";
                }
            }
            summarization += "\n";
            foreach (CheckBox checkBox in DegreesCheckboxes.Where(cb => cb.IsChecked.Value))
            {
                try
                {
                    var searchedPair = filteredDegrees.First(pair => pair.Key.Equals(checkBox.Content));
                    summarization += $"{searchedPair.Value:N3}\t ";

                } catch
                {
                    summarization += "     \t";
                }
            }

            return new KeyValuePair<string, double>(summarization, average);
        }
    }


}
