using WpfApplication.Models.PciCard.advantech.@abstract;

namespace WpfApplication.Models.PciCard.advantech.pci1724u {
    /// <inheritdoc />
    /// <summary>
    /// Класс работы  с интерфейсной платой Advantech PCI-1724U
    /// </summary>
    public class Port1724U : PortClass< DaqPci1724U > {
        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxDevice"></param>
        public Port1724U( string name, int maxDevice ) : base( name, maxDevice, 32 ) { }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void SafeMode( int index ) {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        public void Write( int index, int channel, double value ) {
            Validation( index );
            Device[ index ].Write( channel, value );
        }
    }
}