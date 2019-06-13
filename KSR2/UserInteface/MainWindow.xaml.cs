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
        private List<CheckBox> DegreesCheckboxes = new List<CheckBox>();
        private List<CheckBox> QuantificatorsCheckboxes = new List<CheckBox>();
        private List<CheckBox> SummarizatorsCheckboxes = new List<CheckBox>();
        private List<CheckBox> QualificatorsCheckboxes = new List<CheckBox>();
        private List<SummarizationResult> Summarizations { get; set; } = new List<SummarizationResult>();
        private List<Record> data = new List<Record>();
        private List<LinguisticVariable> summarizators = new List<LinguisticVariable>();
        private List<LinguisticVariable> quantifiers = new List<LinguisticVariable>();
        private List<LinguisticVariable> qualificators = new List<LinguisticVariable>();
        public MainWindow()
        {
            InitializeComponent();
            data = XlsxReader.ReadXlsx("..\\..\\..\\Resources\\weatherAUS.xlsx");
            summarizators = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\Summarizators.xml");
            quantifiers = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\Quantifiers.xml");
            qualificators = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\Qualificators.xml");


            for (int i = 0; i < FuzzySet.DegreesLabels.Count; ++i)
            {
                CheckBox checkBox = new CheckBox() { IsChecked = true, Content = FuzzySet.DegreesLabels[i], Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), FontWeight = FontWeights.Heavy };
                DegreesCheckboxes.Add(checkBox);
                checkBox.Click += DegreesUpdated;
                Stack_Panel_Check_Boxes.Children.Add(DegreesCheckboxes[i]);

            }

            for (int i = 0; i < quantifiers.Count; ++i)
            {
                CheckBox checkBox = new CheckBox() { Content = quantifiers[i].Name, IsEnabled = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), IsChecked = true };
                QuantificatorsCheckboxes.Add(checkBox);
                checkBox.Click += QuantificatorsUpdated;
                Stack_Panel_Quantifiers.Children.Add(checkBox);
            }

            for (int i = 0; i < qualificators.Count; ++i)
            {
                CheckBox checkBox = new CheckBox() { Content = qualificators[i].Name, IsEnabled = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), };
                QualificatorsCheckboxes.Add(checkBox);
                checkBox.Click += QualificatorsUpdated;
                Stack_Panel_Qualificators.Children.Add(checkBox);
            }

            for (int i = 0; i < summarizators.Count; ++i)
            {
                CheckBox checkBox = new CheckBox() { Content = summarizators[i].Name, IsEnabled = true, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), };
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
                Process(chosenSummarizators[0], quantifiers);
                chosenSummarizators[0].AnotherSummarizator.RelationType = "OR";
                Process(chosenSummarizators[0], quantifiers);
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
                        streamWriter.WriteLine("Best summarization:\r\n" + summarizationResult.BestSummarization + "\r\n\r\nRest summarizations:\r\n");
                        foreach (string summarizationItem in summarizationResult.AllSummarizations)
                        {
                            streamWriter.WriteLine(summarizationItem);
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
            string bestResult = "";
            double quality = 0;
            foreach (LinguisticVariable quantifier in aQuantifiers)
            {
                string summarization = "";
                double degreeOfTruth = set.GetDegreeOfTruth(quantifier, ref summarization, true);
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


        private void Generate_Summarizations_Click(object sender, RoutedEventArgs e)
        {
            GenerateSummarizations();
        }

        private void CB_Qualificator_Click(object sender, RoutedEventArgs e)
        {
            Stack_Panel_Qualificators.IsEnabled = true;
            CB_Qualificator.IsChecked = true;
            CB_Complex.IsChecked = false;
            CB_Simple.IsChecked = false;
            EXP_Qualifiers.IsEnabled = true;
        }

        private void CB_Complex_Click(object sender, RoutedEventArgs e)
        {
            Stack_Panel_Qualificators.IsEnabled = false;
            CB_Qualificator.IsChecked = false;
            CB_Complex.IsChecked = true;
            CB_Simple.IsChecked = false;
            EXP_Qualifiers.IsExpanded = false;
            EXP_Qualifiers.IsEnabled = false;
        }

        private void CB_Simple_Click(object sender, RoutedEventArgs e)
        {
            Stack_Panel_Qualificators.IsEnabled = false;
            CB_Qualificator.IsChecked = false;
            CB_Complex.IsChecked = false;
            CB_Simple.IsChecked = true;
            EXP_Qualifiers.IsExpanded = false;
            EXP_Qualifiers.IsEnabled = false;
        }

        private void SummarizationsUpdated(object sender, RoutedEventArgs e)
        {
            int countOfTicked = SummarizatorsCheckboxes.Count(checkBox => checkBox.IsChecked.Value);
            if (CB_Complex.IsChecked.Value)
            {
                if (countOfTicked == 2)
                {
                    SummarizatorsCheckboxes.Where(checkBox => checkBox.IsChecked == false).ToList().ForEach(checkBox => checkBox.IsEnabled = false);
                    Generate_Summarizations.IsEnabled = true;
                }
                else
                {
                    SummarizatorsCheckboxes.ForEach(checkBox => checkBox.IsEnabled = true);
                    Generate_Summarizations.IsEnabled = false;
                }
            }
            else
            {
                if (countOfTicked == 1)
                {
                    SummarizatorsCheckboxes.Where(checkBox => checkBox.IsChecked == false).ToList().ForEach(checkBox => checkBox.IsEnabled = false);
                    if (CB_Qualificator.IsChecked.Value)
                    {
                        if (AreQualiicatorsFine(1))
                        {
                            Generate_Summarizations.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Generate_Summarizations.IsEnabled = true;
                    }

                }
                else
                {
                    SummarizatorsCheckboxes.ForEach(checkBox => checkBox.IsEnabled = true);
                    Generate_Summarizations.IsEnabled = false;
                }
            }
        }

        private void QualificatorsUpdated(object sender, RoutedEventArgs e)
        {
            int countOfTicked = QualificatorsCheckboxes.Count(checkBox => checkBox.IsChecked.Value);
            if (CB_Qualificator.IsChecked.Value)
            {
                if (countOfTicked == 1)
                {
                    QualificatorsCheckboxes.Where(checkBox => checkBox.IsChecked == false).ToList().ForEach(checkBox => checkBox.IsEnabled = false);
                    if (AreSummarizatorsFine(1))
                    {
                        Generate_Summarizations.IsEnabled = true;
                    }
                }
                else
                {
                    QualificatorsCheckboxes.ForEach(checkBox => checkBox.IsEnabled = true);
                    Generate_Summarizations.IsEnabled = false;
                }
            }
        }

        private void QuantificatorsUpdated(object sender, RoutedEventArgs e)
        {
            int countOfTicked = QuantificatorsCheckboxes.Count(checkBox => checkBox.IsChecked.Value);
            if (countOfTicked == 1)
            {
                QuantificatorsCheckboxes.First(checkBox => checkBox.IsChecked == true).IsEnabled = false;
            }
            else
            {
                QuantificatorsCheckboxes.Where(checkBox => checkBox.IsEnabled == false).ToList().ForEach(checkBox => checkBox.IsEnabled = true);
            }
        }

        private void DegreesUpdated(object sender, RoutedEventArgs e)
        {
            int countOfTicked = DegreesCheckboxes.Count(checkBox => checkBox.IsChecked.Value);
            if (countOfTicked == 1)
            {
                DegreesCheckboxes.First(checkBox => checkBox.IsChecked == true).IsEnabled = false;
            }
            else
            {
                DegreesCheckboxes.Where(checkBox => checkBox.IsEnabled == false).ToList().ForEach(checkBox => checkBox.IsEnabled = true);
            }
        }

        private bool AreSummarizatorsFine(int aMaxIncludedCount)
        {
            int countOfTicked = SummarizatorsCheckboxes.Count(checkBox => checkBox.IsChecked.Value);
            if (countOfTicked == aMaxIncludedCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AreQualiicatorsFine(int aMaxIncludedCount)
        {
            int countOfTicked = QualificatorsCheckboxes.Count(checkBox => checkBox.IsChecked.Value);
            if (countOfTicked == aMaxIncludedCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }


}
