using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WpfApplication.Models.Main;

namespace WpfApplication.ViewModels {
    /// <summary>
    /// Класс ViewModel главной формы
    /// </summary>
    public class ApplicationViewModel : BaseViewModel {
        /// <summary>
        /// 
        /// </summary>
        private readonly SemaphoreSlim _semafor = new SemaphoreSlim( 1, 1 );

        /// <summary>
        /// 
        /// </summary>
        private readonly IDialogService _dialogService = new DefaultDialogService();

        /// <summary>
        /// Индекс выбранной вкладки 1-ого уровня
        /// </summary>
        private int _index1;

        /// <summary>
        /// Индекс выбранной вкладки 1-ого уровня
        /// </summary>
        public int Index1 {
            get { return _index1; }
            set {
                _index1 = value;
                OnPropertyChanged( nameof( Index1 ) );
            }
        }

        /// <summary>
        /// Индекс выбранной вкладки 2-ого уровня
        /// </summary>
        private int _index2;

        /// <summary>
        /// Индекс выбранной вкладки 2-ого уровня
        /// </summary>
        public int Index2 {
            get { return _index2; }
            set {
                _index2 = value;
                OnPropertyChanged( nameof( Index2 ) );
            }
        }

        /// <summary>
        /// Текстовое поле
        /// </summary>
        private string _text;

        /// <summary>
        /// Текстовое поле
        /// </summary>
        public string Text {
            get { return _text; }
            set {
                _semafor.Wait();
                _text = value;
                OnPropertyChanged( nameof( Text ) );
                _semafor.Release();
            }
        }

        /// <summary>
        /// Текстовое поле
        /// </summary>
        public string TextLine {
            get { return _text; }
            set {
                _semafor.Wait();
                _text = value + Environment.NewLine;
                OnPropertyChanged( nameof( Text ) );
                _semafor.Release();
            }
        }
        
        /// <summary>
        /// Текстовое поле
        /// </summary>
        private Brush _brush;

        /// <summary>
        /// Текстовое поле
        /// </summary>
        public Brush Brush {
            get { return _brush; }
            set {
                _brush = value;
                OnPropertyChanged( nameof( Brush ) );
            }
        }

        /// <summary>
        /// Минимальное значение ProgressBar
        /// </summary>
        private double _minProgress;

        /// <summary>
        /// Минимальное значение ProgressBar
        /// </summary>
        public double MinProgress {
            get { return _minProgress; }
            set {
                _minProgress = value;
                OnPropertyChanged( nameof( MinProgress ) );
            }
        }

        /// <summary>
        /// Максимальное значение ProgressBar
        /// </summary>
        private double _maxProgress;

        /// <summary>
        /// Максимальное значение ProgressBar
        /// </summary>
        public double MaxProgress {
            get { return _maxProgress; }
            set {
                _maxProgress = value;
                OnPropertyChanged( nameof( MaxProgress ) );
            }
        }

        /// <summary>
        /// Текущеее значение ProgressBar
        /// </summary>
        private double _valueProgress;

        /// <summary>
        /// Текущеее значение ProgressBar
        /// </summary>
        public double ValueProgress {
            get { return _valueProgress; }
            set {
                _valueProgress = value;
                OnPropertyChanged( nameof( ValueProgress ) );
            }
        }

        /// <summary>
        /// Непрерывный ход выполнения ProgressBar
        /// </summary>
        private bool _indeterminateProgress;

        /// <summary>
        /// Непрерывный ход выполнения ProgressBar
        /// </summary>
        public bool IndeterminateProgress {
            get { return _indeterminateProgress; }
            set {
                _indeterminateProgress = value;
                OnPropertyChanged( nameof( IndeterminateProgress ) );
            }
        }

        /// <summary>
        /// Видимость кнопки обнаружения ошибки
        /// </summary>
        private Visibility _visibityFindErrorButton;

        /// <summary>
        /// Видимость кнопки обнаружения ошибки
        /// </summary>
        public Visibility VisibityFindErrorButton {
            get { return _visibityFindErrorButton; }
            set {
                _visibityFindErrorButton = value;
                OnPropertyChanged( nameof( VisibityFindErrorButton ) );
            }
        }

        /// <summary>
        /// Видимость текстового поля обнаружения ошибки
        /// </summary>
        private Visibility _visibityFindErrorText;

        /// <summary>
        /// Видимость текстового поля обнаружения ошибки
        /// </summary>
        public Visibility VisibityFindErrorText {
            get { return _visibityFindErrorText; }
            set {
                _visibityFindErrorText = value;
                OnPropertyChanged( nameof( VisibityFindErrorText ) );
            }
        }

        /// <summary>
        /// Непрерывный ход выполнения ProgressBar
        /// </summary>
        private bool _isEnableLog;

        /// <summary>
        /// Непрерывный ход выполнения ProgressBar
        /// </summary>
        public bool IsEnableLog {
            get { return _isEnableLog; }
            set {
                _isEnableLog = value;
                OnPropertyChanged( nameof( IsEnableLog ) );
            }
        }

        /// <summary>
        /// Непрерывный ход выполнения ProgressBar
        /// </summary>
        private bool _isCheckedLog;

        /// <summary>
        /// Непрерывный ход выполнения ProgressBar
        /// </summary>
        public bool IsCheckedLog {
            get { return _isCheckedLog; }
            set {
                _isCheckedLog = value;
                OnPropertyChanged( nameof( IsCheckedLog ) );
            }
        }

        private readonly ViewModelEventHandling _vm = new ViewModelEventHandling();

        /// <summary>
        /// Команда открыть log-файл
        /// </summary>
        private RelayCommand _openLogFileCommand;

        /// <summary>
        /// Команда открыть log-файл
        /// </summary>
        public RelayCommand OpenLogFileCommand {
            get {
                // ReSharper disable once ArrangeAccessorOwnerBody
                return _openLogFileCommand ??
                       ( _openLogFileCommand = new RelayCommand( _vm.open_log_file ) );
            }
        }

        /// <summary>
        /// Команда закрыть log-файл
        /// </summary>
        private RelayCommand _closeLogFileCommand;

        /// <summary>
        /// Команда закрыть log-файл
        /// </summary>
        public RelayCommand CloseLogFileCommand {
            get {
                // ReSharper disable once ArrangeAccessorOwnerBody
                return _closeLogFileCommand ??
                       ( _closeLogFileCommand = new RelayCommand( _vm.close_log_file ) );
            }
        }

        /// <summary>
        /// Команда запуска одного теста
        /// </summary>
        private RelayCommand _startTestCommand;

        /// <summary>
        /// Команда запуска одного теста
        /// </summary>
        public RelayCommand StartTestCommand {
            get {
                // ReSharper disable once ArrangeAccessorOwnerBody
                return _startTestCommand ??
                       ( _startTestCommand = new RelayCommand( ViewModelStartTest.start_full_test ) );
            }
        }

        /// <summary>
        /// Команда останова выполнения тестов
        /// </summary>
        private RelayCommand _breakTestCommand;

        /// <summary>
        /// Команда останова выполнения тестов
        /// </summary>
        public RelayCommand BreakTestCommand {
            get {
                return _breakTestCommand ??
                       ( _breakTestCommand = new RelayCommand( parameter => App.TaskManager.Break() ) );
            }
        }

        /// <summary>
        /// Команда запуска группы тестов
        /// </summary>
        private RelayCommand _startGroupTestCommand;

        /// <summary>
        /// Команда запуска группы тестов
        /// </summary>
        public RelayCommand StartGroupTestCommand {
            get {
                return _startGroupTestCommand ?? ( _startGroupTestCommand = new RelayCommand( parameter => {
                                   if ( parameter != null ) {
                                       ViewModelStartTest.start_group_test( parameter );
                                   }
                               }
                           )
                       );
            }
        }

        /// <summary>
        /// Команда запуска полного теста
        /// </summary>
        private RelayCommand _startFullTestCommand;

        /// <summary>
        /// Команда запуска полного теста
        /// </summary>
        public RelayCommand StartFullTestCommand {
            get {
                return _startFullTestCommand ?? ( _startFullTestCommand = new RelayCommand( parameter => {
                                   if ( parameter != null ) {
                                       ViewModelStartTest.start_test( parameter );
                                   }
                               }
                           )
                       );
            }
        }

        /// <summary>
        /// Команда сохранения файла
        /// </summary>
        private RelayCommand _saveCommand;

        /// <summary>
        /// Команда сохранения файла
        /// </summary>
        public RelayCommand SaveCommand {
            get {
                return _saveCommand ?? ( _saveCommand = new RelayCommand( obj => {
                    try {
                        _dialogService.SaveFileDialog();
                    }
                    catch ( Exception exc ) {
                        _dialogService.ShowMessage( exc.Message );
                    }
                } ) );
            }
        }

        /// <summary>
        /// Команда открытия файла
        /// </summary>
        private RelayCommand _openCommand;

        /// <summary>
        /// Команда открытия файла
        /// </summary>
        public RelayCommand OpenCommand {
            get {
                return _openCommand ?? ( _openCommand = new RelayCommand( obj => {
                    try {
                        _dialogService.OpenFileDialog();
                    }
                    catch ( Exception exc ) {
                        _dialogService.ShowMessage( exc.Message );
                    }
                } ) );
            }
        }

        /// <summary>
        /// Команда открытия формы инициализации
        /// </summary>
        private RelayCommand _showFormInitCommand;

        /// <summary>
        /// Команда открытия формы инициализации
        /// </summary>
        public RelayCommand ShowFormInitCommand {
            get {
                return _showFormInitCommand ?? ( _showFormInitCommand = new RelayCommand( parameter => {
                    try {
                        var value = parameter as string[];
                        // Проверка допустимости параметров
                        if ( Equals( value, null ) || 1 >= value.Length ) {
                            return;
                        }
                        // Получить диспетчер от текущего окна 
                        // и использовать его для вызова кода обновления
                        var current = Application.Current.Dispatcher;
                        current?.Invoke( DispatcherPriority.Normal,
                            ( ThreadStart ) delegate { _dialogService.ShowFormInit( value[ 0 ], value[ 1 ] ); }
                        );
                    }
                    catch ( Exception exc ) {
                        _dialogService.ShowMessage( exc.Message );
                    }
                } ) );
            }
        }

        /// <summary>
        /// Команда открытия формы сообщения о возникщем исключении
        /// </summary>
        private RelayCommand _showFormErrorCommand;

        /// <summary>
        /// Команда открытия формы сообщения о возникщем исключении
        /// </summary>
        public RelayCommand ShowFormErrorCommand {
            get {
                return _showFormErrorCommand ?? ( _showFormErrorCommand = new RelayCommand( parameter => {
                    try {
                        // Проверка допустимости параметров
                        if ( Equals( parameter, null ) ) {
                            return;
                        }
                        var value = ( StructConvTo2Parameters ) parameter;
                        // Получить диспетчер от текущего окна 
                        // и использовать его для вызова кода обновления
                        var current = Application.Current.Dispatcher;
                        // Диспетчер также предоставляет метод Invoke().Подобно BeginInvoke(), 
                        // он маршализирует указанный код потоку диспетчера. Но в отличие от BeginInvoke(), 
                        // метод Invoke() останавливает поток до тех пор, пока диспетчер выполняет код.
                        // Метод Invoke() можно использовать, если нужно приостановить асинхронную операцию 
                        // до тех пор, пока от пользователя не поступит какой - нибудь отклик.
                        // Например, метод Invoke() можно вызвать для запуска фрагмента кода, 
                        // отображающего диалоговое окно с кнопками ОК и Cancel.
                        // После того как пользователь щелкнет на кнопке и маршализируемый код завершится, 
                        // Invoke() вернет управление, и можно будет продолжить работу
                        // в соответствии с ответом пользователя.
                        //current.BeginInvoke(DispatcherPriority.Normal,
                        //    (ThreadStart)delegate { dialogService.ShowFormError(value.exc, value.text); }
                        //);
                        current.Invoke( DispatcherPriority.Normal,
                            ( ThreadStart ) delegate { _dialogService.ShowFormError( value.Exc, value.Text ); }
                        );
                    }
                    catch ( Exception exc ) {
                        _dialogService.ShowMessage( exc.Message );
                    }
                } ) );
            }
        }
    }

    /// <summary>
    /// Класс, содержащий команды запуска задач
    /// </summary>
    public class ViewModelEventHandling {
        /// <summary>
        /// Переменная для сохранения текста при выводе лог-файла
        /// </summary>
        private string _saveTextBox = string.Empty;

        /// <summary>
        /// Подпрограмма обработчик события показать лог-файл
        /// </summary>
        /// <param name="sender"></param>
        public void open_log_file( object sender ) {
            _saveTextBox                          = App.MyWindows.Text;
            App.MyWindows.Text                    = App.TaskManager.Log.Read();
            App.MyWindows.VisibityFindErrorText   = Visibility.Visible;
            App.MyWindows.VisibityFindErrorButton = Visibility.Visible;
        }

        /// <summary>
        /// Подпрограмма обработчик события скрыть лог-файл
        /// </summary>
        /// <param name="sender"></param>
        public void close_log_file( object sender ) {
            App.MyWindows.Text                    = _saveTextBox;
            App.MyWindows.VisibityFindErrorText   = Visibility.Hidden;
            App.MyWindows.VisibityFindErrorButton = Visibility.Hidden;
        }
    }

    /// <summary>
    /// Класс, содержащий команды запуска задач
    /// </summary>
    public static class ViewModelStartTest {
        /// <summary>
        /// Подпрограмма запуска отдельного теста
        /// </summary>
        /// <param name="obj"></param>
        public static void start_test( object obj ) {
            var index_l1   = ( ( int ) obj >> 24 ) & 0xFF;
            var index_l2   = ( ( int ) obj >> 16 ) & 0xFF;
            var index_page = ( ( int ) obj >> 0 )  & 0xFFFF;
            try {
                // Старт отдельного теста
                var task = new List< int >();
                var dict = App.TaskManager.ConfigProgram.DictCfgTest;
                task.AddRange( dict.Keys.Where( i =>
                    index_page                     == dict[ i ].IndexPage  &&
                    index_l1                       == dict[ i ].IndexTabL1 &&
                    index_l2                       == dict[ i ].IndexTabL2 &&
                    ConfigTestsClass.TypeTask.Test == dict[ i ].Type ) );
                // Добавление теста в словарь
                App.TaskManager.SetTaskAsync( task );
            }
            catch ( InvalidOperationException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute(
                    exc, "Ошибка обработки нажатия кнопки" );
            }
            catch ( ArgumentNullException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute(
                    exc, "Ошибка обработки нажатия кнопки" );
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute(
                    exc, "Ошибка обработки нажатия кнопки" );
            }
        }

        /// <summary>
        /// Подпрограмма запуска группы тестов
        /// </summary>
        /// <param name="obj"></param>
        public static void start_group_test( object obj ) {
            var index_l1 = ( ( int ) obj >> 24 ) & 0xFF;
            var index_l2 = ( ( int ) obj >> 16 ) & 0xFF;
            try {
                var task = new List< int >();
                var dict = App.TaskManager.ConfigProgram.DictCfgTest;
                // Сброс значений CheckBox
                foreach ( var i in dict.Keys.Where( i =>
                    ConfigTestsClass.TypeTask.Test == dict[ i ].Type ) ) {
                    dict[ i ].Control.CycleWorkEnable        = false;
                    dict[ i ].Control.CycleWorkToErrorEnable = false;
                }
                // Старт группы тестов
                task.AddRange( dict.Keys.Where( i =>
                    index_l1                       == dict[ i ].IndexTabL1 &&
                    index_l2                       == dict[ i ].IndexTabL2 &&
                    ConfigTestsClass.TypeTask.Test == dict[ i ].Type ) );
                // Добавление теста в словарь
                App.TaskManager.SetTaskAsync( task );
            }
            catch ( InvalidOperationException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute(
                    exc, "Ошибка обработки нажатия кнопки" );
            }
            catch ( ArgumentNullException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute(
                    exc, "Ошибка обработки нажатия кнопки" );
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute(
                    exc, "Ошибка обработки нажатия кнопки" );
            }
        }

        /// <summary>
        /// Подпрограмма запуска полного теста
        /// </summary>
        /// <param name="obj"></param>
        public static void start_full_test( object obj ) {
            try {
                var task = new List< int >();
                var dict = App.TaskManager.ConfigProgram.DictCfgTest;
                // Сброс значений CheckBox
                foreach ( var i in dict.Keys.Where( i =>
                    ConfigTestsClass.TypeTask.Test == dict[ i ].Type ) ) {
                    dict[ i ].Control.CycleWorkEnable        = false;
                    dict[ i ].Control.CycleWorkToErrorEnable = false;
                }
                // Старт полного теста
                task.AddRange( dict.Keys.Where( i => ConfigTestsClass.TypeTask.Test == dict[ i ].Type ) );
                // Добавление теста в словарь
                App.TaskManager.SetTaskAsync( task );
            }
            catch ( InvalidOperationException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute(
                    exc, "Ошибка обработки нажатия кнопки" );
            }
            catch ( ArgumentNullException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute(
                    exc, "Ошибка обработки нажатия кнопки" );
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute(
                    exc, "Ошибка обработки нажатия кнопки" );
            }
        }
    }

    /// <summary>
    /// Класс управления главной формой
    /// </summary>
    public class ViewModelSetProperty {
        private const    string Unknown       = @"НЕТ ДАННЫХ";
        private const    string Break         = @"ПРЕРВАН";
        private const    string Success       = @"НОРМА";
        private const    string Error         = @"ОТКАЗ";
        private readonly Brush  _colorUnknow  = Brushes.Transparent;
        private readonly Brush  _colorSuccess = new SolidColorBrush( Color.FromArgb( 32, 0, 255, 0 ) );
        private readonly Brush  _colorError   = new SolidColorBrush( Color.FromArgb( 32, 255, 0, 0 ) );

        /// <summary>
        /// Подпрограмма выдачи результата теста в текстовую метку
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nameFile"></param>
        public void init_test( int index, string nameFile = null ) {
            var dict = App.TaskManager.ConfigProgram.DictCfgTest;
            // Проверка является ли задача тестом
            // cо всеми вытекающими характеристиками
            // наличием тестовых полей, кнопок и тд.
            if ( dict[ index ].Type       == ConfigTestsClass.TypeTask.Test &&
                 dict[ index ].IndexTabL1 != null                           &&
                 dict[ index ].IndexTabL2 != null ) {
                // Установка цвета текстового поля
                var brush = _colorUnknow;
                // Заморозка кисти
                brush.Freeze();
                App.MyWindows.Brush = brush;
                // Открытие нужно вкладки
                App.MyWindows.Index1 = ( int ) dict[ index ].IndexTabL1;
                App.MyWindows.Index2 = ( int ) dict[ index ].IndexTabL2;
                // Очистка тесктового поля
                dict[ index ].Control.ResultText      = Unknown;
                dict[ index ].Control.ResultTextBrush = brush;
                // Вывод названия теста
#if DEBUG
                var str = $"{dict[ index ].NameTest}" + Environment.NewLine;
#else
                var str = $"{dict[ index ].NameTest,-35}";
#endif
                App.MyWindows.Text += str;
                App.TaskManager.Log.WriteLineAsync( dict[ index ].NameTest );
            }
            // Данный парамерт используется для работ с файлами и дампами
            // ReSharper disable once InvertIf
            if ( !string.IsNullOrWhiteSpace( nameFile ) ) {
                App.MyWindows.Text += $"Файл {nameFile}" + Environment.NewLine;
                App.TaskManager.Log.WriteLineAsync( $"Файл {nameFile}" );
            }
        }

        /// <summary>
        /// Подпрограмма выдачи результата теста в текстовую метку
        /// </summary>
        /// <param name="index"></param>
        /// <param name="error"></param>
        public void result_test( int index, int error ) {
            // Проверка является ли задача тестом
            // cо всеми вытекающими характеристиками
            // наличием тестовых полей, кнопок и тд.

            var dict = App.TaskManager.ConfigProgram.DictCfgTest;
            if ( ConfigTestsClass.TypeTask.Test != dict[ index ].Type ) {
                return;
            }
            string result;
            Brush  brush;
            // Получение результата
            switch ( error ) {
                case -1: {
                    result = Break;
                    brush  = _colorUnknow;
                    break;
                }
                case 0: {
                    result = Success;
                    brush  = _colorSuccess;
                    break;
                }
                default: {
                    result = Error;
                    brush  = _colorError;
                    break;
                }
            }
            var str = dict[ index ].NameTest.ToLengthExt( " - " + result, 75 );
            // Вывод данных в лог-файл
            App.TaskManager.Log.WriteLineAsync( str + Environment.NewLine );
            // Вывод данных в протокол
            App.TaskManager.Report.WriteLineAsync( str );
            // Вывод результатов теста в текстовое поле
#if DEBUG
            App.MyWindows.TextLine += str + Environment.NewLine;
#else
             App.MyWindows.Text += " - " + result;
#endif
            // Заморозка кисти
            brush.Freeze();
            // Вывод результатов теста в поле результата
            dict[ index ].Control.ResultText      = result;
            dict[ index ].Control.ResultTextBrush = brush;
        }

        /// <summary>
        /// Подпрограмма установки цветового фона элементов после завершения теста
        /// </summary>
        public void set_color() {
            Brush brush;
            var   index = 0;
            // Индексы вкладок
            var index_l1 = 0;
            var index_l2 = 0;
            // Общие счетчики
            var count_task    = 0;
            var count_success = 0;
            var count_error   = 0;
            // Счетчики для вкладок первого уровня
            var count_task_tab_1    = 0;
            var count_success_tab_1 = 0;
            var count_error_tab_1   = 0;
            // Счетчики для вкладок второго уровня
            var count_task_tab_2    = 0;
            var count_success_tab_2 = 0;
            var count_error_tab_2   = 0;

            // Словарь тестов
            var dict = App.TaskManager.ConfigProgram.DictCfgTest;
            // Добавление теста в словарь
            foreach ( var i in dict.Keys.Where( i =>
                !Equals( dict[ i ].Control, null )                   &&
                !Equals( dict[ i ].Control.ResultText, null )        &&
                !Equals( dict[ i ].Control.ResultTextBrush, null )   &&
                !Equals( dict[ i ].Control.SignalFieldBrush1, null ) &&
                !Equals( dict[ i ].Control.SignalFieldBrush2, null ) &&
                ConfigTestsClass.TypeTask.Test == dict[ i ].Type )
            ) {
                if ( dict[ i ].IndexTabL1 != null && dict[ i ].IndexTabL1 != index_l1 ) {
                    // Анализ полученных счетчиков вкладки 1-ого уровня
                    if ( count_success_tab_1 == count_task_tab_1 ) {
                        brush = _colorSuccess;
                    } else {
                        brush = 0 < count_error_tab_1 ? _colorError : _colorUnknow;
                    }
                    // Для потокобезопасности необходимо сделать объект кисть "замороженным"
                    // Чтобы защититься от InvalidOperationException при вызове этого метода, 
                    // проверьте CanFreeze свойства, чтобы определить ли Freezable может быть
                    // сделан неизменяемым перед вызовом этого метода.
                    // Делает текущий объект нередактируемым и определяет для его свойства IsFrozen значение true.
                    brush.Freeze();
                    // Установка цвета
                    dict[ index ].Control.SignalFieldBrush1 = brush;
                    // Сброс счетчиков
                    count_task_tab_1    = 0;
                    count_success_tab_1 = 0;
                    count_error_tab_1   = 0;
                    // Сохранение индекса вкладки
                    index_l1 = ( int ) dict[ i ].IndexTabL1;
                }
                if ( dict[ i ].IndexTabL2 != null && dict[ i ].IndexTabL2 != index_l2 ) {
                    // Анализ полученных счетчиков вкладки 2-ого уровня
                    if ( count_success_tab_2 == count_task_tab_2 ) {
                        brush = _colorSuccess;
                    } else {
                        brush = 0 < count_error_tab_2 ? _colorError : _colorUnknow;
                    }
                    // Заморозка кисти
                    brush.Freeze();
                    // Установка цвета
                    dict[ index ].Control.SignalFieldBrush2 = brush;
                    // Сброс счетчиков
                    count_task_tab_2    = 0;
                    count_success_tab_2 = 0;
                    count_error_tab_2   = 0;
                    // Сохранение индекса вкладки
                    index_l2 = ( int ) dict[ i ].IndexTabL2;
                }
                // Анализ результатов задачи
                if ( Success == dict[ i ].Control.ResultText &&
                     Equals( _colorSuccess, dict[ i ].Control.ResultTextBrush ) ) {
                    // Инкремент положительного теста
                    count_success++;
                    count_success_tab_1++;
                    count_success_tab_2++;
                } else {
                    if ( Error == dict[ i ].Control.ResultText ||
                         Equals( _colorError, dict[ i ].Control.ResultTextBrush ) ) {
                        // Инкремент отрицательного теста
                        count_error++;
                        count_error_tab_1++;
                        count_error_tab_2++;
                    }
                }
                // Инкремент количества задач на вкладке 2-ого уровня
                count_task++;
                count_task_tab_1++;
                count_task_tab_2++;
                // Сохранение индекса задачи
                index = i;
            }
            // Установка цветов для крайних вкладок 1-ого и 2-ого уровня
            // Анализ полученных счетчиков вкладки 1-ого уровня
            if ( count_success_tab_1 == count_task_tab_1 ) {
                brush = _colorSuccess;
            } else {
                brush = 0 < count_error_tab_1 ? _colorError : _colorUnknow;
            }
            // Заморозка кисти
            brush.Freeze();
            // Установка цвета
            dict[ index ].Control.SignalFieldBrush1 = brush;
            // Анализ полученных счетчиков вкладки 2-ого уровня
            if ( count_success_tab_2 == count_task_tab_2 ) {
                brush = _colorSuccess;
            } else {
                brush = 0 < count_error_tab_2 ? _colorError : _colorUnknow;
            }
            // Заморозка кисти
            brush.Freeze();
            // Установка цвета
            dict[ index ].Control.SignalFieldBrush2 = brush;
            // Анализ полученных счетчиков
            if ( count_success == count_task ) {
                brush = _colorSuccess;
            } else {
                brush = 0 < count_error ? _colorError : _colorUnknow;
            }
            // Заморозка кисти
            brush.Freeze();
            // Установка цвета текстового поля
            App.MyWindows.Brush = brush;
        }
    }
}