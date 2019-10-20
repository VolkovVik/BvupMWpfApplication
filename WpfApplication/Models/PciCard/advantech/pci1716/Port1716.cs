using WpfApplication.Models.PciCard.advantech.@abstract;

namespace WpfApplication.Models.PciCard.advantech.pci1716 {
    /// <inheritdoc />
    /// <summary>
    /// Класс работы  с интерфейсной платой Advantech PCI-1716
    /// </summary>
    public class Port1716 : PortClass< DaqPci1716 > {
        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxDevice"></param>
        public Port1716( string name, int maxDevice ) : base( name, maxDevice, 64 ) { }

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
        /// <returns></returns>
        public double Read( int index, int channel ) {
            Validation( index );
            return Device[ index ].Read( channel );
        }
    }
}