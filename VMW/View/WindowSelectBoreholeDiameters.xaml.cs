using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VMW.Model;
using VMW.Resources;
using VMW.ViewModel;

namespace VMW
{
    /// <summary>
    /// Логика взаимодействия для WindowSelectBoreholeDiameters.xaml
    /// </summary>
    partial class WindowSelectBoreholeDiameters : Window
    {
        List<DiametersWell> DiametersWell {
            get => diametersWell;
            set => diametersWell = value;
        }
        ObservableCollection<string>BoreholeDiameters = new ObservableCollection<string>();
        private List<DiametersWell> diametersWell = new();

        internal WindowSelectBoreholeDiameters()
        {
            InitializeComponent();
            ViewSelectDiameters.ItemsSource = DiametersWell;
            ReadingDiameters();
            
        }

        private void ReadingDiameters()
        {
             

            var file = ValuesOfWellDiameters.Diameters;
            var delete = "~Diameters\r\n".ToCharArray();
            var resultDiameters = file.Trim(delete).Replace("\r\n", ";").Replace('.', ',').Split(';');
            double[][] diameters = new double[resultDiameters.Length/2][];

            for (int i = 0, k = 1; i < resultDiameters.Length; i += 2, k += 2)
                if(double.TryParse(resultDiameters[i], out double resultOuterDiameter) && double.TryParse(resultDiameters[k], out double resultInnerDiameter))
                    diametersWell.Add(new(double.Min(resultInnerDiameter, resultOuterDiameter), double.Max(resultInnerDiameter, resultOuterDiameter)));
            
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        internal DiametersWell Diameters { get { return (DiametersWell)ViewSelectDiameters.SelectedItem; }  }
    }
}
