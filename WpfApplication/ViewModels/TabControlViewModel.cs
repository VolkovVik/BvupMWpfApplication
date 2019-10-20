using System.Windows.Media;

namespace WpfApplication.ViewModels {
    /// <summary>
    /// Класс ViewModel элемента TabControl
    /// </summary>
    public class TabControlViewModel : BaseViewModel {
        /// <summary>
        /// Переменная на свойство Fill элемента TabControl 1-ого уровня
        /// </summary>
        private Brush _signalFieldBrush1;

        /// <summary>
        /// Свойство на свойство Fill элемента TabControl 1-ого уровня
        /// </summary>
        public Brush SignalFieldBrush1 {
            get { return _signalFieldBrush1; }
            set {
                _signalFieldBrush1 = value;
                OnPropertyChanged( nameof( SignalFieldBrush1 ) );
            }
        }

        /// <summary>
        /// Переменная на свойство Fill элемента TabControl 2-ого уровня
        /// </summary>
        private Brush _signalFieldBrush2;

        /// <summary>
        /// Свойство на свойство Fill элемента TabControl 2-ого уровня
        /// </summary>
        public Brush SignalFieldBrush2 {
            get { return _signalFieldBrush2; }
            set {
                _signalFieldBrush2 = value;
                OnPropertyChanged( nameof( SignalFieldBrush2 ) );
            }
        }

        /// <summary>
        /// Переменная указатель на свойство Checked элемента CheckBox "цикл."
        /// </summary>
        private bool _cycleWorkEnable;

        /// <summary>
        /// Свойство указатель на свойство Checked элемента CheckBox "цикл."
        /// </summary>
        public bool CycleWorkEnable {
            get { return _cycleWorkEnable; }
            set {
                _cycleWorkEnable = value;
                OnPropertyChanged( nameof( CycleWorkEnable ) );
            }
        }

        /// <summary>
        /// Переменная указатель на свойство Checked элемента CheckBox "цикл.до ошибки"
        /// </summary>
        private bool _cycleWorkToErrorEnable;

        /// <summary>
        /// Свойство указатель на свойство Checked элемента CheckBox "цикл.до ошибки"
        /// </summary>
        public bool CycleWorkToErrorEnable {
            get { return _cycleWorkToErrorEnable; }
            set {
                _cycleWorkToErrorEnable = value;
                OnPropertyChanged( nameof( CycleWorkToErrorEnable ) );
            }
        }

        /// <summary>
        /// Переменная указатель на свойство Text элемента TextBox для результата теста
        /// </summary>
        private string _resultText;

        /// <summary>
        /// Свойство указатель на свойство Text элемента TextBox для результата теста
        /// </summary>
        public string ResultText {
            get { return _resultText; }
            set {
                _resultText = value;
                OnPropertyChanged( nameof( ResultText ) );
            }
        }

        /// <summary>
        /// Переменная указатель на свойство Foreground элемента TextBox для результата теста
        /// </summary>
        private Brush _resultTextBrush;

        /// <summary>
        /// Свойство указатель на свойство Foreground элемента TextBox для результата теста
        /// </summary>
        public Brush ResultTextBrush {
            get { return _resultTextBrush; }
            set {
                _resultTextBrush = value;
                OnPropertyChanged( nameof( ResultTextBrush ) );
            }
        }
    }
}