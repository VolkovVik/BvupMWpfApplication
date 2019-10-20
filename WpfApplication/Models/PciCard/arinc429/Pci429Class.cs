using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace WpfApplication.Models.PciCard.arinc429 {
    /// <summary>
    /// Частоты работы каналов
    /// OFF - выкл.
    /// F12 - 12.5 кГц
    /// F50 - 50 кГц
    /// F100 - 100 кГц
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public enum FREQ : ushort {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        OFF = 0,
        F12 = 12,

        // ReSharper disable once UnusedMember.Global
        F50  = 50,
        F100 = 100
    };

    /// <summary>
    /// Динамический вызов неуправляемой DLL из .NET
    /// </summary>
    internal static class NativeMethods {
        /// <summary>
        /// Подпрограмма получения указателя на сборку .dll
        /// </summary>
        /// <param name="nameDll"></param>
        /// <returns></returns>
        [DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
        internal static extern IntPtr LoadLibrary( string nameDll );

        /// <summary>
        /// Подпрограмма получения точки входа сборки .dll
        /// </summary>
        /// <param name="hModule"></param>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        [DllImport( "kernel32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true )]
        //[DllImport("kernel32.dll")]
        internal static extern IntPtr GetProcAddress( IntPtr hModule, string procedureName );

        /// <summary>
        /// Подпрограмма получения освобождения сборки .dll
        /// </summary>
        /// <param name="hModule"></param>
        /// <returns></returns>
        [DllImport( "kernel32.dll" )]
        internal static extern bool FreeLibrary( IntPtr hModule );
    }

    public class Pci429Class : IDisposable {
        /// <summary>
        /// Описание платы
        /// </summary>
        private DescriptionPci4294 _device;

        /// <summary>
        /// Имя сборки .DLL
        /// </summary>
        private const string NameDll = "PCI429_4.dll";

        /// <summary>
        /// Семафор
        /// </summary>
        /// <remarks>
        /// Instantiate a Singleton of the Semaphore with a value of 1. 
        /// This means that only 1 thread can be granted access at a time.
        /// https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
        /// Synchronously wait to enter the Semaphore. 
        /// If no-one has been granted access to the Semaphore, 
        /// code execution will proceed, otherwise this thread 
        /// waits here until the semaphore is released 
        /// _semaphoreSlim.Wait();
        /// When the task is ready, release the semaphore. 
        /// It is vital to ALWAYS release the semaphore when we are ready, 
        /// or else we will end up with a Semaphore that is forever locked.
        /// This is why it is important to do the Release within a try...finally clause; 
        /// program execution may crash or take a different path, 
        /// this way you are guaranteed execution
        /// _semaphoreSlim.Release();
        /// </remarks>
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim( 1, 1 );

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        // ReSharper disable once RedundantDefaultMemberInitializer
        private bool _disposed = false;

        /// <summary>
        /// Описание платы PCI429_4
        /// </summary>
        [StructLayout( LayoutKind.Sequential )]
        private struct DescriptionPci4294 {
            /// <summary>
            /// Указатель на плату
            /// </summary>
            public readonly IntPtr handle;

            /// <summary>
            /// Идентификатор модуля
            /// </summary>
            private readonly ushort id;

            /// <summary>
            /// Количество входных каналов
            /// </summary>
            private readonly ushort icn;

            /// <summary>
            /// Количество выходных каналов
            /// </summary>
            private readonly ushort ocn;

            // Сообщаем маршалингу, что на этом месте структуры должны быть не
            // указатель на массив, а его 16 элементов. Однако это не освобождает
            // нас от выделения памяти далее с помощью команды new...
            /// <summary>
            /// Служебный массив указателей.
            /// </summary>
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 16 )]
            private readonly ushort[] last_read_ptr;

            /// <summary>
            /// Массив частот и режимов контроля чётности для вх. и вых. каналов
            /// </summary>
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
            private readonly ushort[] freq;
        };

        /// <remark>
        /// Основные коды ошибок при работе с PCI429_4. 
        /// Остальные коды аналогичны кодам ошибок Windows.
        /// Успех                                                         = 0
        /// Не правильный указатель на плату                              = 6
        /// Недостаточно памяти для выполнения операции                   = 8
        /// Один или более параметров функции имеют недопустимое значение = 87
        /// Буфер выдачи не готов                                         = 0x4000
        /// Нет новых данных                                              = 0x4001
        /// Ошибка обмена с драйвером                                     = 0x4002
        /// Неправильный указатель на плату                               = 0xFFFFFFFF
        /// 
        /// https://blogs.msdn.microsoft.com/jonathanswift/2006/10/03/dynamically-calling-an-unmanaged-dll-from-net-c/
        /// см. маршаллинг аргументов https://docs.microsoft.com/ru-ru/cpp/dotnet/calling-native-functions-from-managed-code?view=vs-2017
        /// При возникновении ошибки компилятора error CS0103: The name '$exception' does not exist in the current context
        /// в исключении PInvokeStackImbalance данное исключение следует запретить ( снять галку )
        /// 
        /// </remark>
        /// <summary>
        /// Handle DLL
        /// </summary>
        private IntPtr _dllhandle = IntPtr.Zero;

        private OpenDelegate      _open;
        private CloseDelegate     _close;
        private StartDelegate     _start;
        private ResetDelegate     _reset;
        private ConfigDelegate    _config;
        private ReadDelegate      _read;
        private WriteDelegate     _write;
        private CheckTxDelegate   _checkTx;
        private GetDeviceDelegate _getDeviceList;

        /// <summary>
        /// Delegate with function signature for the Open function
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer( CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.I4 )]
        private delegate int OpenDelegate(
            [Out] ushort sn,
            [Out] out DescriptionPci4294 device );

        /// <summary>
        /// Delegate with function signature for the Close function
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer( CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.U4 )]
        private delegate int CloseDelegate(
            [Out] [In] ref DescriptionPci4294 device );

        /// <summary>
        /// Delegate with function signature for the Start function
        /// </summary>
        /// <param name="device"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer( CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.U4 )]
        private delegate int StartDelegate(
            [Out] [In] ref DescriptionPci4294 device,
            [Out] ushort mode );

        /// <summary>
        ///  Delegate with function signature for the Reset function
        /// </summary>
        /// <param name="device"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer( CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.U4 )]
        private delegate int ResetDelegate(
            [Out] [In] ref DescriptionPci4294 device,
            [Out] out ushort id );

        /// <summary>
        /// Delegate with function signature for the Config function
        /// </summary>
        /// <param name="device"></param>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        /// <param name="freq"></param>
        /// <param name="parityOff"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer( CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.U4 )]
        private delegate int ConfigDelegate(
            [Out] [In] ref DescriptionPci4294 device,
            [Out] byte type,
            [Out] byte channel,
            [Out] FREQ freq,
            [Out] bool parityOff );

        /// <summary>
        /// Delegate with function signature for the Read function
        /// </summary>
        /// <param name="device"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer( CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.U4 )]
        private delegate int ReadDelegate(
            [Out] [In] ref DescriptionPci4294 device,
            [Out]  ushort channel,
            [Out] [In] IntPtr data,
            [Out] [In] ref uint length );

        /// <summary>
        /// Delegate with function signature for the Write function
        /// </summary>
        /// <param name="device"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer( CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.U4 )]
        private delegate int WriteDelegate(
            [Out] [In] ref DescriptionPci4294 device,
            [Out] ushort channel,
            [Out] IntPtr data,
            [Out] uint length );

        /// <summary>
        /// Delegate with function signature for the CheckTx function
        /// </summary>
        /// <param name="device"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer( CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.U4 )]
        private delegate int CheckTxDelegate(
            [Out] [In] ref DescriptionPci4294 device,
            [Out] ushort channel );

        /// <summary>
        /// Delegate with function signature for the GetDevices function
        /// </summary>
        /// <param name="devices"></param>
        /// <param name="readed"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer( CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.U4 )]
        private delegate int GetDeviceDelegate(
            [Out] [In] IntPtr devices,
            [Out] [In] ref uint readed
        );

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Pci429Class() {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose( false );
        }

        /// <summary>
        /// Подпрограмма выполняющая определяемые приложением задачи, 
        /// связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose() {
            // Implement IDisposable.
            // Do not make this method virtual.
            // A derived class should not be able to override this method.
            Dispose( true );
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize( this );
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose( bool disposing ) {
            // Check to see if Dispose has already been called.
            if ( _disposed ) {
                return;
            }
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if ( disposing ) {
                // Dispose managed resources.
                _dllhandle     = IntPtr.Zero;
                _open          = null;
                _close         = null;
                _start         = null;
                _reset         = null;
                _config        = null;
                _read          = null;
                _write         = null;
                _checkTx       = null;
                _getDeviceList = null;
            }
            // Dispose native ( unmanaged ) resources, if exits

            // Note disposing has been done.
            _disposed = true;
        }

        /// <summary>
        /// Подпрограмма открытия платы
        /// </summary>
        /// <param name="serialNumber"></param>
        public int Open( ushort serialNumber ) {
            // Выделение памяти в неуправляемой памяти процесса
            // var pnt = Marshal.AllocHGlobal( Marshal.SizeOf( zzz ) );
            // Копирование структуры zzz в неуправляемую память
            // Marshal.StructureToPtr( zzz, pnt, false );
            // Возврат структуры из неупраяляемой памяти
            // var zzz1 = ( pci429_4_tag) Marshal.PtrToStructure( pnt, typeof( pci429_4_tag ) );
            // Освобождение неуправляемой памяти
            // Marshal.FreeHGlobal( pnt );

            var error = 0;
            // Synchronously wait to enter the Semaphore. 
            SemaphoreSlim.Wait();
            // Get handle to .dll file
            _dllhandle = NativeMethods.LoadLibrary( NameDll );
            if ( _dllhandle == IntPtr.Zero ) {
                // Handle error loading
                error = 1;
            } else {
                // Get handle to Open method in .dll file
                var open_handle = NativeMethods.GetProcAddress( _dllhandle, "pci429_4_open" );
                if ( open_handle == IntPtr.Zero ) {
                    error = 2;
                } else {
                    _open = ( OpenDelegate ) Marshal.GetDelegateForFunctionPointer(
                        open_handle, typeof( OpenDelegate ) );
                    if ( _open == null ) {
                        error = 3;
                    }
                }
                // Get handle to Close method in .dll file
                var close_handle = NativeMethods.GetProcAddress( _dllhandle, "pci429_4_close" );
                if ( close_handle == IntPtr.Zero ) {
                    error = 4;
                } else {
                    _close = ( CloseDelegate ) Marshal.GetDelegateForFunctionPointer( close_handle,
                        typeof( CloseDelegate ) );
                    if ( _close == null ) {
                        error = 5;
                    }
                }
                // Get handle to Start method in .dll file
                var start_handle = NativeMethods.GetProcAddress( _dllhandle, "pci429_4_start" );
                if ( start_handle == IntPtr.Zero ) {
                    error = 6;
                } else {
                    _start = ( StartDelegate ) Marshal.GetDelegateForFunctionPointer( start_handle,
                        typeof( StartDelegate ) );
                    if ( _start == null ) {
                        error = 7;
                    }
                }
                // Get handle to Reset method in .dll file
                var reset_handle = NativeMethods.GetProcAddress( _dllhandle, "pci429_4_reset" );
                if ( reset_handle == IntPtr.Zero ) {
                    error = 8;
                } else {
                    _reset = ( ResetDelegate ) Marshal.GetDelegateForFunctionPointer( reset_handle,
                        typeof( ResetDelegate ) );
                    if ( _reset == null ) {
                        error = 9;
                    }
                }
                // Get handle to Config method in .dll file
                var config_handle = NativeMethods.GetProcAddress( _dllhandle, "pci429_4_set_freq" );
                if ( config_handle == IntPtr.Zero ) {
                    error = 10;
                } else {
                    _config = ( ConfigDelegate ) Marshal.GetDelegateForFunctionPointer( config_handle,
                        typeof( ConfigDelegate ) );
                    if ( _config == null ) {
                        error = 11;
                    }
                }
                // Get handle to Read method in .dll file
                var read_handle = NativeMethods.GetProcAddress( _dllhandle, "pci429_4_read_rx" );
                if ( read_handle == IntPtr.Zero ) {
                    error = 12;
                } else {
                    _read = ( ReadDelegate ) Marshal.GetDelegateForFunctionPointer( read_handle,
                        typeof( ReadDelegate ) );
                    if ( _read == null ) {
                        error = 13;
                    }
                }
                // Get handle to Write method in .dll file
                var write_handle = NativeMethods.GetProcAddress( _dllhandle, "pci429_4_write_tx" );
                if ( write_handle == IntPtr.Zero ) {
                    error = 14;
                } else {
                    _write = ( WriteDelegate ) Marshal.GetDelegateForFunctionPointer( write_handle,
                        typeof( WriteDelegate ) );
                    if ( _write == null ) {
                        error = 15;
                    }
                }
                // Get handle to CheckTx method in .dll file
                var check_tx_handle = NativeMethods.GetProcAddress( _dllhandle, "pci429_4_check_tx" );
                if ( check_tx_handle == IntPtr.Zero ) {
                    error = 16;
                } else {
                    _checkTx = ( CheckTxDelegate ) Marshal.GetDelegateForFunctionPointer( check_tx_handle,
                        typeof( CheckTxDelegate ) );
                    if ( _checkTx == null ) {
                        error = 17;
                    }
                }
                // Get handle to GetDevice method in .dll file
                var get_device_handle = NativeMethods.GetProcAddress( _dllhandle, "pci429_4_get_dev_list" );
                if ( get_device_handle == IntPtr.Zero ) {
                    error = 18;
                } else {
                    _getDeviceList = ( GetDeviceDelegate ) Marshal.GetDelegateForFunctionPointer( get_device_handle,
                        typeof( GetDeviceDelegate ) );
                    if ( _getDeviceList == null ) {
                        error = 19;
                    }
                }
                // Открытие устройства
                error += _open?.Invoke( serialNumber, out _device ) ?? int.MaxValue;
                if ( _device.handle == ( IntPtr ) 0 ) {
                    error = 18;
                }
            }
            // Release the Semaphore.
            SemaphoreSlim.Release();
            return error;
        }

        /// <summary>
        /// Подпрограмма завершения работы с платой
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public int Close() {
            // Synchronously wait to enter the Semaphore. 
            SemaphoreSlim.Wait();
            var error = _close?.Invoke( ref _device ) ?? int.MaxValue;
            if ( _dllhandle != IntPtr.Zero ) {
                // Free resources. 
                // Probably should use SafeHandle or some similar class, but this will do for now.
                NativeMethods.FreeLibrary( _dllhandle );
            }
            // Release the Semaphore.
            SemaphoreSlim.Release();
            return error;
        }

        /// <summary>
        /// Подпрограмма запуска платы и перехода в рабочий режим
        /// </summary>
        /// <param name="mode">Командное слово режима. Значение по умолчанию 0</param>
        /// <returns></returns>
        public int Start( ushort mode = 0 ) {
            // Synchronously wait to enter the Semaphore. 
            SemaphoreSlim.Wait();
            var error = _start?.Invoke( ref _device, mode ) ?? int.MaxValue;
            // Release the Semaphore.
            SemaphoreSlim.Release();
            return error;
        }

        /// <summary>
        /// Подпрограмма сброса платы
        /// </summary>
        /// <returns>Код ошибки</returns>
        public int Reset() {
            // Synchronously wait to enter the Semaphore. 
            SemaphoreSlim.Wait();
            ushort id;
            var    error = _reset?.Invoke( ref _device, out id ) ?? int.MaxValue;
            // Release the Semaphore.
            SemaphoreSlim.Release();
            return error;
        }

        /// <summary>
        /// Подпрограмма задания частоты работы канала ПБК и режима четности
        /// </summary>
        /// <param name="type">1-входной/0-выходной канал</param>
        /// <param name="channel">Номер канала для выдачи (допустимые значения от 1 до 16)</param>
        /// <param name="freq">Частота работы канала</param>
        /// <param name="parityOff">Контролировать/не контролировать бит четности</param>
        /// <returns></returns>
        public int Config( byte type, byte channel, FREQ freq, bool parityOff = false ) {
            // Synchronously wait to enter the Semaphore. 
            SemaphoreSlim.Wait();
            var error = _config?.Invoke( ref _device, type, channel, freq, parityOff ) ?? int.MaxValue;
            // Release the Semaphore.
            SemaphoreSlim.Release();
            return error;
        }

        /// <summary>
        /// Подпрограмма проверки окончания предыдущей выдачи массива слов.
        /// </summary>
        /// <param name="channel">Номер канала для выдачи (допустимые значения от 1 до 16)</param>
        public int check_tx( ushort channel ) {
            // Synchronously wait to enter the Semaphore. 
            SemaphoreSlim.Wait();
            var error = _checkTx?.Invoke( ref _device, channel ) ?? int.MaxValue;
            // Release the Semaphore.
            SemaphoreSlim.Release();
            return error;
        }

        /// <summary>
        /// Подпрограмма выдачи данных
        /// </summary>
        /// <param name="channel">Номер канала для выдачи (допустимые значения от 1 до 16)</param>
        /// <param name="data">Массив выдаваемых данных</param>
        /// <param name="length">Количество выдаваемых слов (допустимые значения от 1 до 512)</param>
        /// <returns></returns>
        public int Write( ushort channel, uint[] data, uint length ) {
            // Synchronously wait to enter the Semaphore. 
            SemaphoreSlim.Wait();
            var gch   = GCHandle.Alloc( data, GCHandleType.Pinned );
            var error = _write?.Invoke( ref _device, channel, gch.AddrOfPinnedObject(), length ) ?? int.MaxValue;
            gch.Free();
            // Release the Semaphore.
            SemaphoreSlim.Release();
            return error;
        }

        /// <summary>
        /// Подпрограмма считывания принятых данных
        /// </summary>
        /// <param name="channel">Номер канала для выдачи (допустимые значения от 1 до 16).</param>
        /// <param name="data">Массив считанных данных</param>
        /// <param name="length">Количество считываемых слов (допустимые значения от 1 до 256).</param>
        /// <param name="readed">Количество считанных слов</param>
        /// <returns></returns>
        public int Read( ushort channel, ref uint[] data, int length, out int readed ) {
            // Synchronously wait to enter the Semaphore. 
            SemaphoreSlim.Wait();
            // Запращиваемое количество данных
            var kern_readed = ( uint ) length;
            var gch         = GCHandle.Alloc( data, GCHandleType.Pinned );
            var error = _read?.Invoke( ref _device, channel, gch.AddrOfPinnedObject(), ref kern_readed ) ??
                        int.MaxValue;
            gch.Free();
            readed = ( int ) kern_readed;
            // Release the Semaphore.
            SemaphoreSlim.Release();
            return error;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public int GetDevice( List< ushort > devices ) {
            if ( devices == null ) return 0;
            // Synchronously wait to enter the Semaphore. 
            SemaphoreSlim.Wait();

            // Получение данных
            var data   = new ushort[ 256 ];
            var readed = ( uint ) ( Marshal.SizeOf( data[ 0 ].GetType() ) * data.Length );
            var gch    = GCHandle.Alloc( data, GCHandleType.Pinned );
            var error  = _getDeviceList?.Invoke( gch.AddrOfPinnedObject(), ref readed ) ?? int.MaxValue;
            gch.Free();
            readed /= ( uint ) Marshal.SizeOf( data[ 0 ].GetType() );
            // Парсинг данных
            for ( var i = 0; i < readed; i++ ) {
                devices.Add( data[ i ] );
            }
            // Release the Semaphore.
            SemaphoreSlim.Release();
            return error;
        }
    }
}