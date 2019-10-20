using Automation.BDaq;
using WpfApplication.Models.PciCard.advantech.@abstract;

namespace WpfApplication.Models.PciCard.advantech.pci1753 {
    /// <inheritdoc />
    /// <summary>
    /// Класс работы  с интерфейсной платой Advantech PCI-1753
    /// </summary>
    public class Port1753 : PortClass< DaqPci1753 > {
        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxDevice"></param>
        public Port1753( string name, int maxDevice ) : base( name, maxDevice, 12 ) { }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void SafeMode( int index ) {
            Validation( index );
            // Установка выходных сигналов в безопасное состояние

            if ( 0 == index ) {
                Device[ index ].Config(
                    new DaqPci1753.ConfigPort {
                        PortA0 = DioPortDir.Output,
                        PortB0 = DioPortDir.Output,
                        PortC0 = DioPortDir.Output,
                        PortA1 = DioPortDir.Output,
                        PortB1 = DioPortDir.Output,
                        PortC1 = DioPortDir.Output,
                        PortA2 = DioPortDir.Output,
                        PortB2 = DioPortDir.Output,
                        PortC2 = DioPortDir.Output,
                        PortA3 = DioPortDir.Input,
                        PortB3 = DioPortDir.Input,
                        PortC3 = DioPortDir.Input
                    } );
            } else {
                Device[ index ].Config(
                    new DaqPci1753.ConfigPort {
                        PortA0 = DioPortDir.Input,
                        PortB0 = DioPortDir.Input,
                        PortC0 = DioPortDir.Input,
                        PortA1 = DioPortDir.Input,
                        PortB1 = DioPortDir.Input,
                        PortC1 = DioPortDir.Input,
                        PortA2 = DioPortDir.Input,
                        PortB2 = DioPortDir.Input,
                        PortC2 = DioPortDir.Input,
                        PortA3 = DioPortDir.Input,
                        PortB3 = DioPortDir.Input,
                        PortC3 = DioPortDir.Input
                    } );
            }
            // Запись исходных данных в порт
            for ( var i = 0; i < Device[ index ].ChannelCountMax; i++ ) {
                // Установка данных в порт
                Device[ index ].WritePort( i, 0x0 );
            }
        }

        /// <summary>
        /// Подпрограмма записи байта данных в порт
        /// </summary>
        /// <param name="index"></param>
        /// <param name="port"></param>
        /// <param name="data"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public void WritePort( int index, int port, byte data, int logic = 1 ) {
            Validation( index );
            if ( logic == 0 ) {
                data = ( byte ) ~data;
            }
            Device[ index ].WritePort( port, data );
        }

        /// <summary>
        /// Подпрограмма записи бита данных в порт
        /// </summary>
        /// <param name="index"></param>
        /// <param name="port"></param>
        /// <param name="bit"></param>
        /// <param name="data"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
        public void WriteBit( int index, int port, int bit, byte data, int logic = 1 ) {
            Validation( index );
            if ( logic == 0 ) {
                data = ( byte ) ~data;
            }
            Device[ index ].WriteBit( port, bit, data );
        }

        /// <summary>
        /// Подпрограмма чтения байта данных из порта
        /// </summary>
        /// <param name="index"></param>
        /// <param name="port"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public byte ReadPort( int index, int port, int logic = 1 ) {
            Validation( index );
            var value = Device[ index ].ReadPort( port );
            if ( logic == 0 ) {
                value = ( byte ) ~value;
            }
            return value;
        }

        /// <summary>
        /// Подпрограмма чтения бита данных из порта
        /// </summary>
        /// <param name="index"></param>
        /// <param name="port"></param>
        /// <param name="bit"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
        public byte ReadBit( int index, int port, int bit, int logic = 1 ) {
            Validation( index );
            var value = Device[ index ].ReadBit( port, bit );
            if ( logic == 0 ) {
                value = ( byte ) ~value;
            }
            return value;
        }
    }
}