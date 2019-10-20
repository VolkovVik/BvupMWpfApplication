using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// для задания целевого элемента, к которому применяется атрибут.
    /// Класс предоставляет удобные методы для извлечения и 
    /// проверки настраиваемых атрибутов Attribute.
    /// </summary>
    /// <remarks>
    /// Коллекция Attribute связывает предопределенную системную информацию 
    /// или заданную пользователем информацию с целевым элементом. 
    /// Целевым элементом может быть: сборка, класс, конструктор, делегат, 
    /// перечисление, событие, поле, интерфейс, метод, переносимый исполняемый
    /// (PE) файл, модуль, параметр, свойство, возвращаемое значение, 
    /// структура или другой атрибут.
    /// Информация, предоставляемая атрибутом, называется также метаданными.
    /// Метаданные можно анализировать в приложении во время выполнения, 
    /// для того чтобы управлять тем, как это приложение осуществляет обработку
    /// данных, или до времени выполнения внешними средствами для управления
    /// обработкой и выполнением самого приложения.Например, на платформе .NET
    /// Framework предопределены и используются типы атрибутов для управления 
    /// поведением времени выполнения, и некоторые языки программирования 
    /// используют типы атрибутов для представления языковых функций, не 
    /// поддерживаемых непосредственно общей системой типов .NET Framework.
    /// Все типы атрибутов прямо или косвенно наследуются от класса Attribute.
    /// Атрибуты могут быть применены к любому целевому элементу; несколько 
    /// экземпляров атрибута могут быть применены к одному и тому же целевому 
    /// элементу; атрибуты могут наследоваться элементом, являющимся 
    /// производным от целевого элемента.Используйте класс AttributeTargets 
    /// для задания целевого элемента, к которому применяется атрибут.
    /// Класс Attribute предоставляет удобные методы для извлечения и 
    /// проверки настраиваемых атрибутов.
    /// </remarks>
    internal static class AssemblyClass {
        ///<remarks>
        /// Полезные ссылки
        /// https://technet.microsoft.com/ru-ru/library/ms753303%28v=vs.90%29.aspx
        /// https://docs.microsoft.com/ru-ru/dotnet/framework/wpf/app-development/wpf-application-resource-content-and-data-files
        /// https://docs.microsoft.com/ru-ru/dotnet/framework/resources/working-with-resx-files-programmatically
        /// </remarks>
        /// <summary>
        /// Подпрограмма получения списка файлов Embedded Resources с описанием
        /// </summary>
        /// <returns></returns>
        public static string get_files() {
            var str = string.Empty;

            try {
                // Получение файлов Embedded Resources
                var assembly = Assembly.GetExecutingAssembly();
                var names    = assembly.GetManifestResourceNames();
                foreach ( var name_resource in names ) {
                    using ( var stream = assembly.GetManifestResourceStream( name_resource ) ) {
                        if ( stream == null ) continue;
                        // При использовании оператора using возникает предупреждение CA2202:
                        // не удаляйте объекты несколько раз
                        // Правильно реализованный метод Dispose можно вызывать несколько раз, 
                        // и при этом не создается исключение.Однако это нельзя гарантировать, 
                        // и чтобы не допустить создания исключения System.ObjectDisposedException, 
                        // не следует вызывать метод Dispose для какого - либо объекта
                        // более одного раза.
                        //using ( var reader = new StreamReader( stream ) ) {
                        var reader  = new StreamReader( stream );
                        var buffer  = new char[ 1024 ];
                        var n_chars = reader.Read( buffer, 0, buffer.Length );
                        var text    = new string( buffer, 0, n_chars );
                        str += $"{name_resource}:" + Environment.NewLine + text + Environment.NewLine;
                        //}
                    }
                }
            }
            catch ( ArgumentNullException exc ) {
                // The name parameter is null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( ArgumentException exc ) {
                // The name parameter is an empty string ("").
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( FileLoadException exc ) {
                // A file that was found could not be loaded. 
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( FileNotFoundException exc ) {
                // name was not found. 
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( BadImageFormatException exc ) {
                // name is not a valid assembly. 
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( NotImplementedException exc ) {
                // Resource length is greater than Int64.MaxValue.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            return str;
        }

        /// <summary>
        /// Подпрограмма чтения файлов Embedded Resources
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        internal static string GetFromResources( string resourceName ) {
            var str = string.Empty;

            try {
                var assem = Assembly.GetExecutingAssembly();
                using ( var stream = assem.GetManifestResourceStream( assem.GetName().Name + '.' + resourceName ) ) {
                    if ( stream != null ) {
                        // При использовании оператора using возникает предупреждение CA2202:
                        // не удаляйте объекты несколько раз
                        // Правильно реализованный метод Dispose можно вызывать несколько раз, 
                        // и при этом не создается исключение.Однако это нельзя гарантировать, 
                        // и чтобы не допустить создания исключения System.ObjectDisposedException, 
                        // не следует вызывать метод Dispose для какого - либо объекта
                        // более одного раза.
                        //using ( var reader = new StreamReader( stream ) ) {
                        var reader = new StreamReader( stream );
                        str = reader.ReadToEnd();
                        //}
                    }
                }
            }
            catch ( ArgumentNullException exc ) {
                // The name parameter is null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( ArgumentException exc ) {
                // The name parameter is an empty string ("").
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( FileLoadException exc ) {
                // A file that was found could not be loaded. 
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( FileNotFoundException exc ) {
                // name was not found. 
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( BadImageFormatException exc ) {
                // name is not a valid assembly. 
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            catch ( NotImplementedException exc ) {
                // Resource length is greater than Int64.MaxValue.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка чтения файлов Embedded Resources" );
            }
            return str;
        }

        /// <summary>
        /// Подпрограмма получения времени и даты изменения .exe файла
        /// </summary>
        /// <returns></returns>
        public static DateTime get_data_exe() {
            var data = DateTime.MinValue;

            try {
                // Получение пути доступа к .exe файлу
                var path = Assembly.GetExecutingAssembly().Location;
                if ( !string.IsNullOrWhiteSpace( path ) ) {
                    // Получение времени и даты изменения .exe файла
                    data = File.GetLastWriteTime( path );
                    //str_data = data.ToString( "d MMMMMMMMMM yyyy HH:mm:ss.ffff" );
                    // Получение имени файла
                    //var name = new FileInfo( path ).Name;
                    // Получение размера файла
                    //var size = new FileInfo( path ).Length;
                }
            }
            catch ( UnauthorizedAccessException exc ) {
                // У вызывающего объекта отсутствует необходимое разрешение.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                // Свойство path имеет значение null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                // The date and time is outside the range
                // of dates supported by the calendar used by provider. 
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( PathTooLongException exc ) {
                // Указанный путь, имя файла или оба значения 
                // превышают максимальную длину, заданную в системе.
                // Например, для платформ на основе Windows 
                // длина пути должна составлять менее 248 знаков,
                // а длина имен файлов — менее 260 знаков.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( NotSupportedException exc ) {
                // Параметр path задан в недопустимом формате.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                // path представляет собой строку нулевой длины, 
                // содержащую только пробелы или один 
                // или несколько недопустимых символов, 
                // заданных методом InvalidPathChars.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( FormatException exc ) {
                //The length of format is 1, 
                // and it is not one of the format specifier 
                // characters defined for DateTimeFormatInfo.
                //- or -
                //format does not contain a valid custom format pattern.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            return data;
        }

        /// <summary>
        /// Подпрограмма получения версии сборки
        /// </summary>
        /// <returns></returns>
        public static string get_version_exe() {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Подпрограмма получения версии сборки
        /// </summary>
        /// <returns></returns>
        public static string get_version_file_exe() {
            var version = string.Empty;

            try {
                // Получение пути доступа к .exe файлу
                var path = Assembly.GetExecutingAssembly().Location;
                if ( !string.IsNullOrWhiteSpace( path ) ) {
                    version = FileVersionInfo.GetVersionInfo( path ).FileVersion;
                }
            }
            catch ( NotSupportedException exc ) {
                // Параметр path задан в недопустимом формате.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( FileNotFoundException exc ) {
                // The exception that is thrown when an attempt 
                // to access a file that does not exist on disk fails.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            return version;
        }

        /// <summary>
        /// Подпрограмма получения описания сборки
        /// </summary>
        /// <returns></returns>
        public static string get_description_file_exe() {
            var description = string.Empty;

            try {
                //version = ( ( AssemblyDescriptionAttribute ) Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false ).FirstOrDefault() ).Description.ToString();
                var assembly_description_attribute = ( AssemblyDescriptionAttribute )
                    Assembly.GetExecutingAssembly()
                            .GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false )
                            .FirstOrDefault();
                // Проверка на NullReferenceException
                if ( assembly_description_attribute != null ) {
                    description = assembly_description_attribute.Description;
                }
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                // Свойство path имеет значение null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                // path представляет собой строку нулевой длины, 
                // содержащую только пробелы или один 
                // или несколько недопустимых символов, 
                // заданных методом InvalidPathChars.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            return description;
        }

        /// <summary>
        /// Подпрограмма получения описания сборки
        /// </summary>
        /// <returns></returns>
        public static string get_title_file_exe() {
            var title = string.Empty;

            try {
                //version = ( ( AssemblyDescriptionAttribute ) Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false ).FirstOrDefault() ).Description.ToString();
                var assembly_title_attribute = ( AssemblyTitleAttribute )
                    Assembly.GetExecutingAssembly()
                            .GetCustomAttributes( typeof( AssemblyTitleAttribute ), false )
                            .FirstOrDefault();
                // Проверка на NullReferenceException
                if ( assembly_title_attribute != null ) {
                    title = assembly_title_attribute.Title;
                }
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                // Свойство path имеет значение null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                // path представляет собой строку нулевой длины, 
                // содержащую только пробелы или один 
                // или несколько недопустимых символов, 
                // заданных методом InvalidPathChars.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            return title;
        }

        /// <summary>
        /// Подпрограмма получения описания сборки
        /// </summary>
        /// <returns></returns>
        public static string get_product_file_exe() {
            var product = string.Empty;

            try {
                //version = ( ( AssemblyDescriptionAttribute ) Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false ).FirstOrDefault() ).Description.ToString();
                var assembly_product_attribute = ( AssemblyProductAttribute )
                    Assembly.GetExecutingAssembly()
                            .GetCustomAttributes( typeof( AssemblyProductAttribute ), false )
                            .FirstOrDefault();
                // Проверка на NullReferenceException
                if ( assembly_product_attribute != null ) {
                    product = assembly_product_attribute.Product;
                }
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                // Свойство path имеет значение null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                // path представляет собой строку нулевой длины, 
                // содержащую только пробелы или один 
                // или несколько недопустимых символов, 
                // заданных методом InvalidPathChars.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            return product;
        }

        /// <summary>
        /// Подпрограмма получения описания сборки
        /// </summary>
        /// <returns></returns>
        public static string get_guid_file_exe() {
            var guid = string.Empty;

            try {
                //version = ( ( AssemblyDescriptionAttribute ) Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false ).FirstOrDefault() ).Description.ToString();
                var assembly_guid_attribute = ( GuidAttribute )
                    Assembly.GetExecutingAssembly()
                            .GetCustomAttributes( typeof( GuidAttribute ), false )
                            .FirstOrDefault();
                // Проверка на NullReferenceException
                if ( assembly_guid_attribute != null ) {
                    guid = assembly_guid_attribute.Value;
                }
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                // Свойство path имеет значение null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                // path представляет собой строку нулевой длины, 
                // содержащую только пробелы или один 
                // или несколько недопустимых символов, 
                // заданных методом InvalidPathChars.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            return guid;
        }

        /// <summary>
        /// Подпрограмма получения описания сборки
        /// </summary>
        /// <returns></returns>
        public static string get_сopyright_file_exe() {
            var сopyright = string.Empty;

            try {
                //version = ( ( AssemblyDescriptionAttribute ) Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false ).FirstOrDefault() ).Description.ToString();
                var assembly_product_attribute = ( AssemblyCopyrightAttribute )
                    Assembly.GetExecutingAssembly()
                            .GetCustomAttributes( typeof( AssemblyCopyrightAttribute ), false )
                            .FirstOrDefault();
                // Проверка на NullReferenceException
                if ( assembly_product_attribute != null ) {
                    сopyright = assembly_product_attribute.Copyright;
                }
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                // Свойство path имеет значение null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                // path представляет собой строку нулевой длины, 
                // содержащую только пробелы или один 
                // или несколько недопустимых символов, 
                // заданных методом InvalidPathChars.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            return сopyright;
        }

        /// <summary>
        /// Подпрограмма получения описания сборки
        /// </summary>
        /// <returns></returns>
        public static string get_company_file_exe() {
            var сopyright = string.Empty;

            try {
                //version = ( ( AssemblyDescriptionAttribute ) Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false ).FirstOrDefault() ).Description.ToString();
                var assembly_product_attribute = ( AssemblyCompanyAttribute )
                    Assembly.GetExecutingAssembly()
                            .GetCustomAttributes( typeof( AssemblyCompanyAttribute ), false )
                            .FirstOrDefault();
                // Проверка на NullReferenceException
                if ( assembly_product_attribute != null ) {
                    сopyright = assembly_product_attribute.Company;
                }
            }
            catch ( NullReferenceException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentNullException exc ) {
                // Свойство path имеет значение null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            catch ( ArgumentException exc ) {
                // path представляет собой строку нулевой длины, 
                // содержащую только пробелы или один 
                // или несколько недопустимых символов, 
                // заданных методом InvalidPathChars.
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка конвертации строки в число" );
            }
            return сopyright;
        }
    }
}