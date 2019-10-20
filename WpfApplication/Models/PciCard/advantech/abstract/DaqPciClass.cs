using System;
using System.Linq;
using Automation.BDaq;
using WpfApplication.Models.Main;

namespace WpfApplication.Models.PciCard.advantech.@abstract {
    /// <inheritdoc />
    /// <summary>
    /// Абстрактный класс реализации базового класса интерфейсной платы
    /// </summary>
    public abstract class DaqPciClass : IDisposable {
        /// <summary>
        /// Свойство инициализация модуля
        /// </summary>
        public bool IsInit { get; protected set; }

        /// <summary>
        /// Максимальное количество каналов платы 
        /// </summary>
        public int ChannelCountMax;

        /// <summary>
        /// Идентификатор интерфейсной платы
        /// </summary>
        protected ProductId ProductId { private get; set; }

        /// <summary>
        /// Объект чтения аналоговых сигналов
        /// </summary>
        protected readonly InstantAiCtrl InstantAiContrl;

        /// <summary>
        /// Объект записи аналоговых сигналов
        /// </summary>
        protected readonly InstantAoCtrl InstantAoContrl;

        /// <summary>
        /// Базовый класс работы с разовыми командами приема/передачи
        /// </summary>
        private readonly DioCtrlBase _baseDioCtrl;

        /// <summary>
        /// Объект чтения разовых команд
        /// </summary>
        protected readonly InstantDiCtrl InstantDiCtrl;

        /// <summary>
        /// Объект записи разовых команд
        /// </summary>
        protected readonly InstantDoCtrl InstantDoCtrl;

        /// <summary>
        /// Настройка канала по умолчанию
        /// </summary>
        protected ValueRange DefaultValueRange;

        /// <summary>
        /// 
        /// </summary>
        protected DaqPciClass() {
            // Step 1:
            // Create a 'InstantAiCtrl' for Instant AI function.
            InstantAiContrl = new InstantAiCtrl();
            // Create a 'InstantAoCtrl' for Instant AO function.
            InstantAoContrl = new InstantAoCtrl();
            // DioCtrlBase class used for digital input(DI),digital input 
            // related interrupt,digital output operations(DO);
            _baseDioCtrl = new DioCtrlBase();
            // Create a 'InstantDiCtrl' for DI function.
            InstantDiCtrl = new InstantDiCtrl();
            // Create a 'InstantDoCtrl' for DO function.
            InstantDoCtrl = new InstantDoCtrl();
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~DaqPciClass() {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose( false );
        }

        /// <inheritdoc />
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
        /// <remarks>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </remarks>
        private void Dispose( bool disposing ) {
            // Check to see if Dispose has already been called.
            if ( _disposed ) {
                return;
            }
            // If disposing equals true, dispose all managed and unmanaged resources.
            if ( disposing ) {
                // Dispose managed resources.
                // Step 4: Close device and release any allocated resource.
                InstantAiContrl.Dispose();
                InstantAoContrl.Dispose();
                InstantDiCtrl.Dispose();
                InstantDoCtrl.Dispose();
                _baseDioCtrl.Dispose();
            }
            // Dispose native ( unmanaged ) resources, if exits
            // Note disposing has been done.
            _disposed = true;
        }

        /// <summary>
        /// Подпрограмма открытия интерфейсной платы
        /// </summary>
        /// <param name="description"></param>
        public abstract void Open( string description );

        /// <summary>
        /// Установка платы в безопасное состояние
        /// </summary>
        /// <returns></returns>
        protected abstract void SafeMode();

        /// <summary>
        /// Подпрограмма открытия интерфейсной платы
        /// </summary>
        public void Close() {
            if ( IsInit ) return;
            // Задание безопасной конфигурации конфигурации
            SafeMode();
            // Прекращение доступа к устройству
            IsInit = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        protected void Init( string name, string description ) {
            IsInit = false;
            try {
                // Step 2: Select a device by device number or device description and specify the access mode.
                // in this example we use AccessWriteWithReset(default) mode so that we can 
                // fully control the device, including configuring, sampling, etc.
                // Step 3: Read samples and do post-process, we show data here.
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch ( ProductId ) {
                    case ProductId.BD_PCI1716:
                    case ProductId.BD_PCI1747: {
                        // Определение интерфейсной карты по описанию
                        var card = InstantAiContrl.SupportedDevices.FirstOrDefault( o => description == o.Description );
                        if ( card.Description == null ) return;
                        // Получение свойств выбранной интерфейсной платы
                        InstantAiContrl.SelectedDevice = new DeviceInformation( card.Description );
                        // Проверка типа интерфейсной платы
                        if ( InstantAiContrl.ProductId != ProductId ) return;
                        // Получение количества каналов
                        ChannelCountMax = InstantAiContrl.Features.ChannelCountMax;
                        break;
                    }
                    case ProductId.BD_PCI1721:
                    case ProductId.BD_PCI1724: {
                        // Определение интерфейсной карты по описанию
                        var card = InstantAoContrl.SupportedDevices.FirstOrDefault( o => description == o.Description );
                        if ( card.Description == null ) return;
                        // Получение свойств выбранной интерфейсной платы
                        InstantAoContrl.SelectedDevice = new DeviceInformation( card.Description );
                        // Проверка типа интерфейсной платы
                        if ( InstantAoContrl.ProductId != ProductId ) return;
                        ChannelCountMax = InstantAoContrl.Features.ChannelCountMax;
                        break;
                    }
                    case ProductId.BD_PCI1753: {
                        // Определение интерфейсной карты по описанию
                        var card = _baseDioCtrl.SupportedDevices.FirstOrDefault( o => description == o.Description );
                        if ( card.Description == null ) return;
                        // Получение свойств выбранной интерфейсной платы
                        InstantDiCtrl.SelectedDevice = new DeviceInformation( card.Description );
                        InstantDoCtrl.SelectedDevice = new DeviceInformation( card.Description );
                        // Проверка типа интерфейсной платы
                        if ( InstantDiCtrl.ProductId != ProductId.BD_PCI1753 ||
                             InstantDoCtrl.ProductId != ProductId.BD_PCI1753 ) return;
                        ChannelCountMax = InstantDiCtrl.Features.PortCount;
                        break;
                    }
                    default: {
                        throw new ArgumentOutOfRangeException( nameof( ProductId ), ProductId,
                            @"Тип интерфейсной платы неизвестен" );
                    }
                }
                // Модуль работы с интерфейсной платой инициализирован
                IsInit = true;
                // Установка безопасного режима
                SafeMode();
            }
            catch ( ArgumentNullException exc ) {
                // The exception that is thrown when a null reference is passed
                // to a method that does not accept it as a valid argument. 
                throw new Exception< AdvantechPciExceptionArgs >(
                    new AdvantechPciExceptionArgs( 0, name ),
                    $"Ошибка инициализации платы {description}", exc );
            }
            catch ( InvalidOperationException exc ) {
                // The exception that is thrown when a method call 
                // is invalid for the object's current state.
                throw new Exception< AdvantechPciExceptionArgs >(
                    new AdvantechPciExceptionArgs( 0, name ),
                    $"Ошибка инициализации платы {description}", exc );
            }
            catch ( DllNotFoundException exc ) {
                // The exception that is thrown when 
                // a DLL specified in a DLL import cannot be found
                throw new Exception< AdvantechPciExceptionArgs >(
                    new AdvantechPciExceptionArgs( 0, name ),
                    $"Ошибка инициализации платы {description}", exc );
            }
        }
    }
}