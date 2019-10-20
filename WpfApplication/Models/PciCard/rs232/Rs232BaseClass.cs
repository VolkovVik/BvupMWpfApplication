using System;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.ComponentModel;
using System.Collections.Generic;
using WpfApplication.Models.Main;

namespace WpfApplication.Models.PciCard.rs232 {
    /// <inheritdoc />
    /// <summary>
    /// Базовый класс для работы с каналом RS-232
    /// </summary>
    public class Rs232BaseClass : IDisposable {
        /// <summary>
        /// Настройки СОМ порта
        /// </summary>
        protected struct Config {
            public string    PortName;
            public int       BaudRate;
            public Parity    Parity;
            public int       DataBits;
            public StopBits  StopBits;
            public Handshake Handshake;
            public Encoding  Encoding;
            public int       ReadBufferSize;
            public int       WriteBufferSize;
            public int       ReadTimeout;
            public int       WriteTimeout;
        }

        /// <summary>
        /// Массив последовательных портов
        /// </summary>
        private readonly SerialPort _portRs232 = new SerialPort();

        /// <summary>
        /// Свойство инициализации
        /// </summary>
        public bool IsOpenPort {
            get {
                var value = false;
                try {
                    value = _portRs232?.IsOpen ?? false;
                }
                catch ( ArgumentNullException ) { }
                catch ( ArgumentException ) { }
                return value;
            }
        }

        /// <summary>
        /// Минимальное время выдачи одного байта
        /// </summary>
        private double _timeByte = 1.0;

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Rs232BaseClass() {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of readability and maintainability.
            Dispose( false );
        }

        /// <inheritdoc />
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
            // Therefore, you should call GC.SupressFinalize to take this object off 
            // the finalization queue and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Track whether Dispose has been called. 
        /// ReSharper disable once RedundantDefaultMemberInitializer
        /// </remarks>
        private bool _disposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        /// <remarks>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </remarks>
        private void Dispose( bool disposing ) {
            // Check to see if Dispose has already been called.
            if ( _disposed ) return;
            // If disposing equals true, dispose all managed and unmanaged resources.
            if ( disposing ) {
                // Dispose managed resources.
                // Step 4: Close device and release any allocated resource.
                _portRs232.Close();
            }
            // Dispose native ( unmanaged ) resources, if exits
            // Note disposing has been done.
            _disposed = true;
        }

        /// <summary>
        /// Подпрограмма открытия СОМ порта
        /// </summary>
        /// <returns></returns>
        public void Open( string portName, int baudRate = 115200, Parity parity = Parity.None, int dataBits = 8,
            StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None,
            int readBufferSize = 8192, int writeBufferSize = 8192,
            int readTimeout = 500, int writeTimeout = 500
        ) {
            // Получение списка доступных портов rs232 
            var ports = GetNamePort();
            // Проверка наличия портов rs232 
            if ( ports.Capacity == 0 ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(), "The count port is 0" );
            }
            // Проверка наличия заданного порта rs232 
            if ( !ports.Any( portName.Equals ) ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The name port is not found in all" );
            }
            // Проверка порта
            if ( _portRs232.IsOpen ) {
                // порт уже открыт
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(), "The port is already open" );
            }
            // Задание настроек СОМ порту
            SetConfigPort( portName, baudRate, parity, dataBits, stopBits, handshake,
                readBufferSize, writeBufferSize, readTimeout, writeTimeout );
            try {
                // Открытие СОМ порта
                _portRs232.Open();
            }
            catch ( TimeoutException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The time allotted for a process or operation has expired", exc );
            }
            catch ( InvalidOperationException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The specified port on the current instance of the SerialPort is already open", exc );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "One or more of the properties for this instance are invalid", exc );
            }
            catch ( ArgumentNullException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The IsOpen value passed is null", exc );
            }
            catch ( ArgumentException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The port name does not begin with \"COM\"" + Environment.NewLine +
                    "The IsOpen value passed is an empty string (\"\")", exc );
            }
            catch ( IOException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The port is in an invalid state", exc );
            }
            catch ( UnauthorizedAccessException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "Access is denied to the port", exc );
            }
        }

        /// <summary>
        /// Подпрограмма закрытия СОМ порта
        /// </summary>
        /// <returns></returns>
        public void Close() {
            try {
                // Проверка порта
                if ( _portRs232.IsOpen ) {
                    // Закрытие порта
                    _portRs232.Close();
                }
            }
            catch ( ArgumentNullException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The IsOpen value passed is null", exc );
            }
            catch ( ArgumentException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The IsOpen value passed is an empty string (\"\")", exc );
            }
            catch ( IOException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when an I/O error occurs", exc );
            }
        }

        /// <summary>
        /// Подпрограмма очистка буферов СОМ порта
        /// </summary>
        /// <returns></returns>
        protected void Erase() {
            try {
                if ( !_portRs232.IsOpen ) return;
                // Очистка буфера приемника
                _portRs232.DiscardInBuffer();
                // Очистка буфера передатчика
                _portRs232.DiscardOutBuffer();
            }
            catch ( ObjectDisposedException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when an operation is performed on a disposed object",
                    exc );
            }
            catch ( InvalidOperationException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The port is in an invalid state", exc );
            }
            catch ( IOException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The stream is closed", exc );
            }
        }

        /// <summary>
        /// Подпрограмма очистка буферов СОМ порта
        /// </summary>
        /// <returns></returns>
        protected int ByteToRead() {
            var count = 0;

            try {
                if ( _portRs232.IsOpen ) {
                    // количество данных в буфере приемника
                    count = _portRs232.BytesToRead;
                }
            }
            catch ( InvalidOperationException exc ) {
                // The port is in an invalid state.
                // An attempt to set the state of the underlying port failed.
                // For example, the parameters passed from this SerialPort 
                // object were invalid.
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The port is in an invalid state", exc );
            }
            catch ( ArgumentNullException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when a null reference is passed " +
                    "to a method that does not accept it as a valid argument",
                    exc );
            }
            catch ( ArgumentException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when one of the arguments provided to a method is not valid",
                    exc );
            }
            return count;
        }

        /// <summary>
        /// Подпрограмаа чтения из СОМ порта
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="taimwait"></param>
        /// <returns></returns>
        protected int Read( byte[] buffer, int offset, int count, bool taimwait = true ) {
            var readed = 0;
            try {
                if ( _portRs232.IsOpen ) {
                    // Задание времени задержки операции чтения из порта
                    _portRs232.ReadTimeout =
                        taimwait ? ( int ) ( _timeByte * count ) + 100 : SerialPort.InfiniteTimeout;
                    // Чтение
                    readed = _portRs232.Read( buffer, offset, count );
                }
            }
            catch ( ArgumentNullException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when a null reference is passed " +
                    "to a method that does not accept it as a valid argument",
                    exc );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "One or more of the properties for this instance are invalid", exc );
            }
            catch ( ArgumentException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when one of the arguments provided to a method is not valid",
                    exc );
            }
            catch ( InvalidOperationException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    " The port is in an invalid state", exc );
            }
            catch ( TimeoutException ) {
                // No bytes were available to read.
                // 2017-03-17 Волков В.А Это не является ошибкой
            }
            catch ( IOException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when an I/O error occurs", exc );
            }
            return readed;
        }

        /// <summary>
        /// Подпрограмаа записи в СОМ порт
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="taimwait"></param>
        /// <returns></returns>
        public void Write( byte[] buffer, int offset, int count, bool taimwait = true ) {
            if ( buffer == null || buffer.Length == 0 || count == 0 || buffer.Length < offset + count ) return;
            try {
                if ( !_portRs232.IsOpen ) return;
                // Задание времени задержки операции записи в порт
                _portRs232.WriteTimeout =
                    taimwait ? ( int ) ( _timeByte * count ) + 100 : SerialPort.InfiniteTimeout;
                // Запись
                _portRs232.Write( buffer, offset, count );
            }
            catch ( ArgumentNullException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when a null reference is passed" +
                    " to a method that does not accept it as a valid argument",
                    exc );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "One or more of the properties for this instance are invalid", exc );
            }
            catch ( ArgumentException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when one of the arguments provided to a method is not valid",
                    exc );
            }
            catch ( TimeoutException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The operation did not complete before the time-out period ended",
                    exc );
            }
            catch ( InvalidOperationException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The port is in an invalid state", exc );
            }
            catch ( IOException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when an I/O error occurs", exc );
            }
        }

        /// <summary>
        /// Подпрограмаа записи в СОМ порт
        /// </summary>
        /// <param name="str"></param>
        /// <param name="taimwait"></param>
        /// <returns></returns>
        protected void Write( string str, bool taimwait = true ) {
            // Проверка открытия порта
            try {
                if ( !_portRs232.IsOpen ) return;
                // Задание времени задержки операции записи в порт
                _portRs232.WriteTimeout =
                    taimwait ? ( int ) ( _timeByte * str.Length ) + 100 : SerialPort.InfiniteTimeout;
                // Запись
                _portRs232.Write( str );
            }
            catch ( ArgumentNullException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when a null reference is passed" +
                    " to a method that does not accept it as a valid argument",
                    exc );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "One or more of the properties for this instance are invalid", exc );
            }
            catch ( ArgumentException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when one of the arguments provided to a method is not valid",
                    exc );
            }
            catch ( TimeoutException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The operation did not complete before the time-out period ended",
                    exc );
            }
            catch ( InvalidOperationException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The port is in an invalid state", exc );
            }
            catch ( IOException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when an I/O error occurs", exc );
            }
        }

        /// <summary>
        /// Подпрограмма получения списка доступных портов rs232
        /// </summary>
        /// <returns></returns>
        private static List< string > GetNamePort() {
            try {
                return SerialPort.GetPortNames().ToList();
            }
            catch ( Win32Exception exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The serial port names could not be queried", exc );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="handshake"></param>
        /// <param name="readBufferSize"></param>
        /// <param name="writeBufferSize"></param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        private void SetConfigPort( string portName, int baudRate,
            Parity parity, int dataBits, StopBits stopBits, Handshake handshake,
            int readBufferSize, int writeBufferSize, int readTimeout, int writeTimeout ) {
            try {
                // Задание номера порта
                // Устанавливаем верхний регистр
                // The communications port. The default is COM1.
                _portRs232.PortName = portName.ToUpper();
                // Задание скорости работы порта
                // The baud rate must be supported by the user's serial driver. 
                // The default value is 9600 bits per second (bps).
                _portRs232.BaudRate = baudRate;
                // Задание режима работы паритета
                // One of the Parity values that represents the parity-checking protocol. 
                // The default is None. 
                // If a parity error occurs on the trailing byte of a stream, 
                // an extra byte will be added to the input buffer with a value of 126.
                _portRs232.Parity = parity;
                // Задание количества информационных бит в байте
                // The range of values for this property is from 5 through 8. 
                // The default value is 8.
                _portRs232.DataBits = dataBits;
                // Задание количества стоп бит
                // The range of values for this property is from 5 through 8. 
                // The default value is 8.
                _portRs232.StopBits = stopBits;
                // Задание режима работы порта
                // One of the Handshake values. The default is None. 
                // When handshaking is used, the device connected to the SerialPort object 
                // is instructed to stop sending data when there is at least 
                // ( ReadBufferSize-1024) bytes in the buffer. 
                // The device is instructed to start sending data again
                // when there are 1024 or fewer bytes in the buffer. 
                // If the device is sending data in blocks that are larger than 1024 bytes, 
                // this may cause the buffer to overflow. 
                // If the Handshake property is set to RequestToSendXOnXOff and CtsHolding 
                // is set to false,the XOff character will not be sent. 
                // If CtsHolding is then set to true, more data must be sent 
                // before the XOff character will be sent. 
                _portRs232.Handshake = handshake;
                // Задание кодировки
                // An Encoding object. The default is ASCIIEncoding. 
                _portRs232.Encoding = Encoding.Unicode;
                // Задание времени задержки операции чтения из порта
                // The number of milliseconds before a time-out occurs when a read operation does not finish.
                // The read time-out value was originally set at 500 milliseconds in the Win32 Communications API.
                // This property allows you to set this value. 
                // The time-out can be set to any value greater than zero, or set to InfiniteTimeout, 
                // in which case no time-out occurs. 
                // InfiniteTimeout is the default. 
                _portRs232.ReadTimeout = readTimeout;
                // Задание времени задержки операции записи в порт
                // The number of milliseconds before a time-out occurs. The default is InfiniteTimeout. 
                // The write time-out value was originally set at 500 milliseconds in the Win32 Communications API. 
                // This property allows you to set this value. 
                // The time-out can be set to any value greater than zero, or set to InfiniteTimeout, 
                // in which case no time-out occurs.
                // InfiniteTimeout is the default. 
                _portRs232.WriteTimeout = writeTimeout;
                // Задание размера буфера передатчика
                // The size of the output buffer. The default is 2048.
                // The WriteBufferSize property ignores any value smaller than 2048. 
                _portRs232.WriteBufferSize = writeBufferSize;
                // Задание размера буфера приемника
                // The buffer size. The default value is 4096.
                // The ReadBufferSize property ignores any value smaller than 4096. 
                _portRs232.ReadBufferSize = readBufferSize;
                //
                // Установка таймаутов.
                //
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch ( _portRs232.StopBits ) {
                    case StopBits.None: {
                        _timeByte = 1000.0 * ( _portRs232.DataBits + 1 ) / _portRs232.BaudRate;
                        break;
                    }
                    case StopBits.One: {
                        _timeByte = 1000.0 * ( _portRs232.DataBits + 2 ) / _portRs232.BaudRate;
                        break;
                    }
                    case StopBits.Two: {
                        _timeByte = 1000.0 * ( _portRs232.DataBits + 3 ) / _portRs232.BaudRate;
                        break;
                    }
                    case StopBits.OnePointFive: {
                        _timeByte = 1000.0 * ( _portRs232.DataBits + 2.5 ) / _portRs232.BaudRate;
                        break;
                    }
                }
            }
            catch ( ArgumentOutOfRangeException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "One or more of the properties for this instance are invalid", exc );
            }
            catch ( ArgumentNullException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when a null reference is passed " +
                    "to a method that does not accept it as a valid argument", exc );
            }
            catch ( ArgumentException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when one of the arguments " +
                    "provided to a method is not valid", exc );
            }
            catch ( InvalidOperationException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The port is in an invalid state", exc );
            }
            catch ( IOException exc ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs(),
                    "The exception that is thrown when an I/O error occurs", exc );
            }
        }

        /// <summary>
        /// Подпрограмма настройки СОМ порта
        /// </summary>
        /// <returns></returns>
        protected Config GetConfigPort() {
            return new Config {
                PortName        = _portRs232.PortName,
                BaudRate        = _portRs232.BaudRate,
                Parity          = _portRs232.Parity,
                DataBits        = _portRs232.DataBits,
                StopBits        = _portRs232.StopBits,
                Handshake       = _portRs232.Handshake,
                Encoding        = _portRs232.Encoding,
                ReadTimeout     = _portRs232.ReadTimeout,
                WriteTimeout    = _portRs232.WriteTimeout,
                WriteBufferSize = _portRs232.WriteBufferSize,
                ReadBufferSize  = _portRs232.ReadBufferSize
            };
        }
    }
}