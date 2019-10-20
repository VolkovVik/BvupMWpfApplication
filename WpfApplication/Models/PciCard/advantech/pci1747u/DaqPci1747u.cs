using Automation.BDaq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.advantech.@abstract;

namespace WpfApplication.Models.PciCard.advantech.pci1747u {
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    public class DaqPci1747U : DaqPciClass {
        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        public DaqPci1747U() {
            IsInit            = false;
            ProductId         = ProductId.BD_PCI1747;
            DefaultValueRange = ValueRange.V_Neg10To10;
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public override void Open( string description ) {
            Init( "PCI-1747U", description );
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
                // Модуль не инициализирован
                throw new Exception< Pci1747UExceptionArgs >( new Pci1747UExceptionArgs( 0 ),
                    "The object of card PCI1747U is not initialized" );
            }
            if ( channel < 0 || channel >= ChannelCountMax ) {
                // Номер канала задан неверно
                throw new Exception< Pci1747UExceptionArgs >( new Pci1747UExceptionArgs( 0 ),
                    "The channel of card PCI1747U is not valid" );
            }
            InstantAiContrl.Channels[ channel ].ValueRange = DefaultValueRange;
            var error_code = InstantAiContrl.Read( channel, out value );
            if ( ErrorCode.Success != error_code ) {
                // Возникла ошибка
                throw new Exception< Pci1747UExceptionArgs >( new Pci1747UExceptionArgs( 0 ),
                    $"Ошибка чтения данных из канала {channel:D} интерфейсной платы PCI-1747U - {error_code}" );
            }
            return value;
        }
    }
}