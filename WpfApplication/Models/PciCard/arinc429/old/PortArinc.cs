using System;
using System.Runtime.InteropServices;
using System.Security;
using WpfApplication.Models.Common;
using WpfApplication.Models.Main;
using WpfApplication.Models.Task;

namespace WpfApplication.Models.PciCard.arinc429 {
    /// <summary>
    /// Класс UnsafeNativeMethods — этот класс подавляет проверку стека на разрешение 
    /// неуправляемого кода. 
    /// (к классу применен System.Security.SuppressUnmanagedCodeSecurityAttribute.) 
    /// Класс предназначен для методов, которые являются потенциально опасными. 
    /// Все вызывающие их методы должны пройти полную проверку безопасности использования, 
    /// поскольку проверка стека не выполняется.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods {
        /// <summary>
        /// /Подпрограмма загрузки DLL библиотеки
        /// </summary>
        /// <param name="dllName"></param>
        /// <returns></returns>
        [DllImport( "kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false )]
        internal static extern IntPtr
            LoadLibrary( [MarshalAs( UnmanagedType.BStr )] string dllName = "PCI429_4.dll" );
    }

    /// <summary>
    /// Класс, определяющий функции для работы с имитатором
    /// </summary>
    public class PortArinc {
        /// <summary>
        /// Количество объектов плата PCI-1753
        /// </summary>
        private const uint MaxDevice = 2;

        /// <summary>
        /// Массив объектов плат daq_pci1753
        /// </summary>
        private readonly GPCI429[] _device;

        /// <summary>
        /// Признак инициализации модуля имитатора
        /// </summary>
        private bool _init;

        /// <summary>
        /// Свойство инициализации модуля
        /// </summary>
        public bool IsInit => _init;

        /// <summary>
        /// Конструктор
        /// </summary>
        public PortArinc() {
            _device = new GPCI429[MaxDevice];
            // Открытие плат PCI1753
            Open();
        }

        /// <summary>
        /// Подпрограмма инициализации драйвера платы PCI429
        /// </summary>
        /// <returns></returns>
        private void Open() {
            // Установка признака инициализации
            _init = true;

            var str_att = new[] {XmlSettings.Pci429Card1Element, XmlSettings.Pci429Card2Element};
            // Проверка наличия библиотеки
            var user_api = UnsafeNativeMethods.LoadLibrary();
            if ( IntPtr.Zero == user_api ) {
                // Сборка DLL не найдена
                _init = false;
                App.MyWork.LogFileData +=
                    "Библиотека DLL PCI429_4.dll не найдена" + Environment.NewLine;
                CommonClass.set_result_text( "Инициализация плат PCI-429", 1, 45, false, visible: true );
            } else {
                // Инициализация плат
                for ( var i = 0; i < MaxDevice; i++ ) {
                    var result         = false;
                    var address_device = 0U;
                    var description    = $"PCI-429,ADR#{address_device:D5}";
                    // Сдвиг шкалы ProgressBar
                    App.MyWindows.ValueProgress++;
                    // Считывание номера платы PCI429 из .xml файла 
                    XmlSettings.Read( XmlSettings.XmlCfgKtaName, str_att[ i ], XmlSettings.Pci429AttSn,
                        out address_device );
                    if ( uint.MaxValue == address_device ) {
                        App.MyWork.LogFileData +=
                            "Ошибка чтения номера платы PCI429 из файла настроек" + Environment.NewLine;
                    } else {
                        try {
                            // Инициализация драйвера
                            // Создание объекта платы PCI429  
                            address_device &= 0xFFFF;
                            _device[ i ]   =  new GPCI429( ( ushort ) address_device );
                            if ( pci429_4_errors.INVALID_HANDLE_VALUE != _device[ i ].handle ) {
                                result = true;
                            } else {
                                // Указатель на плату не получен
                                // Плата не инициализирована
                                App.MyWork.LogFileData +=
                                    $"Ошибка открытия 0х{_device[ i ].handle:X8}" + Environment.NewLine;
                            }
                        }
                        catch ( DllNotFoundException exc ) {
                            // The exception that is thrown when a DLL 
                            // specified in a DLL import cannot be found.
                            // DllNotFoundException uses the HRESULT_COR_E_DLLNOTFOUND,
                            // which has the value 0x80131524.
                            App.MyWork.LogFileData += exc.Message + Environment.NewLine;
                        }
                        catch ( EntryPointNotFoundException exc ) {
                            // The exception that is thrown 
                            // when an attempt to load a class fails 
                            // due to the absence of an entry method. 
                            // The EntryPointNotFoundException type 
                            // exposes the following members.
                            App.MyWork.LogFileData += exc.Message + Environment.NewLine;
                        }
                    }
                    if ( !result ) {
                        // Плата не инициализирована
                        // Сброс признака инициализации модуля
                        _init = false;
                    }
                    // Возврат результата
                    CommonClass.set_result_text( $"Инициализация платы {description}", 1, 45, result, visible: true );
                }
            }
        }

        /// <summary>
        ///  Подпрограмма прекращения работы с драйвером платы PCI429
        /// </summary>
        public void Close() {
            // Сброс признака инициализации 
            _init = false;
            // Закрытие плат
            for ( var i = 0; i < MaxDevice; i++ ) {
                if ( !Equals( null, _device[ i ] ) &&
                     pci429_4_errors.INVALID_HANDLE_VALUE != _device[ i ].handle ) {
                    _device[ i ].close();
                }
            }
        }

        /// <summary>
        /// Подпрограмма проверки допустимости индекса порта
        /// </summary>
        /// <returns></returns>
        private void check_index( int index ) {
            if ( !_init ) {
                // Модуль не инициализирован 
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "The modul of PCI429 is not initialized" );
            }
            if ( 0 > index || MaxDevice <= index ) {
                // Индекс неверный
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "The index cars PCI-429 is not valid" );
            }
            // Проверка существования объетка
            if ( _device[ index ] == null ) {
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( index ),
                    $"The object of card PCI-429 #{index} is not creatе" );
            }
            // Проверка инициализации объекта платы
            if ( pci429_4_errors.INVALID_HANDLE_VALUE != _device[ index ].handle ) {
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( index ),
                    $"The object of card PCI-429 #{index} is not initialized" );
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
        public void set_freq( int index, byte type, byte nk, FREQ freq, bool parityOff ) {
            check_index( index );
            var err_pci = _device[ index ].set_freq( type, nk, freq, parityOff );
            if ( pci429_4_errors.ERROR_SUCCESS != err_pci ) {
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
            check_index( index );
            var err_pci = _device[ index ].start();
            if ( pci429_4_errors.ERROR_SUCCESS != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка запуска интерфейсной платы PCI-429" );
            }
        }

        /// <summary>
        /// Сброс модуля
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void Reset( int index = 0 ) {
            check_index( index );
            var err_pci = _device[ index ].reset();
            if ( pci429_4_errors.ERROR_SUCCESS != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка сброса интерфейсной платы PCI-429" );
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
            uint err_pci;

            check_index( index );
            // Ожидание окончания предыдущей выдачи
            var n_time = DateTime.Now;
            do {
                err_pci = _device[ index ].check_tx( channel );
            } while (  pci429_4_errors.PCI429_ERROR_TxR_BUSY == err_pci &&
                       time                                  > ( DateTime.Now - n_time ).TotalMilliseconds );
            if ( pci429_4_errors.ERROR_SUCCESS != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка проверки окончания предыдущей выдачи в интерфейсной плате PCI-429" );
            }
        }

        /// <summary>
        /// Подпрограмма выдачи массива слов
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="buf"></param>
        /// <param name="bufSize"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public void Write( int index, ushort channel, uint[] buf, uint bufSize, int time = 0 ) {
            uint err_pci;

            check_index( index );
            // Цикл  выдачи остальных слов
            uint writed = 0;
            do {
                // Рассчет количества выдаваемых данных в данном проходе
                uint count = 512;
                if ( 512 > ( bufSize - writed ) ) {
                    count = bufSize - writed;
                }
                // Формирование массива выдаваемых данных в данном проходе
                var data = new uint[count];
                for ( var i = 0; i < count; i++ ) {
                    data[ i ] = buf[ writed + i ];
                }
                // Выдача данных
                var n_time = DateTime.Now;
                do {
                    // Выдача данных
                    err_pci = _device[ index ].check_tx( channel );
                } while ( pci429_4_errors.PCI429_ERROR_TxR_BUSY == err_pci &&
                          time                                  > ( DateTime.Now - n_time ).TotalMilliseconds );
                if ( pci429_4_errors.ERROR_SUCCESS == err_pci ) {
                    // Выдача данных
                    err_pci =  _device[ index ].write_tx( channel, data, count );
                    writed  += count;
                }
            } while ( bufSize > writed && pci429_4_errors.ERROR_SUCCESS == err_pci );
            if ( pci429_4_errors.ERROR_SUCCESS != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка выдачи массива слов интерфейсной платой PCI-429" );
            }
        }

        /// <summary>
        /// Подпрограмма получения массива слов
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public void Read( int index, ushort channel, out uint word ) {
            word = uint.MaxValue;
            check_index( index );
            // Вызов функции
            var err_pci = _device[ index ].read_rx( channel, out word );
            if ( pci429_4_errors.ERROR_SUCCESS != err_pci ) {
                throw new Exception< Pci429ExceptionArgs >( new Pci429ExceptionArgs( index ),
                    "Ошибка получения массива слов интерфейсной платой PCI-429" );
            }
        }
    }
}