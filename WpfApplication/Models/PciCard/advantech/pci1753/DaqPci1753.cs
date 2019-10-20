using Automation.BDaq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.advantech.@abstract;

namespace WpfApplication.Models.PciCard.advantech.pci1753 {
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    public class DaqPci1753 : DaqPciClass {
        /// <summary>
        /// Структура содержащая настройки портов платы
        /// </summary>
        public struct ConfigPort {
            public DioPortDir PortA0;
            public DioPortDir PortB0;
            public DioPortDir PortC0;
            public DioPortDir PortA1;
            public DioPortDir PortB1;
            public DioPortDir PortC1;
            public DioPortDir PortA2;
            public DioPortDir PortB2;
            public DioPortDir PortC2;
            public DioPortDir PortA3;
            public DioPortDir PortB3;
            public DioPortDir PortC3;
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        public DaqPci1753() {
            IsInit    = false;
            ProductId = ProductId.BD_PCI1753;
        }

        public override void Open( string description ) {
            Init( "PCI-1753", description );
        }

        protected override void SafeMode() {
            if ( !IsInit ) {
                return;
            }
            // Задание безопасной конфигурации конфигурации
            var port_dirs = InstantDiCtrl.PortDirection;
            for ( var port = 0; port < ChannelCountMax; port++ ) {
                // В регистры выдачи записываются значения, соответствующие 0В 
                InstantDoCtrl.Write( port, 0 );
                // Все порты настраиваются на ввод
                port_dirs[ port ].Direction = DioPortDir.Input;
            }
        }

        /// <summary>
        /// Задание регистра конфигурации вводов выводов портов
        /// </summary>
        /// <param name="settings">Напрвление</param>
        /// <returns></returns>
        public void Config( ConfigPort settings ) {
            if ( !IsInit ) {
                // Модуль не инициализирован
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    "The object of card Pci1753 is not initialized" );
            }
            // Задание конфигурации 
            var port_dirs = InstantDiCtrl.PortDirection;
            port_dirs[ 0 ].Direction  = settings.PortA0;
            port_dirs[ 1 ].Direction  = settings.PortB0;
            port_dirs[ 2 ].Direction  = settings.PortC0;
            port_dirs[ 3 ].Direction  = settings.PortA1;
            port_dirs[ 4 ].Direction  = settings.PortB1;
            port_dirs[ 5 ].Direction  = settings.PortC1;
            port_dirs[ 6 ].Direction  = settings.PortA2;
            port_dirs[ 7 ].Direction  = settings.PortB2;
            port_dirs[ 8 ].Direction  = settings.PortC2;
            port_dirs[ 9 ].Direction  = settings.PortA3;
            port_dirs[ 10 ].Direction = settings.PortB3;
            port_dirs[ 11 ].Direction = settings.PortC3;
        }

        /// <summary>
        /// Задание регистра конфигурации вводов выводов порта
        /// </summary>
        /// <param name="port">Номер порта</param>
        /// <param name="settings">Напрвление</param>
        /// <returns></returns>
        public void Config( int port, DioPortDir settings ) {
            if ( !IsInit ) {
                // Модуль не инициализирован
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    "The object of card Pci1753 is not initialized" );
            }
            if ( port < 0 || port >= ChannelCountMax ) {
                // Номер канала задан неверно
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    $"The port {port} of card Pci1753 is not valid" );
            }
            // Задание конфигурации
            var port_dirs = InstantDiCtrl.PortDirection;
            port_dirs[ port ].Direction = settings;
        }

        /// <summary>
        /// Подпрограмма записи данных в порт
        /// </summary>
        /// <param name="port">Имя порта</param>
        /// <param name="value">Данные</param>
        /// <returns></returns>
        public void WritePort( int port, byte value ) {
            if ( !IsInit ) {
                // Модуль не инициализирован
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    "The object of card Pci1753 is not initialized" );
            }
            if ( port < 0 || port >= ChannelCountMax ) {
                // Номер канала задан неверно
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    $"The port {port} of card Pci1753 is not valid" );
            }
            var error_code = InstantDoCtrl.Write( port, value );
            if ( error_code != ErrorCode.Success  ) {
                // Возникла ошибка
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    $"Ошибка записи байта данных в порт {port:D} интерфейсной платы PCI-1753 - {error_code}" );
            }
        }

        /// <summary>
        /// Подпрограмма записи данных в порт
        /// </summary>
        /// <param name="port">Имя порта</param>
        /// <param name="bit"></param>
        /// <param name="value">Данные</param>
        /// <returns></returns>
        public void WriteBit( int port, int bit, byte value ) {
            if ( !IsInit ) {
                // Модуль не инициализирован
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    "The object of card Pci1753 is not initialized" );
            }
            if ( port < 0 || port >= ChannelCountMax ) {
                // Номер канала задан неверно
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    $"The port {port} of card Pci1753 is not valid" );
            }
            var error_code = InstantDoCtrl.WriteBit( port, bit, value );
            if ( error_code != ErrorCode.Success  ) {
                // Возникла ошибка
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    $"Ошибка записи бита данных в порт {port:D} интерфейсной платы PCI-1753 - {error_code}" );
            }
        }

        /// <summary>
        /// Подрограмма считывания данных из порта
        /// </summary>
        /// <param name="port">Имя порта</param>
        /// <returns></returns>
        public byte ReadPort( int port ) {
            byte value;
            if ( !IsInit ) {
                // Модуль не инициализирован
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    "The object of card Pci1753 is not initialized" );
            }
            if ( port < 0 || port >= ChannelCountMax ) {
                // Номер канала задан неверно
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    $"The port {port} of card Pci1753 is not valid" );
            }
            var error_code = InstantDiCtrl.Read( port, out value );
            if ( error_code != ErrorCode.Success  ) {
                // Возникла ошибка
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    $"Ошибка чтения байта данных из порта {port:D} интерфейсной платы PCI-1753 - {error_code}" );
            }
            return value;
        }

        /// <summary>
        /// Подрограмма считывания данных из порта
        /// </summary>
        /// <param name="port">Имя порта</param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public byte ReadBit( int port, int bit ) {
            byte value;
            if ( !IsInit ) {
                // Модуль не инициализирован
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    "The object of card Pci1753 is not initialized" );
            }
            if ( 0 > port || ChannelCountMax <= port ) {
                // Номер канала задан неверно
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    $"The port {port} of card Pci1753 is not valid" );
            }
            var error_code = InstantDiCtrl.ReadBit( port, bit, out value );
            if ( error_code != ErrorCode.Success  ) {
                // Возникла ошибка
                throw new Exception< Pci1753ExceptionArgs >( new Pci1753ExceptionArgs( 0 ),
                    $"Ошибка чтения бита данных из порта {port:D} интерфейсной платы PCI-1753 - {error_code}" );
            }
            return value;
        }
    }
}