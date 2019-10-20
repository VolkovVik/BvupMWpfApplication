using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using WpfApplication.Models.Main;

namespace WpfApplication.Models.PciCard.rs232 {
    /// <inheritdoc />
    /// <summary>
    /// Класс протокола обмена по портам RS232
    /// </summary>
    internal class Rs232ProtocolClass : Rs232BaseClass {
        /// <summary>
        /// Порядок следования байт
        /// </summary>
        public enum TypeCpu {
            BigEndian,
            LittleEndian
        }

        /// <summary>
        /// Массив типов
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public TypeCpu TypeEndianCpu { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public int Port { get; private set; }

        /// <summary>
        /// Протоколирование загрузки НЕХ-файла
        /// </summary>
        public string Log { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        public Rs232ProtocolClass( int port ) {
            Port = port;
        }

#region Команды управления устройством

        /// <summary>
        /// Строка обозначающая начало выдачи данных
        /// </summary>
        private const byte AnswerStartData = ( byte ) 'i';

        /// <summary>
        /// Строка обозначающая окончание выдачи данных
        /// </summary>
        private const byte AnswerStopData = ( byte ) '#';

        /// <summary> 
        /// Пустая команда
        /// </summary>
        private const string AnswerEnter = "\r\n>";

        /// <summary>
        /// Тест завершен успешно
        /// </summary>
        private const string AnswerNorm = AnswerEnter + "ok" + AnswerEnter;

        /// <summary>
        /// Тест завершен успешно
        /// </summary>
        private const string AnswerFail = AnswerEnter + "error" + AnswerEnter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public int SetData( IList< string > command, byte[] data, int offset, int count, int time ) {
            int count_err;
            if ( command == null || command.Count == 0 ) return 0;
            try {
                // Запись массива команд в порт
                WriteCommand( command );
                // Выдача данных байтового массива
                Write( data, offset, count );
                // Проверка подтверждения приема команды
                count_err = CheckSetData( command, time );
            }
            catch ( Exception< Rs232ExceptionArgs > ) {
                App.TaskManager.Log.WriteLineAsync( App.TaskManager.NameChanneList[ Port ] +
                                                    $"ошибка выполнения команды {command[ 0 ]}" );
                throw;
            }
            return count_err;
        }

        /// <summary>
        /// Подпрограмма получения результата выпоолнения команды
        /// </summary>
        /// <param name="command"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private int CheckSetData( IList< string > command, int time = 100 ) {
            var str        = string.Empty;
            var start_time = DateTime.Now;
            do {
                // Получение данных
                var str_step = ReadData();
                if ( !string.IsNullOrWhiteSpace( str_step ) ) {
                    // Данные получены
                    str += str_step;
                } else {
                    // Данных в буфере нет
                    // Возможно выдача данных завершена
                    // Время проверить полученный результат
                    if ( string.IsNullOrWhiteSpace( str ) ) continue;
                    //
                    // Сравнение полученной строки с ответом - "\r\n>ok\r\n>"
                    //
                    var start_index = str.Length - AnswerNorm.Length - 1;
                    if ( start_index < 0 || start_index >= str.Length ) continue;
                    // Поиск заданной строки
                    if ( str.IndexOf( AnswerNorm, start_index, StringComparison.OrdinalIgnoreCase ) != -1 ) {
                        return 0;
                    }
                    //
                    // Сравнение полученной строки с ответом - "\r\n>error\r\n>"
                    //
                    start_index = str.Length - AnswerFail.Length - 1;
                    if ( start_index < 0 || start_index >= str.Length ) continue;
                    // ReSharper disable once InvertIf
                    if ( str.IndexOf( AnswerFail, start_index, StringComparison.OrdinalIgnoreCase ) != -1 ) {
                        // Произошла ошибка внутри устройства
                        App.TaskManager.Log.WriteLineAsync( $"Команда {command[ 0 ]} выполнена с ошибкой" +
                                                            $"{Environment.NewLine}\tanswer :{str}" );
                        return 1;
                    }
                }
            } while ( time > ( int ) ( DateTime.Now - start_time ).TotalMilliseconds );
            throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( Port ),
                $"Время ожидания ответа на команду {command[ 0 ]} истекло" +
                $"{Environment.NewLine}\tanswer :{str}" );
        }

#endregion Команды управления устройством

#region Команды для получения данных из устройства

        /// <summary>
        /// Подпрограмма запроса данных из устройства
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetData( IList< string > command, byte[] data, int offset, int count, int time ) {
            if ( command     == null || command.Count == 0 ||
                 data        == null || data.Length   == 0 ||
                 offset      < 0     || count         == 0 ||
                 data.Length < offset + count ) return 0;
            try {
                // Получение данных
                var list = new List< byte >();
                // ReSharper disable once UnusedVariable
                var readed = GetData( command, list, count, time );
                // Преобразование в байтовый массив
                return list.ToArrayExt( data, offset, count );
            }
            catch ( Exception< Rs232ExceptionArgs > ) {
                App.TaskManager.Log.WriteLineAsync( App.TaskManager.NameChanneList[ Port ] +
                                                    $"ошибка выполнения команды {command[ 0 ]}" );
                throw;
            }
        }

        /// <summary>
        /// Подпрограмма запроса данных из устройства
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetData( IList< string > command, ushort[] data, int offset, int count, int time ) {
            if ( command     == null || command.Count == 0 ||
                 data        == null || data.Length   == 0 ||
                 offset      < 0     || count         == 0 ||
                 data.Length < offset + count ) return 0;
            try {
                // Получение данных
                var list = new List< byte >();
                // ReSharper disable once UnusedVariable
                var readed = GetData( command, list, count, time );
                // Преобразование в байтовый массив
                return list.ToArrayExt( data, offset, count, TypeEndianCpu );
            }
            catch ( Exception< Rs232ExceptionArgs > ) {
                App.TaskManager.Log.WriteLineAsync( App.TaskManager.NameChanneList[ Port ] +
                                                    $"ошибка выполнения команды {command[ 0 ]}" );
                throw;
            }
        }

        /// <summary>
        /// Подпрограмма GET- запроса к устройству ( 32-ух разрядные беззнаковые данные )
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetData( IList< string > command, uint[] data, int offset, int count, int time ) {
            if ( command     == null || command.Count == 0 ||
                 data        == null || data.Length   == 0 ||
                 offset      < 0     || count         == 0 ||
                 data.Length < offset + count ) return 0;
            try {
                // Получение данных
                var list = new List< byte >();
                // ReSharper disable once UnusedVariable
                var readed = GetData( command, list, count, time );
                // Преобразование в байтовый массив
                return list.ToArrayExt( data, offset, count, TypeEndianCpu );
            }
            catch ( Exception< Rs232ExceptionArgs > ) {
                App.TaskManager.Log.WriteLineAsync( App.TaskManager.NameChanneList[ Port ] +
                                                    $"ошибка выполнения команды {command[ 0 ]}" );
                throw;
            }
        }

        /// <summary>
        /// Подпрограмма запроса данных у устройства
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private int GetData( IList< string > command, IList< byte > data, int count, int time ) {
            // Запись массива команд в порт
            WriteCommand( command );
            // Инициализация переменных
            data.Clear();
            var index_begin_pack = -1;
            var index_end_pack   = -1;
            // Цикл чтения данных
            var start_time = DateTime.Now;
            do {
                // Получение количества данных в буфере порта
                var length = ByteToRead();
                if ( length <= 0 ) continue;
                // Чтение данных
                var bytes       = new byte[ length ];
                var readed_step = Read( bytes, 0, length );
                // Копирование данных
                for ( var i = 0; i < readed_step; i++ ) {
                    // Проверка наличия символа начала пакета данных
                    // символ i - начало информационной выдачи
                    if ( index_begin_pack == -1 ) {
                        // Символ начала информационного пакета не найден
                        if ( data[ i ] == AnswerStartData ) {
                            index_begin_pack = i;
                        }
                        continue;
                    }
                    // Символ начала информационного пакета найден
                    // Проверка наличия символа конца пакета данных
                    // символ  # - конец информационной выдачи
                    // ReSharper disable once InvertIf
                    if ( index_end_pack == -1 ) {
                        // Символ начала информационного пакета не найден
                        if ( data[ i ] == AnswerStopData ) {
                            index_end_pack = i;
                            break;
                        }
                        data.Add( bytes[ i ] );
                    }
                }
                // Проверка найдены ли символы начала и конца информационного пакета
                if ( index_begin_pack == -1 || index_end_pack == -1 ) continue;
                // Проверка количества полученных данных
                if ( count != data.Count ) {
                    throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( Port ),
                        $"Количество считанных байт {data.Count} отличается от ожидаемого значения {count}" );
                }
                return data.Count;
            } while ( time > ( int ) ( DateTime.Now - start_time ).TotalMilliseconds );
            throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( Port ),
                $"Время ожидания ответа на команду {command[ 0 ]} истекло" );
        }

#endregion Команды для получения данных из устройства

#region Команды загрузки НЕХ-файла в устройство

        /// <summary>
        /// Сообщение о начале загрузки 
        /// </summary>
        private const string StrReady = "Monitor ready";

        /// <summary>
        /// Сообщение о начале загрузки 
        /// </summary>
        private const string StrLoad = "Загрузкa HEX...";

        /// <summary>
        /// Сообение об успешной загрузке файла
        /// </summary>
        private const string StrLoadOk = "HEX-LOAD OK";

        /// <summary>
        /// Сообщение о запуске рабочей программы
        /// </summary>
        private const string StrRunOk = "ОС МПМ приветсвует Вас!\r\n>";

        /// <summary>
        /// Команда проверки контрольной суммы
        /// </summary>
        private const byte CommandVersion = ( byte ) 'V';

        /// <summary>
        /// Команда загрузки файла
        /// </summary>
        private const byte CommandLoad = ( byte ) 'L';

        /// <summary>
        /// Команда запуска файла
        /// </summary>
        private const byte CommandRun = ( byte ) 'R';

        /// <summary>
        /// Подпрограмма выдачи команды
        /// </summary>
        /// <param name="command"></param>
        /// <param name="log"></param>
        /// <param name="cmd"></param>
        /// <param name="answer"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private string SetHexCommand( string command, byte cmd = 0, string answer = "", int time = 100 ) {
            var str = string.Empty;

            // Выдача команды
            Write( new[] { cmd }, 0, 1 );
            // Начало отсчета времени выполнения команды
            var start_time = DateTime.Now;
            do {
                // Получение данных
                var str_step = ReadData();
                if ( !string.IsNullOrWhiteSpace( str_step ) ) {
                    // Данные получены
                    str += str_step;
                    continue;
                }
                // Данных в буфере нет
                // Возможно выдача данных завершена
                // Время проверить полученный результат
                if ( string.IsNullOrWhiteSpace( str ) ) continue;
                // Существует два варианта развития событий
                // 1. answer == null 
                // нам нужно получить неизвестное количество данных
                // для дальнейщего анализа гдето в другой части программы
                // При отсутствии данных более 100 мс производится выход из цикла
                // 2. answer == определенное значение 
                // нам нужно получить строку из словаря     
                if ( ( !string.IsNullOrWhiteSpace( answer ) || ByteToRead( 100 ) != 0 ) &&
                     ( string.IsNullOrWhiteSpace( answer ) ||
                       str.IndexOf( answer, StringComparison.OrdinalIgnoreCase ) == -1 ) ) continue;
                // Выдача данных по порту завершена
                // Удаление символа "конец строки"
                return str.Replace( "\0", "" );
                ;
            } while ( time > ( int ) ( DateTime.Now - start_time ).TotalMilliseconds );
            throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( Port ),
                $"Время ожидания ответа на команду {command} истекло" +
                $"{Environment.NewLine}\tanswer :{str}" );
        }

        /// <summary>
        /// Подпрограмма получения строки готовности
        /// </summary>
        /// <returns></returns>
        public string GetCheckReady() {
            return SetHexCommand( "получение строки готовности", answer: StrReady );
        }

        /// <summary>
        /// Подпрограмма выдачи команды Run
        /// </summary>
        /// <returns></returns>
        public string SetCommandVersion() {
            return SetHexCommand( "команда V", CommandVersion );
        }

        /// <summary>
        /// Подпрограмма выдачи команды Load
        /// </summary>
        /// <returns></returns>
        public string SetCommandLoad() {
            return SetHexCommand( "команда L", CommandLoad, StrLoad, 250 );
        }

        /// <summary>
        /// Подпрограмма получения результата загрузки НЕХ файла
        /// </summary>
        /// <returns></returns>
        public string GetResultLoad() {
            return SetHexCommand( "результат загрузки НЕХ-файла", answer: StrLoadOk );
        }

        /// <summary>
        /// Подпрограмма выдачи команды Run
        /// </summary>
        /// <returns></returns>
        public string SetCommandRun() {
            return SetHexCommand( "команда R", CommandRun, StrRunOk, 2500 );
        }

#endregion Команды загрузки НЕХ-файла в устройство

#region Вспомогательные подпрограммы

        /// <summary>
        /// Подпрограмма ожидания получения данных
        /// </summary>
        /// <param name="time"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private int ByteToRead( int time, int count = 1 ) {
            int readed;
            var start_time = DateTime.Now;
            do {
                // Получение количества данных в буфере приема
                readed = ByteToRead();
            } while ( count > readed && ( int ) ( DateTime.Now - start_time ).TotalMilliseconds < time );
            return readed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadData() {
            // Получение количества данных в буфере приемника
            var readed = ByteToRead();
            if ( readed == 0 ) return string.Empty;
            // Считывание данных из буфера
            var data = new byte[ readed ];
            readed = Read( data, 0, data.Length, false );
            // Сохранение
            return data.ToStringExt( 0, readed );
        }

        /// <summary>
        /// Подпрограмма выдачи команд
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private void WriteCommand( ICollection< string > command ) {
            // При отсутствии команд нехрен чтолибо делать
            // Да здравствует лень и распиздяйство
            if ( command == null || command.Count == 0 ) return;
            // Очистка буфера порта
            Erase();
            // Цикл выдачи команды с параметрами
            foreach ( var cmd in command ) {
                Write( cmd + "\r" );
                // Между выдачей команд должна быть пауза на их обработку
                // Иначе возможна потеря данных
                Thread.Sleep( 5 );
            }
        }

        /// <summary>
        ///  Подпрограмма изменения названия порта
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public void SetName( string portName ) {
            // Закрытие порта
            Close();
            // Открытие порта
            Open( portName.ToUpper() );
        }

        /// <summary>
        ///  Подпрограмма изменения названия порта
        /// </summary>
        /// <param name="hs"></param>
        /// <returns></returns>
        public void SetHandshake( Handshake hs = Handshake.RequestToSend ) {
            // Получение настроек
            var settings = GetConfigPort();
            // Закрытие порта
            Close();
            // Открытие порта
            Open( settings.PortName, handshake: hs );
        }

        /// <summary>
        /// Подпрограмма задания типа камня
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void SetTypeCpu( TypeCpu type ) {
            TypeEndianCpu = type;
        }

#endregion Вспомогательные подпрограммы
    }
}