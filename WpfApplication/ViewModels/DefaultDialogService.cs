using System;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using WpfApplication.Models.Form;

namespace WpfApplication.ViewModels {
    /// <summary>
    /// Интерфейс диалоговых окон
    /// </summary>
    public interface IDialogService {
        /// <summary>
        /// Путь доступа к выбранному файлу
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void ShowMessage( string message );

        /// <summary>
        /// Подпрограмма открытия файла
        /// </summary>
        /// <returns></returns>
        bool OpenFileDialog();

        /// <summary>
        /// Подпрограмма открытия файла
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        bool OpenFileDialog( string path, string filters );

        /// <summary>
        /// Подпрограмма сохранения файла
        /// </summary>
        /// <returns></returns>
        bool SaveFileDialog();

        /// <summary>
        /// Подпрограмма вызова формы инициализации
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        void ShowFormInit( string title, string text );

        /// <summary>
        /// Подпрограмма вызова формы сообщения о возникщем исключении
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="text"></param>
        void ShowFormError( Exception exception, string text );
    }

    /// <summary>
    /// 
    /// </summary>
    public class DefaultDialogService : IDialogService {
        /// <summary>
        /// Путь доступа к выбранному файлу
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessage( string message ) {
            MessageBox.Show( message );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool OpenFileDialog() {
            var open_file_dialog = new OpenFileDialog();
            if ( open_file_dialog.ShowDialog() != true ) {
                return false;
            }
            // Путь доступа к выбранному файлу
            FilePath = open_file_dialog.FileName;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public bool OpenFileDialog( string path, string filters = "" ) {
            if ( !Directory.Exists( path ) ) {
                return false;
            }
            var open_file_dialog = new OpenFileDialog {
                RestoreDirectory = true,
                InitialDirectory = path,
                Filter = filters                                                           +
                         @"Work files (*.mot,*.s19,*.elf,*.bin )|*.mot;*.s19;*.elf;*.bin|" +
                         @"HEX files (*.mot,*.s19 )|*.mot;*.s19|"                          +
                         @"All files (*.*)|*.*"
            };
            if ( open_file_dialog.ShowDialog() != true ) {
                return false;
            }
            // Путь доступа к выбранному файлу
            FilePath = open_file_dialog.FileName;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SaveFileDialog() {
            var save_file_dialog = new SaveFileDialog();
            if ( save_file_dialog.ShowDialog() != true ) {
                return false;
            }
            FilePath = save_file_dialog.FileName;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        public void ShowFormInit( string title, string text ) {
            new WindowInit( title, text ).ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="text"></param>
        public void ShowFormError( Exception exception, string text ) {
            new WindowException( exception, text ).ShowDialog();
        }
    }
}