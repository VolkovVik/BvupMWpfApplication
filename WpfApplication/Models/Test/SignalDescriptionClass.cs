using System;
using System.Collections.Generic;
using WpfApplication.Models.Main;

namespace WpfApplication.Models.Test {

    /// <summary>
    /// Протокол обмена
    /// </summary>
    public static class Protocol {
        /// <summary>
        /// 
        /// </summary>
        public const string Pci429 = "PCI-429";

        /// <summary>
        /// 
        /// </summary>
        public const string Rs232 = "RS-232";

        /// <summary>
        /// 
        /// </summary>
        public const string Pci1716 = "PCI-1716";

        /// <summary>
        /// 
        /// </summary>
        public const string Pci1721 = "PCI-1721";

        /// <summary>
        /// 
        /// </summary>
        public const string Pci1724U = "PCI-1724U";

        /// <summary>
        /// 
        /// </summary>
        public const string Pci1747U = "PCI-1747U";

        /// <summary>
        /// 
        /// </summary>
        public const string Pci1753 = "PCI-1753";

        /// <summary>
        /// 
        /// </summary>
        public const string Upc10 = "UPC-10";
    }

    /// <summary>
    /// Описание всех проверяемых цепей
    /// </summary>
    public class SignalDescription {
        public          int?                           Device;
        public readonly string                         Name;
        // ReSharper disable once UnassignedField.Global
        public          string                         Circuit;
        public readonly List< PlaceSignalDescription > Output = new List< PlaceSignalDescription >();
        public readonly List< PlaceSignalDescription > Input  = new List< PlaceSignalDescription >();
        // ReSharper disable once UnassignedField.Global
        public          double                         Delta;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public SignalDescription( int device, string name, string description,
            IReadOnlyCollection< string > output, IReadOnlyCollection< string > input ) {
            Device = device;
            Name   = name;
            // Передатчики
            if ( output != null && output.Count > 0  ) {
                foreach ( var item in output ) {
                    if ( !string.IsNullOrWhiteSpace( item ) )
                        Output.Add( new PlaceSignalDescription( device, name, description, item ) );
                }
            }
            if ( input != null && input.Count > 0 ) {
                // Приемники
                foreach ( var item in input ) {
                    if ( !string.IsNullOrWhiteSpace( item ) )
                        Input.Add( new PlaceSignalDescription( device, name, description, item ) );
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        public SignalDescription( int device, string name, string description, string output, string input )
            : this( device, name,  description, new List< string > { output }, new List< string > { input } ) { }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        public SignalDescription( int device, string name, string description, IReadOnlyCollection< string > output,
            string input )
            : this( device, name, description, output, new List< string > { input } ) { }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        public SignalDescription( int device, string name, string description, string output,
            IReadOnlyCollection< string > input )
            : this( device, name, description, new List< string > { output }, input ) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Name;
    }
    
    /// <summary>
    /// Описание одной точки входа/выхода проверяемой цепи
    /// </summary>
    public class PlaceSignalDescription {
        public string  Name        { private set; get; }
        public string  Signal      { private set; get; }
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string  Description { private set; get; }
        public string  Protocol    { private set; get; }
        public int?    Device      { private set; get; }
        public int?    Channel     { private set; get; }
        public int?    Word        { private set; get; }
        public int?    Pin         { private set; get; }
        // ReSharper disable once MemberCanBePrivate.Global
        public double? Coefficient { set;         get; }
        public double? Offset      { set;         get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="signal"></param>
        /// <param name="description"></param>
        /// <param name="protocol"></param>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="word"></param>
        /// <param name="pin"></param>
        /// <param name="coefficient"></param>
        /// <param name="offset"></param>
        public PlaceSignalDescription( string name, string signal, string description, string protocol,
            int? index, int? channel, int? word, int? pin, double coefficient = 1, double offset = 0 ) {
            Init( name, signal, description, protocol, index, channel, word, pin, coefficient, offset );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="signal"></param>
        /// <returns></returns>
        public PlaceSignalDescription( int index, string name, string description, string signal ) {
            int? device, channel = null, word, pin;

            if ( string.IsNullOrWhiteSpace( signal ) ) return;
            if ( signal.ToFindStringExt( "ВМ-7" ) ) {
                device = index;
                word   = signal.ToSubstringExt( "ВМ-7 word " ).ToIntExt();
                pin = ( signal.ToSubstringExt( "byte " ).ToIntExt() * 8 ?? 0 ) |
                      signal.ToSubstringExt( "pin " ).ToIntExt();
                Init( name, signal, description, Test.Protocol.Rs232, device, null, word, pin );
                return;
            }
            if ( signal.ToFindStringExt( "OUT_ARINC" ) ) {
                device  = index;
                channel = ( signal.ToSubstringExt( "OUT_ARINC_" ).ToIntExt() ?? 0 ) - 25;
                Init( name, signal, description, Test.Protocol.Rs232, device, channel, null, null );
                return;
            }
            if ( signal.ToFindStringExt( "IN_ARINC" ) ) {
                device  = index;
                channel = ( signal.ToSubstringExt( "IN_ARINC_" ).ToIntExt() ?? 0 ) - 1;
                Init( name, signal, description, Test.Protocol.Rs232, device, channel, null, null );
                return;
            }
            if ( signal.ToFindStringExt( "OUT_DAC" ) ) {
                device  = 0;
                channel = signal.ToSubstringExt( "OUT_DAC_" ).ToIntExt() ?? 0;
                Init( name, signal, description, Test.Protocol.Rs232, device, channel, null, null );
                return;
            }
            if ( signal.ToFindStringExt( "IN_ANALOG" ) ) {
                device = 0;
                channel = ( ( signal.ToSubstringExt( "Код управления " ).ToIntExt( 2 ) ?? 0 ) << 2 ) |
                          ( ( signal.ToSubstringExt( "АЦП №" ).ToIntExt()              ?? 0 ) - 1 );
                Init( name, signal, description, Test.Protocol.Rs232, device, channel, null, null );
                return;
            }
            if ( signal.ToFindStringExt( "УПС-10" ) ) {
                device = index;
                word   = signal.ToSubstringExt( "УПС-10 word " ).ToIntExt();
                pin    = signal.ToSubstringExt( "pin " ).ToIntExt();
                Init( name, signal, description, Test.Protocol.Upc10, device, null, word, pin );
                return;
            }
            if ( signal.ToFindStringExt( "PCI-429" ) ) {
                device = ( signal.ToSubstringExt( "PCI-429-4-3 №" ).ToIntExt() ?? 0 ) - 1;
                if ( signal.ToFindStringExt( "КП" ) ) channel = signal.ToSubstringExt( "КП" ).ToIntExt();
                if ( signal.ToFindStringExt( "КВ" ) ) channel = signal.ToSubstringExt( "КВ" ).ToIntExt();
                Init( name, signal, description, Test.Protocol.Pci429, device, channel, null, null );
                return;
            }
            if ( signal.ToFindStringExt( "PCI-1721" ) ) {
                device  = 0;
                channel = signal.ToSubstringExt( "port VOUT" ).ToIntExt();
                Init( name, signal, description, Test.Protocol.Pci1721, device, channel, null, null );
                return;
            }
            if( signal.ToFindStringExt( "PCI-1724U" ) ) {
                device  = 0;
                channel = signal.ToSubstringExt( "port AO" ).ToIntExt();
                Init( name, signal, description, Test.Protocol.Pci1724U, device, channel, null, null );
                return;
            }
            if( signal.ToFindStringExt( "PCI-1747U" ) ) {
                device  = 0;
                channel = signal.ToSubstringExt( "port AI" ).ToIntExt();
                Init( name, signal, description, Test.Protocol.Pci1724U, device, channel, null, null );
            }
            if ( signal.ToFindStringExt( "PCI-1753" ) ) {
                device = signal.ToSubstringExt( "PCI-1753 " ).ToIntExt();
                var tuple =  GetPci1753Config( signal, "port " );
                channel = tuple.Item1;
                pin     = tuple.Item2;
                Init( name, signal, description, Test.Protocol.Pci1753, device, channel, null, pin );
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="signal"></param>
        /// <param name="description"></param>
        /// <param name="protocol"></param>
        /// <param name="device"></param>
        /// <param name="channel"></param>
        /// <param name="word"></param>
        /// <param name="pin"></param>
        /// <param name="coefficient"></param>
        /// <param name="offset"></param>
        private void Init( string name, string signal, string description, string protocol,
            int? device, int? channel, int? word, int? pin, double coefficient = 1, double offset = 0 ) {
            Name        = name;
            Signal      = signal;
            Description = description;
            Protocol    = protocol;
            Device      = device;
            Channel     = channel;
            Word        = word;
            Pin         = pin;
            Coefficient = coefficient;
            Offset      = offset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="begin"></param>
        /// <returns></returns>
        private static Tuple< int?, int? > GetPci1753Config( string source, string begin ) {
            var str = source.ToSubstringExt( begin );
            if ( str == null || str.Length < 4 ) return new Tuple< int?, int? >( null, null );
            int? port;
            switch ( str.Substring( 0, 2 ) ) {
                case "PA": {
                    port = 0;
                    break;
                }
                case "PB": {
                    port = 1;
                    break;
                }
                case "PC": {
                    port = 2;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException( nameof( str ), str, null );
            }
            port += str.Substring( 2, 1 ).ToIntExt() * 3;
            var pin = str.Substring( 3, 1 ).ToIntExt();
            return new Tuple< int?, int? >( port, pin );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Name}  {Description}";
    }
}