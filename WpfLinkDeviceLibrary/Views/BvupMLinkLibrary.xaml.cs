using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using BvupMLinkLibrary.Models;
using BvupMLinkLibrary.ViewModels;

namespace BvupMLinkLibrary.Views {
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    //public partial class BvupMLinkWindow : System.Windows.Window {
    public partial class BvupMLinkWindow  {
        /// <summary>
        /// Объект для взаимодействия с формой
        /// </summary>
        public static BvupMLinkWindow ScreenMain;

        /// <summary>
        /// 
        /// </summary>
        private readonly WindowsViewModel _windowData;

        /// <summary>
        /// Задача
        /// </summary>
        private readonly TestLinkClass _taskLink = new TestLinkClass();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="name"></param>
        /// <param name="timePause"></param>
        /// <param name="enableHexLoad"></param>
        public BvupMLinkWindow( SerialPort port = null, string name = "", int timePause = 100,
            bool enableHexLoad = false ) {
            InitializeComponent();
            //
            //
            //
            var brush = new SolidColorBrush( Colors.Red );
            _windowData = new WindowsViewModel {
                Text         = "Нет связи",
                Label        = string.IsNullOrWhiteSpace( name ) ? "ВМ-7" : name,
                EllipseColor = brush
            };
            MainGrid.DataContext = _windowData;
            //
            //
            //
            if ( enableHexLoad ) {
                MessageBox.Show( "Выключите и включите питание устройства." );
            }
            ScreenMain = this;
            // Создаём новый рабочий поток
            ThreadPool.QueueUserWorkItem( obj => _taskLink.Start( port, timePause ) );
        }

        public void SetText( string text ) {
            _windowData.Text = text;
        }

        public void SetColor( Brush brush ) {
            _windowData.EllipseColor = brush;
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {
            Close();
        }

        private void Window_Closed( object sender, EventArgs e ) {
            _taskLink.Stop();
        }
    }
}