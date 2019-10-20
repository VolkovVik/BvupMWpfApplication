using System;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;

namespace WpfApplication.Models.Main {
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    public class FilesClass : IDisposable {
        /// <summary>
        /// Имя файла
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string FileName { get; private set; }

        /// <summary>
        /// Проверка существования xml-файла
        /// </summary>
        private bool Exist => File.Exists( FileName );

        /// <summary>
        /// Семафор
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim( 1, 1 );

        /// <summary>
        /// Подпрограмма выдачи сообщения об ошибке
        /// </summary>
        private readonly Action< Exception, string > _showErrorMessage;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="action"></param>
        public FilesClass( string fileName, Action< Exception, string > action = null ) {
            _showErrorMessage = action;
            try {
                FileName = string.IsNullOrWhiteSpace( fileName ) ? "default.txt" : fileName;
                if ( !Path.IsPathRooted( FileName ) ) {
                    FileName = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, FileName );
                }
                // Создание каталога
                new FolderClass( FileName, action ).Create();
            }
            catch ( ArgumentNullException exc ) {
                FileName = string.Empty;
                _showErrorMessage?.Invoke( exc, $"Недопустимый путь к файлу {fileName}" );
            }
            catch ( ArgumentException exc ) {
                FileName = string.Empty;
                _showErrorMessage?.Invoke( exc, $"Недопустимый путь к файлу {fileName}" );
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~FilesClass() {
            Dispose( false );
        }

        /// <inheritdoc />
        /// <summary>
        /// Подпрограмма выполняющая определяемые приложением задачи, 
        /// связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose() {
            Dispose( true );
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
        private void Dispose( bool disposing ) {
            // Check to see if Dispose has already been called.
            if ( _disposed ) return;
            // If disposing equals true, dispose all managed and unmanaged resources.
            if ( disposing ) {
                // Dispose managed resources.
                _semaphore.Dispose();
            }
            // Dispose native ( unmanaged ) resources, if exits
            // Note disposing has been done.
            _disposed = true;
        }

        /// <summary>
        /// Подпрограмма чтения файла
        /// </summary>
        public string Read() {
            var str = string.Empty;

            _semaphore.Wait();
            try {
                if ( !Exist ) return str;
                using ( var rstream = new StreamReader( FileName, Encoding.Default ) ) {
                    // Чтение всех данных из файла
                    str = rstream.ReadToEnd();
                }
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( AppDomainUnloadedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( FileNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( OutOfMemoryException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            finally {
                _semaphore.Release();
            }
            return str;
        }

        /// <summary>
        /// Подпрограмма чтения строки из файла
        /// </summary>
        protected string ReadLine() {
            var str = string.Empty;

            _semaphore.Wait();
            try {
                if ( !Exist ) return str;
                using ( var rstream = new StreamReader( FileName, Encoding.Default ) ) {
                    // Чтение строки данных из файла
                    str = rstream.ReadLine();
                }
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( AppDomainUnloadedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( FileNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( OutOfMemoryException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            finally {
                _semaphore.Release();
            }
            return str;
        }

        /// <summary>
        /// Подпрограмма чтения байтового массива из файла
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read( byte[] array, int offset = 0, int count = 0 ) {
            var readed = 0;

            _semaphore.Wait();
            try {
                if ( !Exist ) return readed;
                using ( var fstream = File.OpenRead( FileName ) ) {
                    if ( offset == 0 && count == 0 ) {
                        count = ( int ) fstream.Length;
                        array = new byte[ fstream.Length ];
                    }
                    readed = fstream.Read( array, offset, count );
                    // декодируем байты в строку
                    //string textFromFile = System.Text.Encoding.Default.GetString(array);
                }
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( FileNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( ObjectDisposedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, $"Чтение файла {FileName}" );
            }
            finally {
                _semaphore.Release();
            }
            return readed;
        }

        /// <summary>
        /// Подпрограмма асинхронной записи в файл
        /// </summary>
        /// <param name="str"></param>
        public async void WriteAsync( string str ) {
            _semaphore.Wait();
            try {
                using ( var writer = new StreamWriter( FileName, true, Encoding.Default ) ) {
                    await writer.WriteAsync( str );
                }
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( SecurityException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ObjectDisposedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( InvalidOperationException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            finally {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Подпрограмма асинхронной записи в файл
        /// </summary>
        /// <param name="str"></param>
        public async void WriteLineAsync( string str ) {
            _semaphore.Wait();
            try {
                using ( var writer = new StreamWriter( FileName, true, Encoding.Default ) ) {
                    await writer.WriteLineAsync( str );
                }
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( SecurityException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ObjectDisposedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( InvalidOperationException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            finally {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Подпрограмма записи в файл
        /// </summary>
        /// <param name="str"></param>
        private void Write( string str ) {
            _semaphore.Wait();
            try {
                using ( var wstream = new StreamWriter( FileName, true, Encoding.Default ) ) {
                    wstream.Write( str );
                }
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( SecurityException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ObjectDisposedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            finally {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Подпрограмма записи строки в файл
        /// </summary>
        /// <param name="str"></param>
        protected void WriteLine( string str ) {
            Write( str + "\r\n" );
        }

        /// <summary>
        /// Подпрограмма записи байтового массива файла
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected void Write( byte[] array, int offset, int count ) {
            _semaphore.Wait();
            try {
                using ( var fstream = new FileStream( FileName, FileMode.OpenOrCreate, FileAccess.Write ) ) {
                    fstream.Write( array, offset, count );
                }
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( FileNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( SecurityException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, $"Запись файла {FileName}" );
            }
            finally {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Подпрограмма удаления файла
        /// </summary>
        public void Delete() {
            _semaphore.Wait();
            try {
                if ( !Exist ) return;
                File.Delete( FileName );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, $"Удаление файла {FileName}" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, $"Удаление файла {FileName}" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, $"Удаление файла {FileName}" );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Удаление файла {FileName}" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, $"Удаление файла {FileName}" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, $"Удаление файла {FileName}" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, $"Удаление файла {FileName}" );
            }
            finally {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Размер файла
        /// </summary>
        public long Lentgh() {
            long result = 0;
            try {
                if ( !Exist ) return result;
                result = !File.Exists( FileName ) ? 0 : new FileInfo( FileName ).Length;
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, $"Получение размера файла {FileName}" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, $"Получение размера файла {FileName}" );
            }
            catch ( SecurityException exc ) {
                _showErrorMessage?.Invoke( exc, $"Получение размера файла {FileName}" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, $"Получение размера файла {FileName}" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, $"Получение размера файла {FileName}" );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, $"Получение размера файла {FileName}" );
            }
            return result;
        }

        /// <summary>
        /// Подпрограмма копирования файла
        /// </summary>
        /// <param name="targetName"></param>
        // ReSharper disable once UnusedMember.Local
        private void Copy( string targetName ) {
            _semaphore.Wait();
            try {
                if ( !Exist ) return;
                File.Copy( FileName, targetName, true );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, "Копирование файла" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, "Копирование файла" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, "Копирование файла" );
            }
            catch ( FileNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, "Копирование файла" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, "Копирование файла" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, "Копирование файла" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, "Копирование файла" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, "Копирование файла" );
            }
            finally {
                _semaphore.Release();
            }
        }
    }
}