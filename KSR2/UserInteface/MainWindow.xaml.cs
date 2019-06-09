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
        }

        private void GenerateSummarizations()
        {
            #region Ładowanie Plików
            List<Record> data = XlsxReader.ReadXlsx("..\\..\\..\\Resources\\weatherAUS.xlsx");
            List<LinguisticVariable> summarizators = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\Summarizators.xml");
            List<LinguisticVariable> quantifiers = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\Quantifiers.xml");
            List<LinguisticVariable> qualificators = LinguisticVariableSerializer.Deserialize("..\\..\\..\\Resources\\Qualificators.xml");
            #endregion

            List<FuzzySet> FuzzySetsWithoutQualificators = new List<FuzzySet>();
            foreach(LinguisticVariable summarizator in summarizators)
            {
                FuzzySetsWithoutQualificators.Add(new FuzzySet(data, summarizator));
            }

            List<FuzzySet> FuzzySetsAndQualificators = new List<FuzzySet>();
            foreach (LinguisticVariable summarizator in summarizators)
            {
                FuzzySetsAndQualificators.Add(new FuzzySet(data, summarizator, "AND"));
            }

            List<FuzzySet> FuzzySetsOrQualificators = new List<FuzzySet>();
            foreach (LinguisticVariable summarizator in summarizators)
            {
                FuzzySetsOrQualificators.Add(new FuzzySet(data, summarizator, "OR"));
            }

            List<FuzzySet> Qualificators = new List<FuzzySet>();
            foreach(LinguisticVariable linguisticVariable in qualificators)
            {
                Qualificators.Add(new FuzzySet(data, linguisticVariable));
            }
            GenerateSummarizations(FuzzySetsWithoutQualificators, quantifiers);
            GenerateSummarizations(FuzzySetsAndQualificators, quantifiers, Qualificators);
            GenerateSummarizations(FuzzySetsOrQualificators, quantifiers, Qualificators);

            Results.ItemsSource = Summarizations;
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

        private void GenerateSummarizations(List<FuzzySet> aFuzzySets, List<LinguisticVariable> aQuantifiers, List<FuzzySet> aQualificators = null)
        {
            foreach (FuzzySet fuzzySet in aFuzzySets)
            {
                if(aQualificators == null)
                {
                        Process(fuzzySet, aQuantifiers);
                }
                else
                {
                    foreach (FuzzySet qualifcator in aQualificators)
                    {
                        fuzzySet.Qualificator = qualifcator;
                        Process(fuzzySet, aQuantifiers);
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
                double degreeOfTruth = set.GetDegreeOfTruth(quantifier, ref summarization);
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
    }


}
