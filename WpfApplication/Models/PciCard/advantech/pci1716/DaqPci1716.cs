using Automation.BDaq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.advantech.@abstract;

namespace WpfApplication.Models.PciCard.advantech.pci1716 {
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    public class DaqPci1716 : DaqPciClass {
        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        public DaqPci1716() {
            IsInit            = false;
            ProductId         = ProductId.BD_PCI1716;
            DefaultValueRange = ValueRange.V_Neg10To10;
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public override void Open( string description ) {
            Init( "PCI-1716", description );
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        protected override void SafeMode() {
            if ( !IsInit ) return;
            // Задание безопасной конфигурации конфигурации
            for ( var i = 0; i < ChannelCountMax; i++ ) {
                InstantAiContrl.Channels[ i ].ValueRange = DefaultValueRange;
                InstantAiContrl.Channels[ i ].SignalType = AiSignalType.SingleEnded;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public double Read( int channel ) {
            double value;

            if ( !IsInit ) {
                throw new Exception< Pci1716ExceptionArgs >( new Pci1716ExceptionArgs( 0 ),
                    "The object of card Pci1716 is not initialized" );
            }
            if ( channel < 0 || channel >= ChannelCountMax ) {
                throw new Exception< Pci1716ExceptionArgs >( new Pci1716ExceptionArgs( 0 ),
                    $"The channel {channel} of card Pci1716 is not valid" );
            }
            InstantAiContrl.Channels[ channel ].ValueRange = DefaultValueRange;
            var error_code = InstantAiContrl.Read( channel, out value );
            if ( error_code != ErrorCode.Success ) {
                // Возникла ошибка
                throw new Exception< Pci1716ExceptionArgs >( new Pci1716ExceptionArgs( 0 ),
                    $"Ошибка чтения данных из канала {channel} интерфейсной платы PCI-1716 - {error_code}" );
            }
            return value;
        }
    }
}