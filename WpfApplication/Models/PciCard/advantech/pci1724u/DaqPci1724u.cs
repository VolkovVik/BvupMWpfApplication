using Automation.BDaq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.advantech.@abstract;

namespace WpfApplication.Models.PciCard.advantech.pci1724u {
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    public class DaqPci1724U : DaqPciClass {
        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        public DaqPci1724U() {
            IsInit            = false;
            ProductId         = ProductId.BD_PCI1724;
            DefaultValueRange = ValueRange.V_Neg10To10;
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public override void Open( string description ) {
            Init( "PCI-1724U", description );
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
            //for ( var i = 0; i < ChannelCountMax; i++ ) {
            //    _instantAoContrl.Channels[ i ].ValueRange = DefaultValueRange;
            //    Write( i, 0 );
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        public void Write( int channel, double value ) {
            if ( !IsInit ) {
                // Модуль не инициализирован
                throw new Exception< Pci1724UExceptionArgs >( new Pci1724UExceptionArgs( 0 ),
                    "The object of card Pci1724U is not initialized" );
            }
            if ( channel < 0 || channel >= ChannelCountMax ) {
                // Номер канала задан неверно
                throw new Exception< Pci1724UExceptionArgs >( new Pci1724UExceptionArgs( 0 ),
                    "The channel of card Pci1724U is not valid" );
            }
            InstantAoContrl.Channels[ channel ].ValueRange = DefaultValueRange;
            var error_code = InstantAoContrl.Write( channel, value );
            if ( ErrorCode.Success != error_code ) {
                // Возникла ошибка
                throw new Exception< Pci1724UExceptionArgs >( new Pci1724UExceptionArgs( 0 ),
                    $"Ошибка записи данных в канал {channel:D} интерфейсной платы PCI-1724U - {error_code}" );
            }
        }
    }
}