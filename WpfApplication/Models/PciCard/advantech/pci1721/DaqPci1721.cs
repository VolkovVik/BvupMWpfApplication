using Automation.BDaq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.advantech.@abstract;

namespace WpfApplication.Models.PciCard.advantech.pci1721 {
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    public class DaqPci1721 : DaqPciClass {
        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        public DaqPci1721() {
            IsInit            = false;
            ProductId         = ProductId.BD_PCI1721;
            DefaultValueRange = ValueRange.V_ExternalRefBipolar;
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public override void Open( string description ) {
            Init( "PCI-1721", description );
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        protected override void SafeMode() {
            if ( !IsInit ) {
                return;
            }
            // Задание безопасной конфигурации конфигурации
            for ( var i = 0; i < ChannelCountMax; i++ ) {
                InstantAoContrl.Channels[ i ].ValueRange = DefaultValueRange;
                Write( i, 0 );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        public void Write( int channel, double value ) {
            if ( !IsInit ) {
                // Модуль не инициализирован
                throw new Exception< Pci1716ExceptionArgs >( new Pci1716ExceptionArgs( 0 ),
                    "The object of card Pci1716 is not initialized" );
            }
            if ( channel < 0 || channel >= ChannelCountMax ) {
                // Номер канала задан неверно
                throw new Exception< Pci1721ExceptionArgs >( new Pci1721ExceptionArgs( 0 ),
                    "The channel of card Pci1721 is not valid" );
            }
            InstantAoContrl.Channels[ channel ].ValueRange = DefaultValueRange;
            var error_code = InstantAoContrl.Write( channel, value );
            if ( error_code != ErrorCode.Success ) {
                // Возникла ошибка
                throw new Exception< Pci1721ExceptionArgs >( new Pci1721ExceptionArgs( 0 ),
                    $"Ошибка записи данных в канал {channel:D} интерфейсной платы PCI-1721 - {error_code}" );
            }
        }
    }
}