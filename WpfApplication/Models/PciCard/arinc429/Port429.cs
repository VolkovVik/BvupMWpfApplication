using System;
using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.Main.XML;
using WpfApplication.Models.Test;

namespace WpfApplication.Models.PciCard.arinc429 {
    /// <summary>
    /// Класс, определяющий функции для работы с имитатором
    /// </summary>
    public class Port429 {
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
        /// Максимальное количество каналов приема
        /// </summary>
        public const byte MaxRxChannel = 16;

        /// <summary>
        /// Максимальное количество каналов выдачи
        /// </summary>
        public const byte MaxTxChannel = 16;

        /// <summary>
        /// Массив объектов плат daq_pci1753
        /// </summary>
        private readonly Pci429Class[] _device;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Port429( string name, int maxDevice ) {
            // Сброс признака инициализации
            IsInit = false;
            // Имя интерфейсной платы
            Name = name;
            // Максимальное количество интерфейсных плат
            MaxDevice = maxDevice;
            // Инициализация массива устройств
            _device = new Pci429Class[ MaxDevice ];
            // Открытие плат PCI1753
            //Open();
        }

        ~Port429() {
            Close();
        }

        /// <summary>
        /// Подпрограмма инициализации драйвера платы PCI429
        /// </summary>
        /// <returns></returns>
        public void Open() {
            // Установка признака инициализации
            IsInit = true;

            // Инициализация плат
            for ( var i = 0; i < MaxDevice; i++ ) {
                // Сдвиг шкалы ProgressBar
                App.MyWindows.ValueProgress++;
                var result = false;
                // Считывание номера платы PCI429 из .xml файла 

                var xml            = new XmlClass();
                var address_device = xml.Read( Name, XmlClass.ElementSn + i ).ToUintWithoutBaseExt();
                var description    = $"{Name},ADR#00000";
                if ( null == address_device ) {
                    App.TaskManager.Log.WriteLineAsync( "Ошибка чтения номера платы PCI429 из файла настроек" );
                } else {
                    description = $"{Name},ADR#{address_device:D5}";
                    try {
                        // Инициализация драйвера
                        // Создание объекта платы PCI429  
                        address_device &= 0xFFFF;
                        _device[ i ]   =  new Pci429Class();
                        if ( address_device != null ) {
                            var error = _device[ i ].Open( ( ushort ) address_device );
                            if ( 0 == error ) {
                                result = true;
                            } else {
                                // Указатель на плату не получен
                                // Плата не инициализирована
                                App.TaskManager.Log.WriteLineAsync( $"Ошибка открытия {error:D}" );
                            }
                        }
                    }
                    catch ( DllNotFoundException exc ) {
                        // The exception that is thrown when a DLL 
                        // specified in a DLL import cannot be found.
                        // DllNotFoundException uses the HRESULT_COR_E_DLLNOTFOUND,
                        // which has the value 0x80131524.
                        App.TaskManager.Log.WriteLineAsync(  exc.Message );
                    }
                    catch ( EntryPointNotFoundException exc ) {
                        // The exception that is thrown 
                        // when an attempt to load a class fails 
                        // due to the absence of an entry method. 
                        // The EntryPointNotFoundException type 
                        // exposes the following members.
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
        ///  Подпрограмма прекращения работы с драйвером платы PCI429
        /// </summary>
        private void Close() {
            // Сброс признака инициализации 
            IsInit = false;
            // Закрытие плат
            for ( var i = 0; i < MaxDevice; i++ ) {
                _device[ i ]?.Close();
            }
        }

        /// <summary>
        /// Подпрограмма проверки допустимости индекса порта
        /// </summary>
        /// <returns></returns>
        private void Validation( int index ) {
            if ( !IsInit ) {
                // Модуль не инициализирован 
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "The modul of PCI429 is not initialized" );
            }
            if ( 0 > index || MaxDevice <= index ) {
                // Индекс неверный
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    $"The index {index} cars PCI-429 is not valid" );
            }
            // Проверка существования объетка
            if ( _device[ index ] == null ) {
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( index ),
                    $"The object of card PCI-429 #{index} is not creatе" );
            }
        }

        /// <summary>
        /// Подпрограмма задания частоты работы канала ПБК и режима четности
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <param name="nk"></param>
        /// <param name="freq"></param>
        /// <param name="parityOff"></param>
        /// <returns></returns>
        public void Config( int index, byte type, byte nk, FREQ freq, bool parityOff = false ) {
            Validation( index );
            if ( 0 != _device[ index ].Config( type, nk, freq, parityOff ) ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка задания конфигурации интерфейсной плате PCI-429" );
            }
        }

        /// <summary>
        /// Запуск платы и переход в рабочий режим
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void Start( int index = 0 ) {
            Validation( index );
            if ( 0 != _device[ index ].Start() ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка запуска интерфейсной платы PCI-429" );
            }
        }

        /// <summary>
        /// Запуск платы и переход в рабочий режим
        /// </summary>
        public void StartAll() {
            // Запуск платы PCI429 
            for ( var index = 0; index < MaxDevice; index++ ) {
                Start( index );
            }
        }

        /// <summary>
        /// Сброс модуля
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void Reset( int index = 0 ) {
            Validation( index );
            if ( 0 != _device[ index ].Reset() ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка сброса интерфейсной платы PCI-429" );
            }
        }

        /// <summary>
        /// Сброс модуля
        /// </summary>
        /// <returns></returns>
        public void ResetAll() {
            for ( var index = 0; index < MaxDevice; index++ ) {
                Reset( index );
            }
        }

        /// <summary>
        /// Подпрограмма проверки окончания предыдущей выдачи
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public void CheckTx( int index, ushort channel, int time ) {
            int err_pci;

            Validation( index );
            // Ожидание окончания предыдущей выдачи
            var n_time = DateTime.Now;
            do {
                err_pci = _device[ index ].check_tx( channel );
                // Возврат 0x4000 = PCI429_ERROR_TxR_BUSY
            } while ( 0x4000                                      == err_pci &&
                      ( DateTime.Now - n_time ).TotalMilliseconds < time );
            if ( 0 != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка проверки окончания предыдущей выдачи в интерфейсной плате PCI-429" );
            }
        }

        /// <summary>
        /// Подпрограмма выдачи массива слов
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public void Write( int index, ushort channel, uint[] data, int length ) {
            Validation( index );
            // Выдача данных
            var err_pci = _device[ index ].Write( channel, data, ( uint ) length );
            if ( 0 != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка выдачи массива слов интерфейсной платой PCI-429" );
            }
        }

        /// <summary>
        /// Подпрограмма выдачи массива слов
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public void Write( int index, ushort channel, uint[] data, int length, int time ) {
            int err_pci;

            Validation( index );
            // Цикл  выдачи остальных слов
            var writed = 0;
            do {
                // Рассчет количества выдаваемых данных в данном проходе
                var count = 512;
                if ( 512 > length - writed ) {
                    count = length - writed;
                }
                // Формирование массива выдаваемых данных в данном проходе
                var kern_data = new uint[ count ];
                for ( var i = 0; i < count; i++ ) {
                    kern_data[ i ] = data[ writed + i ];
                }
                // Выдача данных
                var n_time = DateTime.Now;
                do {
                    // Выдача данных
                    err_pci = _device[ index ].check_tx( channel );
                } while ( 0x4000                                      == err_pci &&
                          ( DateTime.Now - n_time ).TotalMilliseconds < time );
                if ( 0 == err_pci ) {
                    // Выдача данных
                    err_pci =  _device[ index ].Write( channel, kern_data, ( uint ) count );
                    writed  += count;
                }
            } while ( length > writed && 0 == err_pci );
            if ( 0 != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка выдачи массива слов интерфейсной платой PCI-429" );
            }
        }

        /// <summary>
        ///  Подпрограмма получения массива слов
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="length"></param>
        /// <param name="time"></param>
        /// <param name="readed"></param>
        public uint[] Read( int index, ushort channel, int length, int time, out int readed ) {
            Validation( index );
            // Список считываемых данных
            var data = new uint[ length ];

            int delta_time;
            readed = 0;
            // Цикл чтения данных из платы PCI-429
            var time_begin = DateTime.Now;
            do {
                int kern_readed;
                var kern_data = new uint[ length - readed ];
                // Вызов функции
                var err_pci = _device[ index ].Read( channel, ref kern_data, length - readed, out kern_readed );
                if ( 0 != err_pci && 0x4001 != err_pci ) {
                    throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                        "Ошибка получения массива слов интерфейсной платой PCI-429" );
                }
                // Сохранение полученных данных
                for ( var i = 0; i < kern_readed; i++ ) {
                    data[ readed + i ] = kern_data[ i ];
                }
                // Обновления счетчика принятых слов
                readed     += kern_readed;
                delta_time =  ( int ) ( DateTime.Now - time_begin ).TotalMilliseconds;
            } while ( delta_time < time && length > readed );
            // Проверка времени выполнения
            if ( length > readed && delta_time >= time ) {
                // Ошибка при чтении из платы 
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    $"Время ожидания {readed:D3} слова в канале {channel,2:D2} интерфейсной платы PCI-429 превышено" );
            }
            // Проверка количества считанных данных
            if ( length > readed ) {
                // Ошибка при чтении из платы 
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    $" Чтение {readed:D3} слова Arinc429 в канале {channel,2:D2} интерфейсной платы PCI-429 завершено с ошибкой" );
            }
            // Вывод принятых данных
            var str = string.Empty;
            App.TaskManager.Log.WriteLineAsync(
                $"Полученные данные по каналу {channel:D2}" +
                Environment.NewLine                         +
                data.Aggregate( str, ( current, j ) => current + $"0x{j:X8} " ) );
            return data;
        }

        /// <summary>
        ///  Подпрограмма получения массива слов
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="readed"></param>
        public uint[] Read( int index, ushort channel, out int readed ) {
            Validation( index );
            // Вызов функции
            readed = 0;
            const int length  = 256;
            var       data    = new uint[ length ];
            var       err_pci = _device[ index ].Read( channel, ref data, length, out readed );
            if ( 0 != err_pci && 0x4001 != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка получения массива слов интерфейсной платой PCI-429" );
            }
            // Вывод принятых данных
            var str = string.Empty;
            App.TaskManager.Log.WriteLineAsync(
                $"Полученные данные по каналу {channel:D2}" +
                Environment.NewLine                         +
                data.Aggregate( str, ( current, j ) => current + $"0x{j:X8} " ) );
            return data;
        }

        /// <summary>
        /// Подпрограмма получения списка устройств
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public int GetDevice( List< ushort > devices ) {
            if ( devices == null ) return 0;
            const int index = 0;
            // Выдача данных
            var err_pci = _device[ index ].GetDevice( devices );
            if ( 0 != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка получения списка интерфейсных плат PCI-429" );
            }
            return devices.Count;
        }
    }
}