// http://www.csharp-examples.net/string-format-double/

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.VisualBasic.ApplicationServices;
using WpfApplication.Models.Main;
using WpfApplication.ViewModels;
using WindowException = WpfApplication.Models.Form.WindowException;

// Проверка компилятором кода на совместимость с CLS
// [assembly: CLSCompliant(true)]
namespace WpfApplication {
    /// <summary>
    /// Класс cозданиz оболочки для приложения одного экземпляра
    /// </summary>
    public static class Startup {
        [STAThread]
        public static void Main( string[] args ) {
            try {
                bool semaphore_was_created;
                // ReSharper disable once UnusedVariable
                var semaphore = new Semaphore( 1, 1, "2CD69B68-408E-4C8F-A943-3229F5B523F8",
                    out semaphore_was_created );
                if ( !semaphore_was_created ) {
                    MessageBox.Show( "Программа уже выполняется..." );
                } else {
                    var wrapper = new SingleInstanceApplicationWrapper();
                    wrapper.Run( args );
                }
            }
            catch ( ArgumentOutOfRangeException exc ) {
                MessageBox.Show( $"{exc.Message}" );
            }
            catch ( ArgumentException exc ) {
                MessageBox.Show( $"{exc.Message}" );
            }
            catch ( IOException exc ) {
                MessageBox.Show( $"{exc.Message}" );
            }
            catch ( UnauthorizedAccessException exc ) {
                MessageBox.Show( $"{exc.Message}" );
            }
            catch ( WaitHandleCannotBeOpenedException exc ) {
                MessageBox.Show( $"{exc.Message}" );
            }
        }
    }

    /// <summary>
    /// Класс cоздания оболочки для приложения одного экземпляра
    /// наследование от Microsoft.VisualBasic.dll
    /// </summary>
    public class SingleInstanceApplicationWrapper : WindowsFormsApplicationBase {
        /// <summary>
        /// Create the WPF application class.
        /// </summary>
        private App _app;

        /// <summary>
        /// Подпрограмма задания одновременного выполнения только одной своей копии.
        /// </summary>
        public SingleInstanceApplicationWrapper() {
            // Определяет, является ли это приложение приложением, 
            // допускающим одновременное выполнение только одной своей копии.
            // Метод Run использует это свойство, чтобы определить, 
            // является ли это приложение приложением, допускающим 
            // одновременное выполнение только одной своей копии.
            IsSingleInstance = true;
        }

        /// <summary>
        /// Подпрограмма инициализации последующих экземпляров программы
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartupNextInstance( StartupNextInstanceEventArgs e ) {
            MessageBox.Show( "Программа уже выполняется." );
        }

        /// <summary>
        /// Подпрограмма инициализации первого экземпляра программы
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool OnStartup( Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e ) {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            if ( UacClass.CheckDirectory( path ) ) {
                // Доступ к рабочей директории текущим пользователем возможен
                Run();
            } else {
                // Права доступа текущего пользователя Windows
                // не позволяют получить доступ к рабочей директории
                if ( UacClass.IsAdmin() ) {
                    // Пользователь обладает правами Администатора
                    // но права на доступ к папке 
                    // по неизвестным причинам отсутствуют
                    MessageBox.Show(
                        "Права доступа текущего пользователя Windows" +
                        Environment.NewLine                           +
                        "не позволяют получить доступ к директории",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error );
                } else {
                    // Пользователю не предоставлены права Администатора
                    // Запуск процесса с правами Администратора
                    UacClass.RunAsAdmin();
                }
            }
            return false;
        }

        /// <summary>
        /// Подпрограмма запуска приложения
        /// </summary>
        private void Run() {
            try {
                // Для проверки установлен ли на компьютер пакет .NET Framework
                // необходимо удостоверится в наличии файла mscorees.dll 
                // в каталоге %SystemRoot%\system32

                // Показ окна загрузки программы
                App.SplashScreen.Show( false );
                App.SplashScreen.Close( new TimeSpan( 1000 ) );
                // Загрузка библиотек .DLL
                load_dll();
                // Создание пользовательского окна
                var window = new Views.MainWindow();
                // Запуск приложения
                _app = new App();
                // Задание подпрограммы инициализации программы  
                _app.Startup += App_Startup;
                // Задание подпрограммы завершения программы
                _app.Exit += App_Exit;
                _app.Run( window );
            }
            catch ( TargetInvocationException exc ) {
                new WindowException( exc, "Ошибка инициализации приложения" ).ShowDialog();
            }
            catch ( UnauthorizedAccessException exc ) {
                // The access requested is not permitted by the operating system  
                // for the specified path, such as when access is Write or  
                // ReadWrite and the file or directory is set for read-only access. 
                new WindowException( exc, "Ошибка инициализации приложения" ).ShowDialog();
            }
            catch ( AnimationException exc ) {
                new WindowException( exc, "Ошибка инициализации приложения" ).ShowDialog();
            }
            catch ( Exception exc ) {
                new WindowException( exc, "Ошибка инициализации приложения" ).ShowDialog();
                throw;
            }
        }

        /// <summary>
        /// Подпрограмма загрузки библиотек .dll
        /// </summary>
        private static void load_dll() {
#if DEBUG
            var resource = Resolver.GetAllAssemblyResource();
            App.MyWindows.TextLine += "Загружаемые приложением сборки:";
            foreach ( var str in resource ) {
                App.MyWindows.TextLine += str;
            }
#endif
            // Для текущего домена приложения вешаем свой обработчик,
            // в котором и будем вручную подсовывать нужные сборки
            AppDomain.CurrentDomain.AssemblyResolve += Resolver.CurrentDomain_AssemblyResolve;
            // DLL созданы с помощью управляемого кода 
            // и AssemblyResolve их банально не видит
            // посему втупую загружаю их из .exe
            Resolver.ExtractDllResourceToFile( "PCI429_4.dll" );
            // Инициализация интерфейсных плат Advantech
            // ReSharper disable once UnusedVariable
            var assembly = Assembly.Load( "Automation.BDaq" );
        }

        /// <summary>
        /// Подпрограмма запуска задачи инициализации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void App_Startup( object sender, EventArgs e ) {
            App.MyWindows.VisibityFindErrorText   = Visibility.Hidden;
            App.MyWindows.VisibityFindErrorButton = Visibility.Hidden;
            // Проверка допустимости работы с словарем
            var dict = App.TaskManager.ConfigProgram.DictCfgTest;
            if ( Equals( dict, null ) || dict.Count == 0 ) {
                App.MyWindows.TextLine += "Ошибка инициализации словаря задач";
                return;
            }
            // Запуск задачи
            App.TaskManager.SetTaskAsync( new[]
                { ( int ) ConfigTestsClass.IdTest.Init, ( int ) ConfigTestsClass.IdTest.Load } );
        }

        /// <summary>
        /// Подпрограмма завершения работы 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void App_Exit( object sender, ExitEventArgs e ) {
            // Остановить тестирование
            App.TaskManager.Break();
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public class App : Application {
        /// <summary>
        /// Заставка ПО
        /// </summary>
        public static readonly SplashScreen SplashScreen = new SplashScreen( "models/icon/kret.png" );

        /// <summary>
        /// Данные для привязки элементов
        /// </summary>
        public static readonly ApplicationViewModel MyWindows = new ApplicationViewModel();

        /// <summary>
        /// Объект организации работы программы
        /// </summary>
        public static readonly TaskManagerClass TaskManager = new TaskManagerClass( "БВУП-М" );
    }
}