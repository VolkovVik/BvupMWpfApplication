using System.Collections.Generic;
using System.Linq;

namespace WpfApplication.Models.Main.XML {
    internal class XmlClass : XmlFileClass {
        public const string NameRs232    = "RS-232";
        public const string NamePci1716  = "PCI-1716";
        public const string NamePci1721  = "PCI-1721";
        public const string NamePci1724  = "PCI-1724U";
        public const string NamePci1747  = "PCI-1747U";
        public const string NamePci1753  = "PCI-1753";
        public const string NamePci429   = "PCI-429";
        public const string NameHexFiles = "HEX_FILES";
        public const string NameLogFiles = "LOG_FILES";
        public const string NameDebug    = "DEBUG";

        public const string ElementPort = "port";
        public const string ElementBid  = "bid";
        public const string ElementSn   = "sn";

        public const string ElementOperator = "OPERATORS";
        public const string ElementOtk      = "OTK";
        public const string ElementVp       = "BP_MO";

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public XmlClass( string name = "config.xml" ) : base( name ) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public List< string > Read( string element ) {
            return base.Read( element ).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public new string Read( string element, string name ) {
            var list = base.Read( element, name ).ToList();
            return list.Count == 0 ? string.Empty : list[ 0 ];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="append"></param>
        public new void Write( string element, string name, string value, bool append = false ) {
            base.Write( element, name, value, append );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        public new void Delete( string element, string name = "" ) {
            base.Delete( element, name );
        }
    }
}