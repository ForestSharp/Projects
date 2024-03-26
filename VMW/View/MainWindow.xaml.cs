using System;
using System.Windows;
using System.Collections.ObjectModel;
using OpenTK.Wpf;
using OpenTK.Mathematics;
using System.Diagnostics;
using VMW.Model;
using VMW.ViewModel;


/* формула нахожения координаты:
 * х = центр окружности + радиус окружности * cos угла поворота
 * y = центр окружности + радиус окружности * sin угла поворота
 */

namespace VMW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _MainViewModel = new();
        private GraphicCore GraphicCore = new();

        private ObservableCollection<string> SelectedDiameter = new();
        private bool _initializeGrapgicCore = false;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                var settings = new GLWpfControlSettings
                {
                    MajorVersion = 4,
                    MinorVersion = 6,
                    GraphicsContextFlags = OpenTK.Windowing.Common.ContextFlags.ForwardCompatible,
                    GraphicsProfile = OpenTK.Windowing.Common.ContextProfile.Core,
                };

                DataContext = _MainViewModel;

                OpenTkControl.Start(settings);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            if (!_initializeGrapgicCore)
            {
                GraphicCore.Initialization(OpenTkControl.RenderSize);
                _initializeGrapgicCore = true;
            }

            Title = $"Объёмное моделирование скважины fps: {10000000 / delta.Ticks}";
            GraphicCore.ToRender();
        }

        private void OpenTkControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// При движение мыши в пределах окна рендеринга и определение необходимого действия при нажатия на ЛКМ и ПКМ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTkControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            Vector2 mouse = new((float)mousePosition.X, (float)mousePosition.Y);
            GraphicCore.MoveMouse(mouse, e);
        }

        /// <summary>
        /// Изменение приближения камеры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTkControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            GraphicCore.CameraZoom(e.Delta);
        }        

        /// <summary>
        /// При закрытие программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Выход вызван");

            GraphicCore.CloseProgram();// удаление программы с шейдарами
        }

        /// <summary>
        /// При изменение размера окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GraphicCore.UpdateProjection(e);
        }

        private void Modeling_Click(object sender, RoutedEventArgs e)
        {
            //if (DataLas is null)
            //{
            //    MessageBox.Show("Необходимо выбрать Las файл", "Внимание");
            //    return;
            //}

            //WellModeling();
        }

        private void WellModeling()
        {
            //Borehole borehole = new(DataLas);

            //GraphicCore.ClearRenderObject();
            //GraphicCore.LoadRenderObject(borehole.Vertices, borehole.Indices);
        }

        private void ButtonCenteringMeasurements_Checked(object sender, RoutedEventArgs e)
        {
            //Borehole borehole = new(DataLas, true);

            //GraphicCore.ClearRenderObject();
            //GraphicCore.LoadRenderObject(borehole.Vertices, borehole.Indices);
        }

        private void ButtonCenteringMeasurements_Unchecked(object sender, RoutedEventArgs e)
        {
            //Borehole borehole = new(DataLas, false);

            //GraphicCore.ClearRenderObject();
            //GraphicCore.LoadRenderObject(borehole.Vertices, borehole.Indices);
        }
    }
}
