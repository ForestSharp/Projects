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

        public ICommand ReadLas {  get; set; }
        public ICommand UpButtonDrawing { get; set; }
        public ICommand DownButtonDrawing { get; set; }

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

        }

        void FillingMinMaxValueSlider()
        {
            double.TryParse(DataLas.Well.Find(x => x.Name == "STRT.M").GetValue().Replace('.', ','), out double first);
            double.TryParse(DataLas.Well.Find(x => x.Name == "STOP.M").GetValue().Replace('.', ','), out double second);

            MinValue = double.Min(first, second);
            _maxDepth = double.Max(first, second);

            Depth = MinValue;
            MaxValue = _maxDepth - DrawingLenght;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
