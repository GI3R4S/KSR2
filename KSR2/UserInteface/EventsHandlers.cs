using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserInteface
{
    public partial class MainWindow : Window
    {
        private void Generate_Summarizations_Click(object sender, RoutedEventArgs e)
        {
            GenerateSummarizations();
            Button_Save.IsEnabled = true;
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
            int countOfTicked = QuantifiersCheckboxes.Count(checkBox => checkBox.IsChecked.Value);
            if (countOfTicked == 1)
            {
                QuantifiersCheckboxes.First(checkBox => checkBox.IsChecked == true).IsEnabled = false;
            }
            else
            {
                QuantifiersCheckboxes.Where(checkBox => checkBox.IsEnabled == false).ToList().ForEach(checkBox => checkBox.IsEnabled = true);
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
