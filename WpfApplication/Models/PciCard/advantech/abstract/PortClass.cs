using System;
using System.IO;
using WpfApplication.Models.Main;
using WpfApplication.Models.Main.XML;
using WpfApplication.Models.Test;

namespace WpfApplication.Models.PciCard.advantech.@abstract {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PortClass< T > where T : DaqPciClass, new() {
        /// <summary>
        /// Имя интерфейсной платы
        /// </summary>
        private string Name { get; }

        /// <summary>
        /// Признак инициализации интерфейсной платы
        /// </summary>
        public bool IsInit { get; private set; }

        /// <summary>
        /// Максимальное количество интерфейсных плат
        /// </summary>
        public readonly int MaxDevice;

        /// <summary>
        /// Максимальное количество каналов интерфейсной платы
        /// </summary>
        public readonly int MaxChannel;

        /// <summary>
        /// Массив объектов интерфейсных плат
        /// </summary>
        protected readonly T[] Device;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxDevice"></param>
        /// <param name="maxChannel"></param>
        protected PortClass( string name, int maxDevice, int maxChannel ) {
            // Сброс признака инициализации
            IsInit = false;
            // Имя интерфейсной платы
            Name = name;
            // Максимальное количество каналов интерфейсной платы
            MaxChannel = maxChannel;
            // Максимальное количество интерфейсных плат
            MaxDevice = maxDevice;
            // Инициализация массива устройств
            Device = new T[ maxDevice ];
            // Открытие устройства
            //Open();
        }

        ~PortClass() {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open() {
            // Установка признака инициализации
            IsInit = true;

            // Инициализация плат
            for ( var i = 0; i < Device.Length; i++ ) {
                var result = false;
                // Сдвиг шкалы ProgressBar
                App.MyWindows.ValueProgress++;
                // Считывание индекса платы PCI из .xml файла
                var xml         = new XmlClass();
                var description = xml.Read( Name, XmlClass.ElementBid + i );
                if ( string.IsNullOrWhiteSpace( description ) ) {
                    App.TaskManager.Log.WriteLineAsync( $"Ошибка чтения индекса платы {Name} из файла настроек" );
                } else {
                    // Инициализация драйвера
                    try {
                        // Создание объекта интерфейсной платы
                        Device[ i ] = new T();
                        Device[ i ].Open( description );
                        if ( Device[ i ].IsInit ) {
                            result = true;
                            // Установка безопасного режима
                            SafeMode( i );
                        }
                    }
                    catch ( DllNotFoundException exc ) {
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( EntryPointNotFoundException exc ) {
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( FileNotFoundException exc ) {
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( MissingMemberException exc ) {
                        // При работе с драйвером Adsapi возможно попадание в данное прерывание
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( Exception< Pci1716ExceptionArgs > exc ) {
                        // При работе с драйвером Adsapi возможно попадание в данное прерывание
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( Exception< Pci1721ExceptionArgs > exc ) {
                        // При работе с драйвером Adsapi возможно попадание в данное прерывание
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( Exception< Pci1724UExceptionArgs > exc ) {
                        // При работе с драйвером Adsapi возможно попадание в данное прерывание
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( Exception< Pci1747UExceptionArgs > exc ) {
                        // При работе с драйвером Adsapi возможно попадание в данное прерывание
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( Exception< Pci1753ExceptionArgs > exc ) {
                        // При работе с драйвером Adsapi возможно попадание в данное прерывание
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( Exception exc ) {
                        // При работе с драйвером Adsapi возможно попадание в данное прерывание
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                }
                if ( !result ) {
                    // Плата не инициализирована
                    // Сброс признака инициализации модуля
                    IsInit = false;
                }
                // Возврат результата
                CommonClass.SetResText( $"Инициализация платы {description}", 1, 75, result, visible: true );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Close() {
            // Очистка ресурсов по требованию MSDN
            foreach ( var device in Device ) {
                device?.Close();
                device?.Dispose();
            }
            // Сброс признака инициализации модуля
            IsInit = false;
        }

        /// <summary>
        /// Подпрограмма проверки допустимости обращения к плате
        /// </summary>
        /// <param name="index"></param>
        protected void Validation( int index ) {
            // Проверка инициализации модуля
            if ( !IsInit ) {
                throw new Exception< AdvantechPciExceptionArgs >(
                    new AdvantechPciExceptionArgs( index, Name ), "The modul of card is not initialized" );
            }
            // Проверка допустимости индекса
            if ( index < 0 || index >= Device.Length ) {
                throw new Exception< AdvantechPciExceptionArgs >(
                    new AdvantechPciExceptionArgs( index, Name ), "The index of card is not valid" );
            }
            // Проверка существования объетка
            if ( Device[ index ] == null ) {
                throw new Exception< AdvantechPciExceptionArgs >(
                    new AdvantechPciExceptionArgs( index, Name ), $"The object of card #{index} is not creatе" );
            }
            // Проверка инициализации объекта платы
            if ( !Device[ index ].IsInit ) {
                throw new Exception< AdvantechPciExceptionArgs >(
                    new AdvantechPciExceptionArgs( index, Name ), $"The object of card #{index} is not initialized" );
            }
        }

        /// <summary>
        /// Подпрограмма установки безопасного режима работы платы
        ///  </summary>
        protected abstract void SafeMode( int index );
    }
}