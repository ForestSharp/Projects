using Microsoft.Win32;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using LibLAS;
using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using OpenTK.Mathematics;

namespace VMW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        LasFile LasFile;

        private static ObservableCollection<LibLAS.LasFile.InformationWell> InformationWells = new();

        private ObservableCollection<string> SelectedDiameter = new();


        public MainWindow()
        {
            InitializeComponent();

            ListViewInformationWells.ItemsSource = InformationWells;
            ListViewBoreholeDiameter.ItemsSource = SelectedDiameter;

            var settings = new GLWpfControlSettings
            {
                MajorVersion = 4,
                MinorVersion = 6,
                TransparentBackground = true
            };

            OpenTkControl.Start(settings);


        }

        private void LoadFileLAS_Click(object sender, RoutedEventArgs e)
        {
            LoadFile();
        }

        /// <summary>
        /// Загрузить и обработать LAS файл
        /// </summary>
        public void LoadFile()
        {
            string selectFile = OpenFileDialog("Файлы LAS(*.las)|*.las");

            LasFile = new(selectFile);

            try
            {
                LasFile.ReadFile();
                InitializeInformationWell();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        /// <summary>
        /// Открыть диалоговое окно для выбора файла
        /// </summary>
        /// <param name="filter">Фильтр для отображения необходимого расширения файла</param>
        /// <returns></returns>
        private static string OpenFileDialog(string filter = "")
        {
            OpenFileDialog LAS_file = new()
            {
                Filter = filter
            };

            LAS_file.ShowDialog();

            return LAS_file.FileName;
        }

        /// <summary>
        /// Вывести информацию взятую из блока с информации о скважине
        /// </summary>
        private void InitializeInformationWell()
        {
            InformationWells.Clear();

            foreach (var block in LasFile.BlockDataLAS)
                if(block.Name == "~well information")
                    for(int i = 0; i < 4; i++)
                        InformationWells.Add(block.Data[i]);                    
        }

        private void SelectDiameter_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectBoreholeDiameters windowSelectDiameter = new();
            windowSelectDiameter.ShowDialog();

            SelectedDiameter.Clear();
            SelectedDiameter.Add((string)windowSelectDiameter.ViewSelectDiameters.SelectedItem);
        }

        private void OpenTkControl_OnRender(TimeSpan delta)
        {


        }
    }
}
