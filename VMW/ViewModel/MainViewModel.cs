using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using VMW.Command;
using VMW.Model;

namespace VMW.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        DataLas DataLas { get; set; }
        

        private int _drawingLenght = 100;       
        private double _depth;

        private double _maxDepth;
        private double _minValue;
        private double _maxValue;

        private double innerDiameter;
        private double outerDiameter;

        public int DrawingLenght
        {
            get => _drawingLenght;
            set
            {
                if (_drawingLenght != value && value > 0)
                    _drawingLenght = value;

                if (value > 1000)
                    _drawingLenght = 1000;

                if (MinValue != MaxValue)
                    MaxValue = _maxDepth - _drawingLenght;

                if (Depth > MaxValue)
                    Depth = MaxValue;

                OnPropertyChanged("DrawingLenght");
            }
        }

        public double Depth 
        { 
            get => _depth ; 
            set 
            { 
                if (_depth != value)                    
                    _depth = value;

                if (_maxValue < value)
                    _depth = _maxValue;

                if (_minValue > value)
                    _depth = _minValue;

                OnPropertyChanged("Depth");
            } 
        }
        public double MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                OnPropertyChanged("MinValue");
            }
        }
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                OnPropertyChanged("MaxValue");
            }
        }

        public double InnerDiameter { 
            get => innerDiameter;
            set 
            { 
                innerDiameter = value;
                OnPropertyChanged("InnerDiameter");
            } 
        }
        public double OuterDiameter { 
            get => outerDiameter;
            set
            {
                outerDiameter = value;
                OnPropertyChanged("OuterDiameter");
            }
        }

        public ICommand ReadLas {  get; set; }
        public ICommand UpButtonDrawing { get; set; }
        public ICommand DownButtonDrawing { get; set; }
        public ICommand SelectDiameters { get; set; }

        internal MainViewModel()
        {
            ReadLas = new MainCommand(_ =>
            {
                OpenFileDialog file = new()
                {
                    Filter = "Файлы LAS(*.las)|*.las"
                };

                file.ShowDialog();
                try
                {
                    DataLas = LasFile.GetDataLas(file.FileName);
                    FillingMinMaxValueSlider();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });        
            UpButtonDrawing = new MainCommand(_ =>  DrawingLenght += 10);
            DownButtonDrawing = new MainCommand(_ => DrawingLenght -= 10);
            SelectDiameters = new MainCommand(_ =>
            {

                WindowSelectBoreholeDiameters WindowSelectDiameters = new();
                    foreach(var window in App.Current.Windows)
                        if (window is MainWindow)
                            WindowSelectDiameters.Owner = (Window)window;

                WindowSelectDiameters.ShowDialog();

                if (WindowSelectDiameters.Diameters is null)
                    return;

                InnerDiameter = WindowSelectDiameters.Diameters.InnerDiameter;
                OuterDiameter = WindowSelectDiameters.Diameters.OuterDiameter;
            });

        }

        void FillingMinMaxValueSlider()
        {
            double.TryParse(DataLas.Well.Find(x => x.Name == "STRT.M").GetValue().Replace('.', ','), out double first);
            double.TryParse(DataLas.Well.Find(x => x.Name == "STOP.M").GetValue().Replace('.', ','), out double second);

            MinValue = double.Min(first, second);
            Depth = MinValue;

            _maxDepth = double.Max(first, second);           
            MaxValue = _maxDepth - DrawingLenght;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
