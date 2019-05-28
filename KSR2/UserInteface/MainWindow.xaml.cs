using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Toolkit;

namespace UserInteface
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Record> DbRecords = new List<Record>();
        public MainWindow()
        {
            InitializeComponent();
            DbRecords = XlsxReader.ReadXlsx("..\\..\\..\\Resources\\weatherAUS.xlsx");
        }
    }
}
