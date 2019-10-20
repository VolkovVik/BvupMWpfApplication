using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// Класс работы с файлами .ico
    /// </summary>
    internal class IconClass {
        /// <summary>
        /// Имя файла
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string FileName { get; private set; }

        /// <summary>
        /// Подпрограмма выдачи сообщения об ошибке
        /// </summary>
        private readonly Action< Exception, string > _showErrorMessage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="action"></param>
        public IconClass( string fileName = "pack://application:,,,/models/icon/icon.ico",
            Action< Exception, string > action = null ) {
            _showErrorMessage = action;
            FileName          = fileName;
        }

        /// <summary>
        /// Подпрограмма получения иконки окна
        /// </summary>
        public BitmapFrame get_icon() {
            BitmapFrame icon = null;
            // Настройка иконки
            try {
                var icon_uri = new Uri( FileName, UriKind.RelativeOrAbsolute );
                icon = BitmapFrame.Create( icon_uri );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( UriFormatException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( COMException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( FileFormatException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( InvalidOperationException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( OutOfMemoryException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
            }
            catch ( Exception exc ) {
                _showErrorMessage?.Invoke( exc,  "Ошибка задания иконки окна" );
                throw;
            }
            return icon;
        }

        /// <summary>
        /// Подпрограмма получения изображения
        /// </summary>
        public BitmapImage get_image() {
            var image = new BitmapImage();
            // Настройка иконки
            try {
                image.BeginInit();
                image.UriSource = new Uri( FileName, UriKind.RelativeOrAbsolute );
                image.EndInit();
            }
            catch ( IOException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка задания рисунка" );
            }
            catch ( InvalidOperationException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка задания рисунка" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка задания рисунка" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка задания рисунка" );
            }
            catch ( UriFormatException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка задания рисунка" );
            }
            return image;
        }
    }
}