using System.Windows.Media;

namespace BvupMLinkLibrary.ViewModels {
    internal class WindowsViewModel : BaseViewModel {
        /// <summary>
        /// Текстовое поле
        /// </summary>
        private string _text;

        public string Text {
            get { return _text; }
            set {
                _text = value;
                OnPropertyChanged( "Text" );
            }
        }

        /// <summary>
        /// Метка
        /// </summary>
        private string _label;

        public string Label {
            get { return _label; }
            set {
                _label = value;
                OnPropertyChanged( "Label" );
            }
        }

        /// <summary>
        /// Фоновый цвет Ellipse
        /// </summary>
        private Brush _ellipseColor;

        public Brush EllipseColor {
            get { return _ellipseColor; }
            set {
                _ellipseColor = value;
                OnPropertyChanged( "EllipseColor" );
            }
        }
    }
}