using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace WpfApplication.Models.Main.XML {
    internal class XmlFileClass : IDisposable {
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string FileName { get; private set; }

        /// <summary>
        /// Проверка существования xml-файла
        /// </summary>
        public bool Exist => File.Exists( FileName );

        /// <summary>
        /// Название элемента
        /// </summary>
        private const string ElementParameter = "parameter";

        /// <summary>
        /// Название аттрибута
        /// </summary>
        private const string AttibuteName = "name";

        /// <summary>
        /// Семафор
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim( 1, 1 );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        protected XmlFileClass( string fileName ) {
            FileName = string.IsNullOrWhiteSpace( fileName ) ? "default.xml" : fileName;
            if ( !Path.IsPathRooted( FileName ) ) {
                FileName = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, FileName );
            }
            // Создание каталога
            new FolderClass( FileName, App.MyWindows.ShowFormErrorCommand.Execute ).Create();
        }

        ~XmlFileClass() {
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
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="append"></param>
        protected void Write( string element, string name, string value, bool append = false ) {
            // Проверка входных данных
            if ( string.IsNullOrWhiteSpace( element ) ||
                 string.IsNullOrWhiteSpace( name )    ||
                 string.IsNullOrWhiteSpace( value ) ) {
                return;
            }
            // Проверяем различные ситуации
            // Проверка существования файла
            if ( !Exist ) {
                // Файл не найден
                AddElementLevel1( element, name, value );
                return;
            }
            // Файл найден
            var xdoc = XDocument.Load( FileName );
            // Проверка корневого элемента
            if ( xdoc.Elements( $"{App.TaskManager.DeviceName}" ).ToList().Count ==  0 ) {
                // Ошибка формата xml-файла
                return;
            }
            // Проверка элемента 2-уровня
            if ( xdoc.Element( $"{App.TaskManager.DeviceName}" )?.Elements( ElementParameter )
                     .Where( item => item.Attribute( AttibuteName )?.Value == element ).ToList()
                     .Count == 0 ) {
                AddElementLevel2( element, name, value );
                return;
            }
            if ( append ) {
                AddElementLevel3( element, name, value );
                return;
            }
            // Проверка элемента 3-уровня
            if ( xdoc.Element( $"{App.TaskManager.DeviceName}" )?.Elements( ElementParameter )
                     .Where( item => item.Attribute( AttibuteName )?.Value == element ).Elements( name ).ToList()
                     .Count == 0 ) {
                AddElementLevel3( element, name, value );
                return;
            }
            // Элемент существует
            // Изменение значения
            WriteValue( element, name, value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void AddElementLevel1( string element, string name, string value ) {
            // Проверка входных данных
            if ( string.IsNullOrWhiteSpace( element ) ||
                 string.IsNullOrWhiteSpace( name )    ||
                 string.IsNullOrWhiteSpace( value )   ||
                 Exist ) {
                return;
            }
            _semaphore.Wait();
            // Файл не найден
            var xdoc = new XDocument();
            // Cоздаем корневой элемент
            var xml_main_element = new XElement( $"{App.TaskManager.DeviceName}" );
            // Создаем элемент второго уровня
            var xml_element = new XElement( ElementParameter );
            xml_element.Add( new XAttribute( AttibuteName, element ) );
            xml_element.Add( new XElement( name, value ) );
            // Добавляем элемент в корневой элемент
            xml_main_element.Add( xml_element );
            // Добавляем корневой элемент в документ
            xdoc.Add( xml_main_element );
            // Cохраняем документ
            xdoc.Save( FileName );
            _semaphore.Release();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void AddElementLevel2( string element, string name, string value ) {
            // Проверка входных данных
            if ( string.IsNullOrWhiteSpace( element ) ||
                 string.IsNullOrWhiteSpace( name )    ||
                 string.IsNullOrWhiteSpace( value )   ||
                 !Exist ) {
                return;
            }
            _semaphore.Wait();
            var xdoc = XDocument.Load( FileName );
            var list = xdoc.Element( $"{App.TaskManager.DeviceName}" )?.Elements( ElementParameter )
                           .Where( item => item.Attribute( AttibuteName )?.Value == element ).ToList();
            // Проверка входных данных
            if ( list != null && list.Count == 0 ) {
                // Создаем элемент третьего уровня
                var xml_paramert = new XElement( ElementParameter );
                xml_paramert.Add( new XAttribute( AttibuteName, element ) );
                xml_paramert.Add( new XElement( name, value ) );
                // Добавляем элемент в элемент 2-его уровня
                xdoc.Element( $"{App.TaskManager.DeviceName}" )?.Add( xml_paramert );
                // Cохраняем документ
                xdoc.Save( FileName );
            }
            _semaphore.Release();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void AddElementLevel3( string element, string name, string value ) {
            // Проверка входных данных
            if ( string.IsNullOrWhiteSpace( element ) ||
                 string.IsNullOrWhiteSpace( name )    ||
                 string.IsNullOrWhiteSpace( value )   ||
                 !Exist ) {
                return;
            }
            _semaphore.Wait();
            var xdoc = XDocument.Load( FileName );
            var list = xdoc.Element( $"{App.TaskManager.DeviceName}" )?.Elements( ElementParameter )
                           .Where( item => item.Attribute( AttibuteName )?.Value == element ).ToList();
            // Проверка входных данных
            if ( list != null  ) {
                // Создаем элемент четвертого уровня
                var xml_paramert = new XElement( name, value );
                // Добавляем элемент в элемент 3-его уровня
                foreach ( var item in list ) {
                    item.Add( xml_paramert );
                }
                // Cохраняем документ
                xdoc.Save( FileName );
            }
            _semaphore.Release();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void WriteValue( string element, string name, string value ) {
            // Проверка входных данных
            if ( string.IsNullOrWhiteSpace( element ) ||
                 string.IsNullOrWhiteSpace( name )    ||
                 string.IsNullOrWhiteSpace( value )   ||
                 !Exist ) {
                return;
            }
            _semaphore.Wait();
            var xdoc = XDocument.Load( FileName );
            var list = xdoc.Element( $"{App.TaskManager.DeviceName}" )?.Elements( ElementParameter )
                           .Where( item => item.Attribute( AttibuteName )?.Value == element ).Elements( name )
                           .ToList();
            // Проверка входных данных
            if ( list != null ) {
                foreach ( var item in list ) {
                    item.Value = value;
                }
                // Cохраняем документ
                xdoc.Save( FileName );
            }
            _semaphore.Release();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected IEnumerable< string > Read( string element, string name = "" ) {
            var value = new List< string >();

            // Проверка входных данных
            if ( string.IsNullOrWhiteSpace( element ) ||
                 !Exist ) {
                return value;
            }
            _semaphore.Wait();
            var xdoc = XDocument.Load( FileName );
            // Список элементов верхнего уровня
            var elements = xdoc.Element( $"{App.TaskManager.DeviceName}" )?.Elements( ElementParameter )
                               .Where( item => item.Attribute( AttibuteName )?.Value == element ).ToList();
            if ( elements != null ) {
                var list = string.IsNullOrWhiteSpace( name )
                    ? elements.Elements().ToList()
                    : elements.Elements( name ).ToList();
                // Цикл чтения
                foreach ( var item in list ) {
                    value.Add( item.Value );
                }
            }
            _semaphore.Release();
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected void Delete( string element, string name = "" ) {
            // Проверка входных данных
            if ( string.IsNullOrWhiteSpace( element ) ||
                 !Exist ) {
                return;
            }
            _semaphore.Wait();
            var xdoc = XDocument.Load( FileName );
            // Список элементов верхнего уровня
            var elements = xdoc.Element( $"{App.TaskManager.DeviceName}" )?.Elements( ElementParameter )
                               .Where( item => item.Attribute( AttibuteName )?.Value == element ).ToList();
            if ( elements != null ) {
                var list = string.IsNullOrWhiteSpace( name ) ? elements : elements.Elements( name ).ToList();
                // Цикл удаления
                foreach ( var item in list ) {
                    item.Remove();
                }
                // Cохраняем документ
                xdoc.Save( FileName );
            }
            _semaphore.Release();
        }
    }
}