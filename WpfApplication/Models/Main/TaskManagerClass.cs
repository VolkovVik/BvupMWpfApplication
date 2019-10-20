using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using WpfApplication.ViewModels;
using System.Collections.Generic;
using AppLibrary.Learn.Structures;
using WpfApplication.Models.PciCard.rs232;
using WpfApplication.Models.PciCard.arinc429;
using WpfApplication.Models.PciCard.advantech.pci1716;
using WpfApplication.Models.PciCard.advantech.pci1721;
using WpfApplication.Models.PciCard.advantech.pci1724u;
using WpfApplication.Models.PciCard.advantech.pci1747u;
using WpfApplication.Models.PciCard.advantech.pci1753;
using WpfApplication.Models.Test;

namespace WpfApplication.Models.Main {
    /// <inheritdoc />
    /// <summary>
    /// Класс выполнения задач 
    /// </summary>
    public class TaskManagerClass : IDisposable {
        /// <summary>
        /// Имя устройства
        /// </summary>
        public string DeviceName { get; private set; }

        /// <summary>
        /// Структура настройки программы
        /// </summary>
        public readonly ConfigProgramClass ConfigProgram;

        /// <summary>
        /// Название каналов
        /// </summary>
        public readonly List< string > NameChanneList = new List< string >();

        /// <summary>
        /// Словарь предназначенный для организации работы тестов 
        /// </summary>
        public readonly Dictionary< int, CheckIdClass > IdEnabledsDictionary = new Dictionary< int, CheckIdClass >();

        /// <summary>
        /// Лог-файл
        /// </summary>
        public readonly FilesClass Log = new FilesClass( "log\\log.txt", App.MyWindows.ShowFormErrorCommand.Execute );

        /// <summary>
        /// Лог-файл
        /// </summary>
        public readonly FilesClass Report =
            new FilesClass( "log\\report.txt", App.MyWindows.ShowFormErrorCommand.Execute );

        /// <summary>
        /// Объект работы с аналоговыми сигналами выдачи
        /// </summary>
        public Port1716 Port1716;

        /// <summary>
        /// Объект работы с аналоговыми сигналами приема
        /// </summary>
        public Port1721 Port1721;

        /// <summary>
        /// Объект работы с аналоговыми сигналами приема
        /// </summary>
        public Port1724U Port1724U;

        /// <summary>
        /// Объект работы с аналоговыми сигналами выдачи
        /// </summary>
        public Port1747U Port1747U;

        /// <summary>
        /// Объект работы с разовыми командами
        /// </summary>
        public Port1753 Port1753;

        /// <summary>
        /// Объект работы по каналам ПБК
        /// </summary>
        public Port429 PortArinc;

        /// <summary>
        /// Объект работы по каналам RS-232
        /// </summary>
        public PortRs232 PortCom;

        /// <summary>
        /// Семафор
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim( 1, 1 );

        /// <summary>
        /// 
        /// </summary>
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Очередь диспетчера задач
        /// </summary>
        private readonly MyDequeClass< int > _myQueue = new MyDequeClass< int >();
        
        /// <summary>
        /// Признак загрузки НЕХ-файла
        /// </summary>
        private bool _signLoadHexFile;

        /// <summary>
        /// Признак полного теста устройства
        /// </summary>
        private bool _signFullTest;

        /// <summary>
        /// Признак выпонения теста интерфейсов
        /// </summary>
        private bool _signTests;

        /// <summary>
        /// Конструктор
        /// </summary>
        public TaskManagerClass( string deviceName ) {
            DeviceName = deviceName;
            // Название каналов
            NameChanneList.Add( $"{DeviceName,-8}" );
            // Задание настроек программе
            ConfigProgram = new ConfigProgramClass( DeviceName );
            // Сброс
            IdEnabledsDictionary.Clear();
            for ( var i = 0; i < PortRs232.MaxDevice; i++ ) {
                IdEnabledsDictionary.Add( i, new CheckIdClass() );
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~TaskManagerClass() {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose( false );
        }

        /// <summary>
        /// Подпрограмма выполняющая определяемые приложением задачи, 
        /// связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose() {
            // Implement IDisposable.
            // Do not make this method virtual.
            // A derived class should not be able to override this method.
            Dispose( true );
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        // ReSharper disable once RedundantDefaultMemberInitializer
        private bool _disposed = false;

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose( bool disposing ) {
            // Check to see if Dispose has already been called.
            if ( _disposed ) {
                return;
            }
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if ( disposing ) {
                // Dispose managed resources.
                // Сброс 
                IdEnabledsDictionary.Clear();
                NameChanneList.Clear();
                Log.Dispose();
                Report.Dispose();
                _semaphore.Dispose();
                _cancelTokenSource.Dispose();
            }
            // Dispose native ( unmanaged ) resources, if exits

            // Note disposing has been done.
            _disposed = true;
        }

        /// <summary>
        /// Подпрограмма проверки состояния токена отмены
        /// </summary>
        /// <returns></returns>
        public void СheckСancelToken() {
            // Poll on this property if you have to do other cleanup before throwing.
            if ( _cancelTokenSource != null && _cancelTokenSource.Token.IsCancellationRequested ) {
                // Clean up here, then...
                _cancelTokenSource.Token.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// Подпрограмма остановки тестирования
        /// </summary>
        public void Break() {
            // Остановка выполнения задач
            _cancelTokenSource?.Cancel( false );
        }

        /// <summary>
        /// Подпрограмма запуска задач на выполнение
        /// </summary>
        /// <param name="list"></param>
        public async void SetTaskAsync( IList< int > list ) {
            if ( list == null || list.Count == 0 ) return;
            // Проверка прохождения полного теста устройства
            _signFullTest = list.Count ==
                            App.TaskManager.ConfigProgram.DictCfgTest.Count( item => item.Value.Type ==
                                                                                     ConfigTestsClass.TypeTask.Test );
            // Проверка прохождения именно теста устройства
            _signTests = App.TaskManager.ConfigProgram.DictCfgTest[ list[ 0 ] ].Type ==
                         ConfigTestsClass.TypeTask.Test;
            // Сброс очереди
            _myQueue.Clear();
            // При попытке запуска тестов и сброшеном признаке загрузки НЕХ-файла
            // требуется сначала загрузить НЕХ-файл
            if ( !_signLoadHexFile && _signTests ) {
                _myQueue.PushFirst( ( int ) ConfigTestsClass.IdTest.Load );
            }
            // Загрузка новой очереди
            foreach ( var item in list ) {
                _myQueue.PushLast( item );
            }
            App.MyWindows.TextLine += _myQueue;
            // Ожидание и захват семафора
            if ( !_semaphore.Wait( 0 ) ) {
                App.MyWindows.TextLine += "Ошибка запуска задачи при выполнении других задач";
                return;
            }
            // Объект скоординированной отмены
            _cancelTokenSource.Dispose();                       // Clean up old token source....
            _cancelTokenSource = new CancellationTokenSource(); // "Reset" the cancellation token source..
            try {
                // Асинхронное выполнение задач
                await Task.Run( () => TaskManager( _cancelTokenSource.Token ) );
            }
            catch ( OperationCanceledException exc ) {
                App.MyWindows.TextLine +=
                    exc.Message + Environment.NewLine + "Выполнение задачи прервано пользователем";
            }
            finally {
                // Сброс семафора
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Подпрограмма запуска тестов
        /// </summary>
        /// <param name="ct"></param>
        private void TaskManager( CancellationToken ct ) {


            //var sw11 = Stopwatch.StartNew();

            //var db = new ApplicationContext();
            //db.Devices.Load();
            //var iii = db.Devices.Local;

            //db.Devices.Add(
            //    new Device( 555555, DateTime.Now, "Иванов", "Петров", "Сидоров", 10, "НОРМА", "хрень", "файл4", DatabaseClass.GetByte( "111.txt" ) ) );
            //db.SaveChanges();

            //var str = sw11.Elapsed.ToString( "g" );

            //return;


            //App.MyWindows.Text = "";
            //var data = new DatabaseClass( "test.sqlite" );
            //data.Create( "Devices" );
            //var list_out = new List< Device > {
            //    new Device( 111111, DateTime.Now, "Иванов", "Петров", "Сидоров", 10, "НОРМА", "хрень", "файл1", DatabaseClass.GetByte( "111.txt" ) ),
            //    new Device( 222222, DateTime.Now, "Иванов", "Петров", "Сидоров", 10, "НОРМА", "хрень", "файл2", DatabaseClass.GetByte( "222.txt" ) ),
            //    new Device( 333333, DateTime.Now, "Иванов", "Петров", "Сидоров", 10, "НОРМА", "хрень", "файл3", DatabaseClass.GetByte( "333.txt" ) )
            //};
            //foreach( var items in list_out ) {
            //    data.Insert( "Devices", items );
            //}
            //var list = data.Select( "Devices" );
            ////data.Delete( "device08" );
            ////data.Drop( "device08" );

            //return;

            var sw = Stopwatch.StartNew();
            // Настройки TextBox
            App.MyWindows.Text = string.Empty;
            // Настройки ProgressBar
            App.MyWindows.IndeterminateProgress = false;
            App.MyWindows.MinProgress           = 1;
            App.MyWindows.MaxProgress =
                _myQueue.Sum( item => App.TaskManager.ConfigProgram.DictCfgTest[ item ].MaxProgress );
            App.MyWindows.ValueProgress = 1;
            // Сброс элементов CheckBox
            App.MyWindows.IsEnableLog  = false;
            App.MyWindows.IsCheckedLog = false;
            // Сброс данных логов
            App.TaskManager.Log.Delete();
            App.TaskManager.Report.Delete();
            // Тест
            var count_err = StartTest( ct );
            // Сохранение логов в заданной директории
            CommonClass.SaveLogFiles( _signFullTest, count_err );
            // Разрещение обращения к элементу CheckBox
            App.MyWindows.IsEnableLog = true;
#if DEBUG
            App.MyWindows.TextLine += "Значение ProgressBar:        "         +
                                      $"min: {App.MyWindows.MinProgress}  "   +
                                      $"max: {App.MyWindows.MaxProgress}  "   +
                                      $"value: {App.MyWindows.ValueProgress}" +
                                      Environment.NewLine                     +
                                      "Время выполнения:".ToLengthExt( sw.Elapsed.ToString( "g" ), 75 );
#endif
            // Сброс ProgressBar
            App.MyWindows.ValueProgress = App.MyWindows.MinProgress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private int StartTest( CancellationToken ct ) {
            // Количество ошибок
            //  -1 - тест прерван пользователем
            //   0 - тест завершен успешно
            //  >0 - тест завершен с ошибками
            var count_err = 0;

            int? i_channel    = -1;
            var  set_property = new ViewModelSetProperty();
            var  dict         = App.TaskManager.ConfigProgram.DictCfgTest;
            // Время запуска теста
            var time = DateTime.Now;
            // Создание заголовка log-файла
            CommonClass.SetHeaderLog( time, _signFullTest );
            // Запуск тестирования
            foreach ( var item in _myQueue ) {
                // Проверка идентификатора устройства
                i_channel = CommonClass.SetTypeChannel( item, i_channel );
                // Проверка допустимости выпонения функции
                if ( dict[ item ].Function == null ) continue;
                // Счетчик проходов
                var count_step = 0;
                // Цикл проходов теста
                do {
                    // Проверка признака прерывания теста
                    if ( ct.IsCancellationRequested ) break;
                    if ( 0 < count_step ) {
                        // При первом или одиночном проходе ничего этого делать не нужно
                        App.MyWindows.TextLine      += $"Проход {count_step:D}";
                        App.MyWindows.ValueProgress =  App.MyWindows.MinProgress;
                    }
                    // Сброс метки результата
                    set_property.init_test( item );
                    // Запуск теста
                    var count_err_test = KernTest( item );
                    if ( item == ( int ) ConfigTestsClass.IdTest.Load && count_err_test == 0 ) {
                        _signLoadHexFile = true;
                    }
                    // Вывод результата пользователю
                    set_property.result_test( item, count_err_test );
                    count_err += count_err_test;
                    count_step++;
                } while ( dict[ item ].Type == ConfigTestsClass.TypeTask.Test &&
                          ( dict[ item ].Control.CycleWorkEnable ||
                            dict[ item ].Control.CycleWorkToErrorEnable && count_err == 0 ) );

                // Установка цветого фона элементов
                set_property.set_color();
            }
            // Прерывание тестирования модуля
            if ( ct.IsCancellationRequested ) count_err = -1;
            // Результаты тестирования выдаются только на тесты
            // Дополнительные и системные задачи результат не выдают
            if ( _signTests ) CommonClass.SetResultText( count_err, _signFullTest );
            // Запись результата теста в log-файл
            CommonClass.SetResultLog( time );
            return count_err;
        }

        /// <summary>
        /// Подпрограмма выбора теста
        /// </summary>
        /// <returns></returns>
        private static int KernTest( int index ) {
            var count_err = 0;
            var dict      = App.TaskManager.ConfigProgram.DictCfgTest;
            try {

                count_err += dict[ index ].Function( index );
            }
            catch ( Exception< Rs232ExceptionArgs > exc ) {
                count_err = ViewError( index, exc, 1001 );
            }
            catch ( Exception< Pci429ExceptionArgs > exc ) {
                count_err = ViewError( index, exc, 1002 );
            }
            catch ( Exception< Pci1716ExceptionArgs > exc ) {
                count_err = ViewError( index, exc, 1003 );
            }
            catch ( Exception< Pci1721ExceptionArgs > exc ) {
                count_err = ViewError( index, exc, 1004 );
            }
            catch ( Exception< Pci1724UExceptionArgs > exc ) {
                count_err = ViewError( index, exc, 1005 );
            }
            catch ( Exception< Pci1747UExceptionArgs > exc ) {
                count_err = ViewError( index, exc, 1006 );
            }
            catch ( Exception< Pci1753ExceptionArgs > exc ) {
                count_err = ViewError( index, exc, 1007 );
            }
            catch ( Exception< Upc10ExceptionArgs > exc ) {
                count_err = ViewError( index, exc, 1008 );
            }
            catch ( Exception< ParameterNotFoundExceptionArgs > exc ) {
                count_err = ViewError( index, exc, 1009 );
            }
            catch ( Exception< IdEnabledsNotValidExceptionArgs > exc ) {
                count_err = 11;
#if DEBUG
                App.MyWindows.TextLine += exc.Message;
#endif
                App.TaskManager.Log.WriteLineAsync( exc.Message );
            }
            catch ( Exception< DeviceNotInitializedExceptionArgs > exc ) {
                count_err = 12;
#if DEBUG
                App.MyWindows.TextLine += exc.Message;
#endif
                App.TaskManager.Log.WriteLineAsync( exc.Message );
            }
            catch ( ObjectDisposedException exc ) {
                count_err = ViewError( index, exc, 1100 );
            }
            catch ( ArgumentNullException exc ) {
                count_err = ViewError( index, exc, 1101 );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                count_err = ViewError( index, exc, 1102 );
            }
            catch ( ArgumentException exc ) {
                count_err = ViewError( index, exc, 1103 );
            }
            catch ( AbandonedMutexException exc ) {
                count_err = ViewError( index, exc, 1104 );
            }
            catch ( InvalidOperationException exc ) {
                count_err = ViewError( index, exc, 1105 );
            }
            catch ( NotSupportedException exc ) {
                count_err = ViewError( index, exc, 1106 );
            }
            catch ( OperationCanceledException ) {
                count_err = 107;
                //ViewError( index, exc );
            }
            // ReSharper disable once RedundantEmptyFinallyBlock
            finally { }
            return count_err;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="exc"></param>
        /// <param name="error"></param>
        private static int ViewError( int id, Exception exc, int error  ) {
            var dict = App.TaskManager.ConfigProgram.DictCfgTest;
            dict[id].Control.CycleWorkEnable        = false;
            dict[id].Control.CycleWorkToErrorEnable = false;
            App.MyWindows.ShowFormErrorCommand.Execute( exc, "Возникло исключение в процессе выполнения задачи" );
            return error;
        }
    }
}