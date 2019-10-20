using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// 
    /// </summary>
    internal class FolderClass {
        /// <summary>
        /// Путь доступа
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string FolderPath { get; private set; }

        /// <summary>
        /// Проверка существования xml-файла
        /// </summary>
        private bool Exist => Directory.Exists( FolderPath );

        /// <summary>
        /// Подпрограмма выдачи сообщения об ошибке
        /// </summary>
        private readonly Action< Exception, string > _showErrorMessage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="action"></param>
        public FolderClass( string path, Action< Exception, string > action = null ) {
            _showErrorMessage = action;
            try {
                // Получение полного пути папки
                FolderPath = string.IsNullOrWhiteSpace( path ) || Path.IsPathRooted( path )
                    ? path
                    : Path.Combine( AppDomain.CurrentDomain.BaseDirectory, path );
                if ( Path.HasExtension( FolderPath ) ) {
                    // Преобразование путь доступа к файлу в путь доступа к папке
                    FolderPath = Path.GetDirectoryName( FolderPath );
                }
            }
            catch ( ArgumentException exc ) {
                FolderPath = string.Empty;
                _showErrorMessage?.Invoke( exc, $"Недопустимый путь к папке {path}" );
            }
            catch ( PathTooLongException exc ) {
                FolderPath = string.Empty;
                _showErrorMessage?.Invoke( exc, $"Недопустимый путь к папке {path}" );
            }
        }

        /// <summary>
        /// Подпрограмма создания директории
        /// </summary>
        /// <returns></returns>
        public void Create() {
            if ( string.IsNullOrWhiteSpace( FolderPath ) || Exist ) return;
            try {
                // Создание папки
                Directory.CreateDirectory( FolderPath );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
        }

        /// <summary>
        /// Подпрограмма создания директории с параметрами доступа
        /// </summary>
        /// <returns></returns>
        public void CreateWithAccess() {
            if ( string.IsNullOrWhiteSpace( FolderPath ) || Exist ) return;
            try {
                // Создание папки с параметрами доступа
                var               d  = new DirectoryInfo( FolderPath );
                var               ds = new DirectorySecurity();
                IdentityReference u  = new NTAccount( Environment.UserDomainName, Environment.UserName );
                // владелец - текущий пользователь
                ds.SetOwner( u );
                // полный доступ текущему пользователю
                var permissions = new FileSystemAccessRule( u, FileSystemRights.FullControl, AccessControlType.Allow );
                ds.AddAccessRule( permissions );
                d.Create( ds );
            }
            catch ( SecurityException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( PlatformNotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, "Создание директории" );
            }
        }

        /// <summary>
        /// Подпрограмма удаления директории
        /// </summary>
        /// <returns></returns>
        public void Delete() {
            if ( string.IsNullOrWhiteSpace( FolderPath ) || !Exist ) return;
            try {
                // Удаление папки
                Directory.Delete( FolderPath, true );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, "Удаление директории" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, "Удаление директории" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage?.Invoke( exc, "Удаление директории" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, "Удаление директории" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, "Удаление директории" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, "Удаление директории" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, "Удаление директории" );
            }
        }
    }
}