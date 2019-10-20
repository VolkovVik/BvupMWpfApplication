using WpfApplication.Models.PciCard.advantech.@abstract;

namespace WpfApplication.Models.PciCard.advantech.pci1721 {
    /// <inheritdoc />
    /// <summary>
    /// Класс работы  с интерфейсной платой Advantech PCI-1721
    /// </summary>
    public class Port1721 : PortClass< DaqPci1721 > {
        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxDevice"></param>
        public Port1721( string name, int maxDevice ) : base( name, maxDevice, 4 ) { }

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