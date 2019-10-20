using System;
using System.Windows;
using System.Windows.Input;
using WpfApplication.Models.Main;

namespace WpfApplication.Models.Form {
    /// <summary>
    /// Interaction logic for WindowException.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class WindowException : Window {
        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exc"></param>
        /// <param name="text"></param>
        public WindowException( Exception exc, string text = "" ) {
            InitializeComponent();

            Title += " " + exc.GetType();
            // Настройка изображений
            Icon          = new IconClass().get_icon();
            Image1.Source = new IconClass().get_image();
            // Заполнение данных
            LabelType.Text       = exc.GetType().ToString();
            LabelText.Text       = text;
            LabelSource.Text     = exc.Source;
            LabelTargetSite.Text = exc.TargetSite != null ? exc.TargetSite.ToString() : string.Empty;
            LabelStackTrace.Text = exc.StackTrace;
            LabelMessage.Text    = exc.Message;
            // Выход по кнопкам ECS и Enter
            PreviewKeyDown += HandleKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKey( object sender, KeyEventArgs e ) {
            if ( e.Key == Key.Escape || e.Key == Key.Enter ) {
                Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowsOfError_Loaded( object sender, RoutedEventArgs e ) {
            WindowsOfError.Height = LabelType.ActualHeight       + LabelText.ActualHeight       +
                                    LabelSource.ActualHeight     + LabelTargetSite.ActualHeight +
                                    LabelStackTrace.ActualHeight + LabelMessage.ActualHeight    + 100;
        }
    }
}