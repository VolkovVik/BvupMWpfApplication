using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using WpfApplication.Models.Main;
using WpfApplication.Models.Main.XML;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.LoadHexFile {
    /// <summary>
    /// Класс загрузки НЕХ файла
    /// </summary>
    internal class ManagerLoadHexFileClass {
        /// <summary>
        /// Количество потоков, используется для контроля контрольной суммы
        /// </summary>
        private long _intChecksum;

        /// <summary>
        /// Результат проверки контрольной суммы
        /// </summary>
        private string _strChecksum;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public int InitTaskManager( int param = 0 ) {
            ManagerAsync().GetAwaiter().GetResult();
            return 0;
        }

        /// <summary>
        ///  Подпрограмма загрузки НЕХ файла
        /// </summary>
        /// <returns></returns>
        private async Task ManagerAsync() {
            var count_err = 0;

            // Проверка инициализации СОМ-портов 
            if ( null == App.TaskManager.PortCom || !App.TaskManager.PortCom.IsInit ) {
                throw new Exception< DeviceNotInitializedExceptionArgs >( new DeviceNotInitializedExceptionArgs(),
                    "Ошибка! СОМ-порт не инициализирован" );
            }
            // Инициализация значений
            _strChecksum = string.Empty;
            _intChecksum = PortRs232.MaxDevice;
            // Запуск диалогового окна
            var title = $"Окно запуска ПО КТА {App.TaskManager.ConfigProgram.NameDevice}";
            var text  = $"Включите питание блока {App.TaskManager.ConfigProgram.NameDevice}";
            App.MyWindows.ShowFormInitCommand.Execute( new[] { title, text } );
            // Загрузка НЕХ-файла
            CommonClass.SetText( "Загрузка НЕХ файлов..." );
            // Запуск задач
            try {
                var list = new List< Task< int > > {
                    Task.Run( () => Load( 0 ) ),
                };
                var answer = await Task.WhenAll( list );
                count_err = answer.Sum();
            }
            catch ( OperationCanceledException exc ) {
                App.MyWindows.Text +=
                    exc.Message                                 + Environment.NewLine +
                    "Выполнение задачи прервано пользователем." + Environment.NewLine;
            }
            App.MyWindows.TextLine +=
                "Загрузка НЕХ файлов завершена "                +
                ( 0 != count_err ? "с ошибками." : "успешно." ) +
                Environment.NewLine                             +
                "Связь с КТА для проверки "                     +
                App.TaskManager.ConfigProgram.NameDevice        +
                ( 0 == count_err ? " установлена." : " установить не удалось." );
        }

        /// <summary>
        /// Подпрограмма загрузки НЕХ-файла
        /// </summary>
        /// <param name="index"></param>
        private int Load( int index ) {
            var log       = string.Empty;
            var time      = DateTime.Now;
            var path      = string.Empty;
            var count_err = 100;
            try {
                // Проверка необходимости загрузка НЕХ-файла в заданный канал
                if ( new XmlClass().Read( XmlClass.NameDebug, "load" + index ) == "YES" ) {
                    // Получение пути доступа к файлу
                    var answer = GetPathFile( index );
                    path         =  answer.Item4;
                    _strChecksum += answer.Item3;
                } else {
                    // Режим отладки
                    App.TaskManager.IdEnabledsDictionary[ index ].Set( true );
                    App.MyWindows.Text +=
                        App.TaskManager.NameChanneList[ index ] +
                        " включен режим отладки"                +
                        Environment.NewLine;
                }
                // Событие завершения проверки контрольной суммы
                if ( 0 == Interlocked.Decrement( ref _intChecksum ) ) {
                    if ( !string.IsNullOrWhiteSpace( _strChecksum ) ) {
                        Task.Run( () => MessageBox.Show(
                            messageBoxText:
                            "Проверка контрольных сумм МПМ завершена с ошибками." +
                            Environment.NewLine                                   +
                            _strChecksum,
                            caption: "Загрузка НЕХ-файлов",
                            button: MessageBoxButton.OK,
                            icon: MessageBoxImage.Error
                        ) );
                    }
                }
                if ( !string.IsNullOrWhiteSpace( path ) ) {
                    // Загрузка НЕХ-файла
                    log += new LoadHexFileClass( index, path ).Load();
                    // Время завершения загрузки НЕХ-файла
                    var time_loader = DateTime.Now - time;
                    log = $"{App.TaskManager.NameChanneList[ index ]}{Environment.NewLine}" +
                          "время загрузки НЕХ-файла: "                                      +
                          $"{time_loader.Hours:D2}:{time_loader.Minutes:D2}:"               +
                          $"{time_loader.Seconds:D2}:{time_loader.Milliseconds:D3}"         +
                          $"{Environment.NewLine}Лог загрузки НЕХ-файла"                    +
                          $"{Environment.NewLine}{log}";
                    CommonClass.SetText( log );
                }
                count_err = 0;
            }
            catch ( Exception< Rs232ExceptionArgs > exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( ObjectDisposedException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            return count_err;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private static Tuple< string, string, string, string > GetPathFile( int port ) {
            var path     = string.Empty;
            var checksum = string.Empty;
            // Команда V
            var log  = App.TaskManager.PortCom.SetCommandVersion( port );
            var log1 = log.Replace( " = ", " " );
            // Разнородность в выдаче информации
            // Первые строки всегда одинаковые, например
            // MPM MPC8308 LOADER
            // ver 1.0  CS_LOADER = 1046CA17h  CS_USER = 4FB64385h
            // Команда V для получения контрольных сумм
            // может выдавать несколько типов сообщенй
            // MPC8308 LOADER ver 1.0  CS_BIOS 1046CA17h  CS_USER 4FB64385h
            // MPC8308 LOADER ver 1.0  CS_LOADER 1046CA17h  CS_USER 4FB64385h
            // MPC8308 LOADER ver 1.0  CS_LOADER = 1046CA17h  CS_USER = 4FB64385h
            // МПМ MPC8308 LOADER ver 1.0  CS_LOADER = 1046CA17h  CS_USER = 4FB64385h
            // Получение контрольной суммы первоначального загрузчика
            var check_sum = log1.ToSubstringExt( "CS_BIOS" )?.Replace( "h", "" ).ToUintExt( 16 ) ??
                            log1.ToSubstringExt( "CS_LOADER" )?.Replace( "h", "" ).ToUintExt( 16 );
            // Получение наименование процессора
            var name = log1.ToSubstringExt( "МПМ" ) ?? log1.ToSubstringExt( "VM-7" );
            // Получение имени hex-файла
            var list = new XmlClass().Read( XmlClass.NameHexFiles );
            for ( var i = 0; i < list.Count; i += 3 ) {
                if ( list[ i ] != name || list.Count <= i + 2 ) continue;
                // Проверка контрольной суммы
                var cpu_cs = list[ i + 1 ].ToUintWithoutBaseExt();
                if ( cpu_cs == null || cpu_cs != check_sum ) {
                    checksum += App.TaskManager.NameChanneList[ port ]                   +
                                $"  {name} = 0x{check_sum:X8} вместо 0x{cpu_cs ?? 0:X8}" +
                                Environment.NewLine;
                }
                // Путь доступа к файлу 
                path = list[ i + 2 ];
                // Проверка является ли путь абсолютным
                if ( !Path.IsPathRooted( path ) ) {
                    // Путь доступа к файлу настроек НЕХ файлов
                    path = AppDomain.CurrentDomain.BaseDirectory      +
                           App.TaskManager.ConfigProgram.PathHexFiles + path;
                }
                var str = $"{App.TaskManager.NameChanneList[ port ]}процессор {name}" +
                          $" c контрольной суммой загрузчика 0x{check_sum:X8}"        +
                          Environment.NewLine;
                // Вывод пользователю
                App.MyWindows.Text += str;
                App.TaskManager.Log.WriteLineAsync( str + "path: " + path );
                break;
            }
            return new Tuple< string, string, string, string >( log, name, checksum, path );
        }

        /// <summary>
        /// Подпрограмма проверки готовности монитора
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static string check_ready( int port ) {
            var str = App.TaskManager.PortCom.GetCheckReady( port );
            // Получение контрольных сумм и наименования процессора
            // Данное действие пришлось реализовать из  необходимости смены настроек СОМ порта
            // При неверных настройках выдать команду V невозможно
            var name = str.ToSubstringExt( "MPM" ) ?? str.ToSubstringExt( "VM-7" );
            // Задание настроек в зависимости от типа процессора
            App.TaskManager.PortCom.SetHandshake( port,
                "MPC5200" == name || "MCF5485" == name ? Handshake.RequestToSend : Handshake.None );
            return str;
        }
    }
}