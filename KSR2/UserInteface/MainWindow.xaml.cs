using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Toolkit;

namespace UserInteface
{
    public partial class MainWindow : Window
    {
        private List<SummarizationResult> Summarizations { get; set; } = new List<SummarizationResult>();

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

            #region Generate simple summaries
            List<FuzzySet> simpleSummaries = new List<FuzzySet>();
            for (int i = 0; i < summarizators.Count; ++i)
            {
                Process(new FuzzySet(data, summarizators[i]), quantifiers);
            }
            #endregion

            #region Generate summaries with complex summarators
            for (int i = 0; i < 2; i++)
            {
                FuzzySet second = new FuzzySet(data, summarizators[summarizators.Count - 1 - i], "OR");
                Process(new FuzzySet(data, summarizators[i]) { AnotherSummarizator = second }, quantifiers);
            }
            for (int i = 0; i < 2; i++)
            {
                FuzzySet second = new FuzzySet(data, summarizators[summarizators.Count - 1 - i], "AND");
                Process(new FuzzySet(data, summarizators[i]) { AnotherSummarizator = second }, quantifiers);
            }
            #endregion

            #region Generate summaries of second type
            for (int i = 0; i < qualificators.Count; i++)
            {
                FuzzySet qualificator = new FuzzySet(data, qualificators[i]);
                Process(new FuzzySet(data, summarizators[3 + i]) { Qualificator = qualificator }, quantifiers);
            }
            #endregion

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
                double degreeOfTruth = set.GetDegreeOfTruth(quantifier, ref summarization, CB_All.IsChecked.Value);
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

        private void CB_All_Click(object sender, RoutedEventArgs e)
        {
            CB_All.IsChecked = true;
            CB_Part.IsChecked = false;
        }

        private void CB_Part_Click(object sender, RoutedEventArgs e)
        {
            CB_All.IsChecked = false;
            CB_Part.IsChecked = true;
        }
    }


}
