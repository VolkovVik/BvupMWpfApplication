using System;
using System.Text;
using System.Windows;
using WpfApplication.Models.Main;

namespace WpfApplication.Models.Test.LoadHexFile {
    internal class LoadHexFileClass {
        /// <summary>
        /// Имя файла
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string FileName { get; private set; }

        /// <summary>
        /// Индекс порта
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public int Port { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="port"></param>
        public LoadHexFileClass( int port, string fileName ) {
            Port     = port;
            FileName = fileName;
        }

        /// <summary>
        /// Объекты для работы с СОМ-портом
        /// </summary>
        /// <summary>
        /// Подпрограмма загрузки НЕХ-файла
        /// </summary>
        public string Load() {
            var log = new StringBuilder();

            // Чтение данных НЕХ-файла
            var data   = new byte[ 0 ];
            var readed = new FilesClass( FileName, App.MyWindows.ShowFormErrorCommand.Execute ).Read( data );
            // При загрузке последней строки НЕХ-файла символ '\r' является Enter для строки
            // Символ '\n' воспринимается как команду получаемую после загрузки НЕХ-файла
            // и добавляет после загрузки фразу \r\n.UNKNOW COMMAND\r\n.
            // Соответственно исключаем ее
            if ( readed             >= 2             &&
                 data.Length        >= 2             &&
                 data[ readed - 2 ] == ( byte ) '\r' &&
                 data[ readed - 1 ] == ( byte ) '\n' ) {
                readed--;
            }
            // Корректировка максимального значения ProgressBar
            // НЕХ-файл грузится по 512 байт ( шаг = readed >> 9 )
            App.MyWindows.MaxProgress += readed >> 9;
            // Выдача в порт команды на загрузку HEX
            log.Append( App.TaskManager.PortCom.SetCommandLoad( Port ) );
            // Загрузка НЕХ файла  
            var writed = 0;
            do {
                // Инкремент элемента Progress Bar
                App.MyWindows.ValueProgress++;
                // Проверка токена отмены операции
                App.TaskManager.СheckСancelToken();
                // Чтение данных из СОМ порта
                // Если забить на очистку буферов приема/выдачи, то происходят сбои в работе порта
                // Выполнять чтение ОБЯЗАТЕЛЬНО ( либо очистку буферов )
                log.Append( App.TaskManager.PortCom.Read( Port ) );
                // Запись данных в СОМ-порт
                var size_pack = readed - writed >= 512 ? 512 : readed - writed;
                App.TaskManager.PortCom.Write( Port, data, writed, size_pack );
                // Обновление количества записанных байт
                writed += size_pack;
            } while ( writed < readed );
            // Считывание сообщения о успешной загрузки
            log.Append( App.TaskManager.PortCom.GetResultLoad( Port ) );
            // НЕХ файл загружен успешно
            // Запуск работы загруженной программы
            // Выдача в порт команды на запуск HEX
            var str = App.TaskManager.PortCom.SetCommandRun( Port );
            // НЕХ файл загружен и запушен успешно
            // Проверка результатов загрузки НЕХ-файла
            // Проверка результатов загрузки
            CheckResultLoad( str );
            return log.Append( str ).ToString();
        }

        /// <summary>
        /// Подпрограмма проверки результирующих строк запуска НЕХ файла
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private void CheckResultLoad( string str ) {
            if ( string.IsNullOrWhiteSpace( str ) ) return;
            // Анализ данных запуска НЕХ-файла
            // Проверка строки
            // В модуле МВ канал управления приводов отсутствует
            // В модуле МУП канал управления приводов присутствует
            // Строку об успешной загрузке "Инициализация - ok" не отлавливаем
            App.TaskManager.IdEnabledsDictionary[ Port ].Rs232 =
                -1 != str.IndexOf( "rs232 - ok", StringComparison.OrdinalIgnoreCase );
            App.TaskManager.IdEnabledsDictionary[ Port ].Rk =
                -1 != str.IndexOf( "rk    - ok", StringComparison.OrdinalIgnoreCase );
            App.TaskManager.IdEnabledsDictionary[ Port ].Arinc =
                -1 != str.IndexOf( "a429  - ok", StringComparison.OrdinalIgnoreCase );
            App.TaskManager.IdEnabledsDictionary[ Port ].Adc =
                -1 != str.IndexOf( "adc   - ok", StringComparison.OrdinalIgnoreCase );
            App.TaskManager.IdEnabledsDictionary[ Port ].Dac =
                -1 != str.IndexOf( "dac   - ok", StringComparison.OrdinalIgnoreCase );
            // Проверка инициализации базового ПО
            if ( str.IndexOf( "Инициализация базового ПО - ok", StringComparison.OrdinalIgnoreCase ) == -1 ) {
                MessageBox.Show(
                    "Критическая ошибка!!!"                +
                    Environment.NewLine                    +
                    App.TaskManager.NameChanneList[ Port ] +
                    " Обнаружены ошибки в процессе инициализации БПО канала",
                    "Инициализация ПО КТА",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly );
            }
        }
    }
}