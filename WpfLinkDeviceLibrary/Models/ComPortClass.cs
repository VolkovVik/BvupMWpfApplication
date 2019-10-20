using System;
using System.IO;
using System.IO.Ports;

namespace BvupMLinkLibrary.Models {
    /// <summary>
    /// 
    /// </summary>
    internal class ComPortClass {
        /// <summary>
        /// Сом-порт
        /// </summary>
        private SerialPort PortRs232 { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="port"></param>
        public ComPortClass( SerialPort port ) {
            PortRs232 = port;
        }

        /// <summary>
        /// Подпрограмаа чтения из СОМ порта
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read( byte[] buffer, int offset, int count ) {
            var readed = 0;
            try {
                if ( PortRs232.IsOpen ) {
                    // Задание времени задержки операции чтения из порта
                    PortRs232.ReadTimeout = 10 * count + 1;
                    // Чтение данных из порта
                    readed = PortRs232.Read( buffer, offset, count );
                }
            }
            catch ( NullReferenceException ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Ошибка доступа к объекту." );
            }
            catch ( IOException ) {
                // The port is in an invalid state. 
                // An attempt to set the state of the underlying port failed. 
                // For example, the parameters passed from this SerialPort object were invalid. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Порт находится в недопустимом состоянии." );
            }
            catch ( ArgumentNullException ) {
                //  The buffer passed is null. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Буфер для записи задан null." );
            }
            catch ( InvalidOperationException ) {
                // The specified port is not open. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Указанный порт не открыт." );
            }
            catch ( ArgumentOutOfRangeException ) {
                // The offset or count parameters are outside a 
                // valid region of the buffer being passed. 
                // Either offset or count is less than zero. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Значение параметра выходит за допустимые пределы." );
            }
            catch ( ArgumentException ) {
                // offset plus count is greater than the length of the buffer. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "offset а также count больше, чем длина buffer." );
            }
            catch ( TimeoutException ) {
                // No bytes were available to read.
                // 2017-03-17 Волков В.А Это не является ошибкой
            }
            return readed;
        }

        /// <summary>
        /// Подпрограмаа записи в СОМ порт
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public void Write( byte[] buffer, int offset, int count ) {
            try {
                if ( !PortRs232.IsOpen ) return;
                // Задание времени задержки операции записи в порт
                PortRs232.WriteTimeout = 10 * count + 1;
                // Запись данных в порт
                PortRs232.Write( buffer, offset, count );
            }
            catch ( NullReferenceException ) {
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Ошибка доступа к объекту." );
            }
            catch ( IOException ) {
                // The port is in an invalid state. 
                // An attempt to set the state of the underlying
                // port failed. For example, the parameters 
                // passed from this SerialPort object were invalid. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Порт находится в недопустимом состоянии." );
            }
            catch ( ArgumentNullException ) {
                //  The buffer passed is null. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Буфер для записи задан null." );
            }
            catch ( InvalidOperationException ) {
                // The specified port is not open. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Указанный порт не открыт." );
            }
            catch ( ArgumentOutOfRangeException ) {
                // The offset or count parameters are outside a 
                // valid region of the buffer being passed. 
                // Either offset or count is less than zero. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Значение параметра выходит за допустимые пределы." );
            }
            catch ( ArgumentException ) {
                // offset plus count is greater than the length of the buffer. 
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "offset а также count больше, чем длина buffer." );
            }
            catch ( TimeoutException ) {
                // No bytes were available to read.
                throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                    "Операция не завершилась до истечения срока ожидания." );
            }
        }

        /// <summary>
        /// Подпрограмма очистка буферов СОМ порта
        /// </summary>
        /// <returns></returns>
        public void Erase() {
            try {
                // Очистка буфера приемника
                PortRs232.DiscardInBuffer();
                // Очистка буфера передатчика
                PortRs232.DiscardOutBuffer();
            }
            catch ( ObjectDisposedException ) {
                // The exception that is thrown when an operation is performed 
                // on a disposed object. 
                //throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                //    "Объект уничтожен." );
            }
            catch ( InvalidOperationException ) {
                // The port is in an invalid state.
                // An attempt to set the state of the underlying port failed.
                // For example, the parameters passed from this SerialPort 
                // object were invalid.
                //throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                //    "Поток закрыт." );
            }
            catch ( IOException ) {
                // The stream is closed. This can occur because the Open method
                // has not been called or the Close method has been called. 
                //throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                //    "Порт находится в недопустимом состоянии." );
            }
            catch ( NullReferenceException ) {
                //throw new Exception< Rs232ExceptionArgs >( new Rs232ExceptionArgs( PortRs232?.PortName ),
                //    "Ошибка доступа к объекту." );
            }
        }
    }
}