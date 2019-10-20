using System.Collections.Generic;
using System.IO.Ports;
using WpfApplication.Models.Main;
using WpfApplication.Models.Main.XML;
using WpfApplication.Models.Test;

namespace WpfApplication.Models.PciCard.rs232 {
    /// <summary>
    /// Класс, определяющий функции для работы с имитатором
    /// </summary>
    public class PortRs232 {
        /// <summary>
        /// Имя интерфейсной платы
        /// </summary>
        //private string Name { get; }

        /// <summary>
        /// Признак инициализации интерфейсной платы
        /// </summary>
        public bool IsInit { get; private set; }

        /// <summary>
        /// Максимальное количество интерфейсных плат
        /// </summary>
        public const int MaxDevice = 1;

        /// <summary>
        /// Массив объектов плат порт RS232
        /// </summary>
        private readonly Rs232ProtocolClass[] _device;

        /// <inheritdoc />
        /// <summary>
        /// Конструктор
        /// </summary>
        // ReSharper disable once UnusedParameter.Local
        public PortRs232( string name ) {
            // Сброс признака инициализации
            IsInit = false;
            // Имя интерфейсной платы
            //Name = name;
            // Инициализация массива устройств
            _device = new Rs232ProtocolClass[ MaxDevice ];
            for ( var i = 0; i < MaxDevice; i++ ) {
                _device[ i ] = new Rs232ProtocolClass( i );
                _device[ i ].SetTypeCpu( Rs232ProtocolClass.TypeCpu.LittleEndian );
            }
            //Open();
        }

        ~PortRs232() {
            Close();
        }

        /// <summary>
        /// Подпрограмма инициализации платы PCI1753 драйвером DAQ
        /// </summary>
        /// <returns></returns>
        public void Open() {
            // Установка признака инициализации
            IsInit = true;
            var xml = new XmlClass();
            // Инициализация плат
            for ( var i = 0; i < MaxDevice; i++ ) {
                var result = false;
                // Сдвиг шкалы ProgressBar
                App.MyWindows.ValueProgress++;
                // Считывание имени технологического порта rs232 из .xml файла
                var port_name = xml.Read( XmlClass.NameRs232, XmlClass.ElementPort + i ).ToUpper();
                if ( !string.IsNullOrWhiteSpace( port_name ) ) {
                    try {
                        // Открытие порта
                        _device[ i ].Open( port_name );
                        result = true;
                    }
                    catch ( Exception< Rs232ExceptionArgs > exc ) {
                        // Сброс признака инициализации
                        App.TaskManager.Log.WriteLineAsync( exc.Message );
                    }
                }
                if ( !result ) {
                    // Сброс признака инициализации модуля
                    IsInit = false;
                }
                // Возврат результата
                var description = string.IsNullOrWhiteSpace( port_name )
                    ? "RS-232,NAME#COM0"
                    : $"RS-232,NAME#{port_name}";
                CommonClass.SetResText( $"Инициализация платы {description}", 1, 75, result, true );
            }
        }

        /// <summary>
        /// Подпрограмма закрытия портов
        /// </summary>
        /// <returns></returns>
        public void Close() {
            // Сброс признака инициализации 
            IsInit = false;
            // Закрытие плат
            // Исправлено 2016-12-26 Была ошибка закрытия
            for ( var index = 0; index < MaxDevice; index++ ) {
                try {
                    _device[ index ]?.Close();
                }
                catch ( Exception< Rs232ExceptionArgs > exc ) {
                    App.MyWindows.ShowFormErrorCommand.Execute( exc, "Port RS232 is closed with error" );
                }
            }
        }

        /// <summary>
        /// Подпрограмма проверки допустимости индекса порта
        /// </summary>
        /// <returns></returns>
        private void Check( int device ) {
            if ( !IsInit ) {
                // Модуль не инициализирован 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( device ),
                    $"The modul of port RS232 #{device} is not initialized" );
            }
            if ( 0 > device || MaxDevice <= device ) {
                // Индекс неверный
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( device ),
                    $"The port index of port RS232 #{device} is not valid" );
            }
            // Проверка существования объетка
            if ( _device[ device ] == null ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( device ),
                    $"The object of port RS232 #{device} is not creatе" );
            } // Проверка инициализации объекта платы
            if ( !_device[ device ].IsOpenPort ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( device ),
                    $"The object of port RS232 #{device} is not initialized" );
            }
        }

        /// <summary>
        ///  Подпрограмма изменения названия порта
        /// </summary>
        /// <param name="device"></param>
        /// <param name="hs"></param>
        /// <returns></returns>
        public void SetHandshake( int device, Handshake hs = Handshake.RequestToSend ) {
            Check( device );
            _device[ device ].SetHandshake( hs );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public string Read( int device ) {
            Check( device );
            return _device[ device ].ReadData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write( int device, byte[] data, int offset, int count ) {
            Check( device );
            _device[ device ].Write( data, offset, count );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public int Send( int device, IList< string > command,
            byte[] data = null, int offset = 0, int count = 0, int time = 100 ) {
            Check( device );
            return _device[ device ].SetData( command, data, offset, count, time );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetData( int device, IList< string > command, ushort[] data, int offset, int count, int time ) {
            Check( device );
            return _device[ device ].GetData( command, data, offset, count, time );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetData( int device, IList< string > command, uint[] data, int offset, int count, int time ) {
            Check( device );
            return _device[ device ].GetData( command, data, offset, count, time );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public string GetCheckReady( int device ) => _device[ device ].GetCheckReady();

        /// <summary>
        /// Подпрограмма выдачи команды Version
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public string SetCommandVersion( int device ) =>_device[ device ].SetCommandVersion();

        /// <summary>
        /// Подпрограмма выдачи команды Load
        /// </summary>
        /// <param name="device"></param>
        public string SetCommandLoad( int device ) => _device[ device ].SetCommandLoad();

        /// <summary>
        /// Подпрограмма получения результата загрузки НЕХ файла
        /// </summary>
        /// <param name="device"></param>
        public string GetResultLoad( int device ) => _device[ device ].GetResultLoad();

        /// <summary>
        /// Подпрограмма выдачи команды Run
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public string SetCommandRun( int device ) => _device[ device ].SetCommandRun();
    }
}