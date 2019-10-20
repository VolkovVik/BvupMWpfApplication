using System;
using System.IO;
using System.Security;
using System.Threading;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// 
    /// </summary>
    internal class PdfFileClass : IDisposable {
        /// <summary>
        /// Имя файла
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string FileName { get; private set; }

        /// <summary>
        /// Семафор
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim( 1, 1 );

        /// <summary>
        /// Подпрограмма выдачи сообщения об ошибке
        /// </summary>
        private readonly Action< Exception, string > _showErrorMessage = App.MyWindows.ShowFormErrorCommand.Execute;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="fileName"></param>
        public PdfFileClass( string fileName ) {
            fileName = string.IsNullOrWhiteSpace( fileName ) ? "default.pdf" : fileName;
            if ( !System.IO.Path.IsPathRooted( fileName ) ) {
                FileName = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, fileName );
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~PdfFileClass() {
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
        /// Подпрограмма чтения текста
        /// </summary>
        /// <returns></returns>
        public string Read() {
            _semaphore.Wait();
            var str = string.Empty;

            // считаем, что программе передается один аргумент - имя файла
            using ( var reader = new PdfReader( FileName ) ) {
                // нумерация страниц в PDF начинается с единицы.
                for ( var i = 1; i <= reader.NumberOfPages; ++i ) {
                    var strategy = new SimpleTextExtractionStrategy();
                    str += PdfTextExtractor.GetTextFromPage( reader, i, strategy );
                }
            }
            _semaphore.Release();
            return str;
        }

        /// <summary>
        /// Подпрограмма записи текста и установки фона
        /// </summary>
        /// <param name="str"></param>
        /// <param name="backColor"></param>
        public void Write( string str, BaseColor backColor ) {
            // Создание шрифта
            var font = GetFont();
            _semaphore.Wait();
            try {
                // Создание файла
                using ( var file_stream = new FileStream( FileName, FileMode.Create ) ) {
                    // Создание документа
                    using ( var document = new Document() ) {
                        PdfWriter.GetInstance( document, file_stream );
                        // Книжная ориентация
                        var rec = new Rectangle( PageSize.A4 ) {
                            // Альбомная ориентация
                            // PageSize.A4.Rotate()
                            BackgroundColor = backColor
                        };
                        // Установка размера страницы
                        document.SetPageSize( rec );
                        //document.SetMargins( 0, 0, 0, 0 );
                        // Открытие документа
                        document.Open();
                        // Создание параграфа
                        var paragraph = new Paragraph( str, font );
                        // Изменение межстрочного расстояния
                        // Корректировка параметра paragraph.Leading;
                        paragraph.SetLeading( 1, 1 );
                        // Запись текстовых данных
                        document.Add( paragraph );
                        //var res = Application.GetResourceStream(
                        //    new Uri( App.MyGmainTest.ConfigProgram.ImageUri,
                        //        UriKind.RelativeOrAbsolute ) );
                        //if ( res != null ) {
                        //    using ( var stream_file = new FileStream( "newRiver.jpg", FileMode.Create ) ) {
                        //        res.Stream.CopyTo( stream_file );
                        //    }
                        //    var jpg = Image.GetInstance( "pack://application:,,,/models/icon/kret.png" );
                        //    jpg.Alignment = Element.ALIGN_CENTER;
                        //    document.Add( jpg );
                        //}
                        // Warning CA2202: не удаляйте объекты несколько раз
                        // http://msdn.microsoft.com/library/ms182334.aspx
                        //document.Close();
                    }
                    //using ( var writer = PdfWriter.GetInstance( document, stream ) ) {
                    //    writer.DirectContent.BeginText();
                    //    writer.DirectContent.SetFontAndSize( base_font, 12f );
                    //    writer.DirectContent.ShowTextAligned( Element.ALIGN_TOP, str, 0, 750, 0 );
                    //    writer.DirectContent.EndText();
                    //}
                }
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( PlatformNotSupportedException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( SecurityException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( FileNotFoundException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( DirectoryNotFoundException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( DocumentException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            catch ( IOException exc ) {
                _showErrorMessage( exc, "Создание pdf-файла" );
            }
            finally {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Подпрограмма создание шрифта
        /// </summary>
        /// <param name="nameFont"></param>
        /// <returns></returns>
        private Font GetFont( string nameFont = "PTM55F.ttf" ) {
            Font font_mono = null;
            try {
                //@"C:\Windows\Fonts\arial.ttf"
                //var fonts_uri = System.IO.Path.Combine(
                //    Environment.GetFolderPath( Environment.SpecialFolder.Fonts ), "arial.ttf" );
                //var base_font = BaseFont.CreateFont( fonts_uri, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED );
                //var arial = new Font(base_font, Font.DEFAULTSIZE, Font.NORMAL, BaseColor.BLACK);

                // Создание шрифта
                var uri  = new Uri( "/Models/Font/" + nameFont, UriKind.Relative );
                var info = Application.GetResourceStream( uri );
                if ( info != null ) {
                    var assembly_data = new byte[ info.Stream.Length ];
                    // ReSharper disable once UnusedVariable
                    var readed = info.Stream.Read( assembly_data, 0, assembly_data.Length );
                    var base_font = BaseFont.CreateFont( nameFont, BaseFont.IDENTITY_H, BaseFont.EMBEDDED,
                        BaseFont.CACHED, assembly_data, null );
                    font_mono = new Font( base_font, Font.DEFAULTSIZE, Font.NORMAL, BaseColor.BLACK );
                }
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( FileLoadException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( FileNotFoundException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( BadImageFormatException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( NotImplementedException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( IOException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( ObjectDisposedException exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            catch ( Exception exc ) {
                _showErrorMessage( exc, "Создание шрифта для pdf-файла" );
            }
            return font_mono;
        }
    }
}