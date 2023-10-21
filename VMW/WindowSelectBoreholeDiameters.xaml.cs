using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Linq.Expressions;
using System.Windows;
using VMW.Resources;

namespace VMW
{
    /// <summary>
    /// Логика взаимодействия для WindowSelectBoreholeDiameters.xaml
    /// </summary>
    public partial class WindowSelectBoreholeDiameters : Window
    {
        string[] Diameter { get; set; }

        ObservableCollection<string>BoreholeDiameters = new ObservableCollection<string>();

        public WindowSelectBoreholeDiameters()
        {
            InitializeComponent();
            ReadingDiameters();
            ViewSelectDiameters.ItemsSource = Diameter;
        }

        private void ReadingDiameters()
        { 
            var file = ValuesOfWellDiameters.Diameters;

            int index = file.LastIndexOf("~Diameters");

            if (index == -1)
                return;

            Diameter = file.Substring(10).Trim('\r','\n').Split("\r\n");
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
