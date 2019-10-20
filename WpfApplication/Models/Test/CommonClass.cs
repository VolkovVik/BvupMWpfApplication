using System;
using System.IO;
using iTextSharp.text;
using WpfApplication.Models.Main;
using WpfApplication.Models.Main.XML;

namespace WpfApplication.Models.Test {
    internal static class CommonClass {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string Separator = new string( '-', 80  ) + Environment.NewLine;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="level"></param>
        /// <param name="visible"></param>
        /// <param name="separator1"></param>
        /// <param name="separator2"></param>
        public static void SetText( string str, int level = 0,
            bool visible = true, bool separator1 = false, bool separator2 = false ) {
            App.TaskManager.Log.WriteAsync(
                ( separator1 ? Separator : "" ) + str + Environment.NewLine + ( separator2 ? Separator : "" ) );
#if DEBUG
            if ( visible ) {
                App.MyWindows.Text += str.ToRigthExt( level << 2 ) + Environment.NewLine;
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="level"></param>
        /// <param name="offset"></param>
        /// <param name="result"></param>
        /// <param name="visible"></param>
        /// <param name="debug"></param>
        /// <param name="separator1"></param>
        /// <param name="separator2"></param>
        public static void SetResText( string str, int level, int offset, bool result,
            bool visible = false, bool debug = true,
            bool separator1 = false, bool separator2 = false ) {
            App.TaskManager.Log.WriteAsync( ( separator1 ? Separator : "" )      + str                 +
                                            ( result ? " - норма" : " - отказ" ) + Environment.NewLine +
                                            ( separator2 ? Separator : "" ) );
            // Выравнивание результата
            var aligned_str = str.ToRigthExt( level << 2 ).ToLengthExt( result ? " - норма" : " - отказ", offset );
            if ( visible ) {
                App.MyWindows.TextLine += aligned_str;
            }
#if DEBUG
            if ( debug && !visible ) {
                App.MyWindows.TextLine += aligned_str;
            }
#endif
        }

        /// <summary>
        /// Подпрограмма определения типа устройства канал/модель
        /// </summary>
        /// <param name="task"></param>
        /// <param name="channel"></param>
        public static int? SetTypeChannel( int task, int? channel ) {
            var dict = App.TaskManager.ConfigProgram.DictCfgTest;
            // Выдача пользователю идентификатора устройства
            if ( dict[ task ].Type       != ConfigTestsClass.TypeTask.Test ||
                 dict[ task ].IndexTabL1 == null                           ||
                 dict[ task ].IndexTabL1 == channel ) {
                return dict[ task ].IndexTabL1;
            }
            var str =
                dict[ task ].IndexTabL1 == null ? "«Устройство незадано»" :
                dict[ task ].IndexTabL1 == 0    ? "«Канал»" :
                dict[ task ].IndexTabL1 == 1    ? "«Модель»" : "«Устройство неопределено»";
            // Вывод данных в лог-файл
            App.TaskManager.Log.WriteLineAsync( str );
            // Вывод данных в протокол
            App.TaskManager.Report.WriteLineAsync( str );
            // Вывод пользователю
            App.MyWindows.TextLine += str;
            return dict[ task ].IndexTabL1;
        }

        /// <summary>
        /// Подпрограмма создания заголовка .log-файла
        /// </summary>
        /// <param name="time"></param>
        /// <param name="signFullTest"></param>
        public static void SetHeaderLog( DateTime time, bool signFullTest ) {
            // Вывод заголовка
            var str = $"Оператор:             {App.TaskManager.ConfigProgram.UserOperator}" + Environment.NewLine +
                      $"Представитель ОТК:    {App.TaskManager.ConfigProgram.UserOtk}"      + Environment.NewLine +
                      $"Представитель ВП МО:  {App.TaskManager.ConfigProgram.UserVp}"       + Environment.NewLine +
                      $"Температура:          {App.TaskManager.ConfigProgram.Temp:F2} °C"   + Environment.NewLine +
                      Environment.NewLine                                                   +
                      ( $"Контроль {App.TaskManager.ConfigProgram.NameDevice} № " +
                        ( string.Equals( "00000000", App.TaskManager.ConfigProgram.Nomer )
                            ? $"{App.TaskManager.ConfigProgram.Nomer}"
                            : "_______________" ) )
                      .ToLengthExt( time.ToString( "dd MMMM yyyy HH:mm:ss.fffffff" ), 75 ) +
                      Environment.NewLine;
            if ( signFullTest ) {
                // Запуск полного теста
                App.MyWindows.TextLine += $"Контроль {App.TaskManager.ConfigProgram.NameDevice}" +
                                          Environment.NewLine;
            } else {
                // Запуск отдельной задачи
                str += "Запуск отдельной задачи" + Environment.NewLine;
            }
            App.TaskManager.Log.WriteLineAsync( str );
            App.TaskManager.Report.WriteLineAsync( str );
        }

        /// <summary>
        /// Подпрограмма записи результата теста в .log-файл
        /// </summary>
        /// <param name="time"></param>
        public static void SetResultLog( DateTime time ) {
            // Определение времени выполнения теста
            var str = Environment.NewLine +
                      $"Контроль {App.TaskManager.ConfigProgram.NameDevice} завершен".ToLengthExt(
                          DateTime.Now.ToString( "dd MMMM yyyy HH:mm:ss.fffffff" ), 75 )             +
                      Environment.NewLine                                                            +
                      "Время выполнения:".ToLengthExt( ( DateTime.Now - time ).ToString( "g" ), 75 ) +
                      Environment.NewLine;
            App.TaskManager.Log.WriteAsync( str );
#if DEBUG
            App.MyWindows.Text +=
                Environment.NewLine                                                               +
                "**************************** Отладочная информация ****************************" +
                str;
#endif
        }

        /// <summary>
        /// Подпрограмма выдачи результата проведенного теста пользователю
        /// </summary>
        /// <param name="countErr"></param>
        /// <param name="signFullTest"></param>
        public static void SetResultText( int countErr, bool signFullTest ) {
            // Проверка результата теста
            string str;
            var    str_report = string.Empty;
            switch ( countErr ) {
                case -1: {
                    // Тестирование прервано пользователем
                    str = $"Контроль {App.TaskManager.ConfigProgram.NameDevice} прерван пользователем.";
                    break;
                }
                case 0: {
                    // Тестирование завершено успешно
                    str_report = $"Контроль {App.TaskManager.ConfigProgram.NameDevice} завершен успешно.";
                    str = signFullTest
                        ? "Изделие соответствует ТУ" + Environment.NewLine + str_report
                        : "Контроль завершен успешно.";
                    break;
                }
                default: {
                    // Тестирование завершено с ошибками
                    str_report = $"Контроль {App.TaskManager.ConfigProgram.NameDevice} завершена с ошибками!!!";
                    str = signFullTest
                        ? "Изделие НЕ соответствует ТУ." + Environment.NewLine + str_report
                        : "Контроль завершен с ошибками.";
                    break;
                }
            }
            // Вывод пользователю
            App.MyWindows.TextLine += str;
            // Вывод данных в лог-файл
            App.TaskManager.Log.WriteLineAsync( str );
            // Вывод данных в протокол
            App.TaskManager.Report.WriteLineAsync( Environment.NewLine + str_report );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signFullTest"></param>
        /// <param name="error"></param>
        public static void SaveLogFiles( bool signFullTest, int error ) {
            // Проверка необходимости сохранеия лог-файлов 
            // в отдельной директории
            if ( !signFullTest ||
                 string.IsNullOrWhiteSpace( App.TaskManager.ConfigProgram.UserOtk ) &&
                 string.IsNullOrWhiteSpace( App.TaskManager.ConfigProgram.UserVp ) ) return;
            // Задание режима работы ProgressBar
            App.MyWindows.IndeterminateProgress = true;
            // Cчитывания пути доступа к директории хранения тестов ПСИ
            var xml  = new XmlClass();
            var path = xml.Read( XmlClass.NameLogFiles, "directory" );
            new FolderClass( path, App.MyWindows.ShowFormErrorCommand.Execute ).Create();
            // Название файла
            var file_name =
                ( string.IsNullOrWhiteSpace( path ) ? "" : $"{path}\\" ) +
                $"{App.TaskManager.ConfigProgram.NameDevice}_"           +
                $"{App.TaskManager.ConfigProgram.Nomer}_"                +
                DateTime.Now.ToString( "yyyyMMddHHmmss" );
            // Создание файла протокола
            new FilesClass( $"{file_name}.report", App.MyWindows.ShowFormErrorCommand.Execute ).WriteAsync(
                App.TaskManager.Report.Read() );
            // Чтение лог файла
            var log_string = App.TaskManager.Log.Read();
            // Создание копии лог-файла
            new FilesClass( $"{file_name}.log", App.MyWindows.ShowFormErrorCommand.Execute ).WriteAsync( log_string );
            // Создание pdf-файла
            CreatePdfFile( $"{file_name}.pdf", log_string, error );
            // Сброс режима работы ProgressBar
            App.MyWindows.IndeterminateProgress = false;
        }

        /// <summary>
        /// Подпрограмма создания файла отчета в PDF
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="countErr"></param>
        private static void CreatePdfFile( string fileName, string text, int countErr ) {
            if ( string.IsNullOrWhiteSpace( text ) ||
                 new XmlClass().Read( XmlClass.NameLogFiles, "enable" ) == "NOT" ) return;
            App.MyWindows.TextLine += "Запущен процесс создания файлов отчетов";
            // Задание цвета фона
            var back_color = 0 == countErr ? new BaseColor( 0, 255, 0, 32 ) : new BaseColor( 255, 0, 0, 32 );
            try {
                // Создание файла .pdf
                new PdfFileClass( fileName ).Write( text, back_color );
                App.MyWindows.TextLine += $"Процесс создания файла {fileName} завершен";
            }
            catch ( ArgumentNullException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, $"Ошибка создания файла {fileName}" );
            }
            catch ( ArgumentException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, $"Ошибка создания файла {fileName}" );
            }
            catch ( FileLoadException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, $"Ошибка создания файла {fileName}" );
            }
            catch ( FileNotFoundException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, $"Ошибка создания файла {fileName}" );
            }
        }
    }
}