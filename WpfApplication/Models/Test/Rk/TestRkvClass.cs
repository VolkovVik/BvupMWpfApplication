using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.Rk {
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    internal class TestRkvClass : TestRkClass{
        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        private readonly List< SignalDescription > _testList = new List< SignalDescription > {
            // Устройство ВМ-7 канал
            //
            // Тестирование разовые команды выдачи 27В/Обрыв
            //
            new SignalDescription( 0, "РКВ1", "", "УПС-10 word 251 pin 12",
                new List< string > { "PCI-1753 2 pin 1 port PA00", "УПС-10 word 251 pin 12" } ),
            new SignalDescription( 0, "РКВ2", "", "УПС-10 word 251 pin 13",
                new List< string > { "PCI-1753 2 pin 2 port PA01", "УПС-10 word 251 pin 13" } ),
            new SignalDescription( 0, "РКВ3", "", "УПС-10 word 251 pin 14",
                new List< string > { "PCI-1753 2 pin 3 port PA02", "УПС-10 word 251 pin 14" } ),
            new SignalDescription( 0, "РКВ4", "", "УПС-10 word 251 pin 15",
                new List< string > { "PCI-1753 2 pin 4 port PA03", "УПС-10 word 251 pin 15" } ),
            new SignalDescription( 0, "РКВ5", "", "УПС-10 word 251 pin 16",
                new List< string > { "PCI-1753 2 pin 5 port PA04", "УПС-10 word 251 pin 16" } ),
            new SignalDescription( 0, "РКВ6", "", "УПС-10 word 251 pin 17",
                new List< string > { "PCI-1753 2 pin 6 port PA05", "УПС-10 word 251 pin 17" } ),
            new SignalDescription( 0, "РКВ7", "", "УПС-10 word 251 pin 18",
                new List< string > { "PCI-1753 2 pin 7 port PA06", "УПС-10 word 251 pin 18" } ),
            new SignalDescription( 0, "РКВ8", "", "УПС-10 word 251 pin 19",
                new List< string > { "PCI-1753 2 pin 8 port PA07", "УПС-10 word 251 pin 19" } ),
            new SignalDescription( 0, "РКВ9", "", "УПС-10 word 251 pin 20",
                new List< string > { "PCI-1753 2 pin 9 port PB00", "УПС-10 word 251 pin 20" } ),
            new SignalDescription( 0, "РКВ10", "", "УПС-10 word 251 pin 21",
                new List< string > { "PCI-1753 2 pin 10 port PB01", "УПС-10 word 251 pin 21" } ),
            new SignalDescription( 0, "РКВ11", "", "УПС-10 word 251 pin 22",
                new List< string > { "PCI-1753 2 pin 11 port PB02", "УПС-10 word 251 pin 22" } ),
            new SignalDescription( 0, "РКВ12", "", "УПС-10 word 251 pin 23",
                new List< string > { "PCI-1753 2 pin 12 port PB03", "УПС-10 word 251 pin 23" } ),
            new SignalDescription( 0, "РКВ13", "", "УПС-10 word 251 pin 24",
                new List< string > { "PCI-1753 2 pin 13 port PB04", "УПС-10 word 251 pin 24" } ),
            new SignalDescription( 0, "РКВ14", "", "УПС-10 word 251 pin 25",
                new List< string > { "PCI-1753 2 pin 14 port PB05", "УПС-10 word 251 pin 25" } ),
            new SignalDescription( 0, "РКВ15", "", "УПС-10 word 251 pin 26",
                new List< string > { "PCI-1753 2 pin 15 port PB06", "УПС-10 word 251 pin 26" } ),
            new SignalDescription( 0, "РКВ16", "", "УПС-10 word 251 pin 27",
                new List< string > { "PCI-1753 2 pin 16 port PB07", "УПС-10 word 251 pin 27" } ),
            new SignalDescription( 0, "РКВ17", "", "УПС-10 word 251 pin 28",
                new List< string > { "PCI-1753 2 pin 17 port PC00", "УПС-10 word 251 pin 28" } ),
            new SignalDescription( 0, "РКВ18", "Исправность БВУП RKOS_0 (эхо RKIS_2)", "ВМ-7 word 0 pin 0",
                new List< string > { "PCI-1753 2 pin 42 port PC10", "ВМ-7 word 0 pin 2" } ),
            //
            // Тестирование разовые команды выдачи 0В/Обрыв
            //
            new SignalDescription( 0, "РКВо1", "", "УПС-10 word 252 pin 15",
                new List< string > { "PCI-1753 2 pin 18 port PC01", "УПС-10 word 252 pin 15" } ),
            new SignalDescription( 0, "РКВо2", "", "УПС-10 word 252 pin 16",
                new List< string > { "PCI-1753 2 pin 19 port PC02", "УПС-10 word 252 pin 16" } ),
            new SignalDescription( 0, "РКВо3", "", "УПС-10 word 252 pin 17",
                new List< string > { "PCI-1753 2 pin 20 port PC03", "УПС-10 word 252 pin 17" } ),
            new SignalDescription( 0, "РКВо4", "", "УПС-10 word 252 pin 18",
                new List< string > { "PCI-1753 2 pin 21 port PC04", "УПС-10 word 252 pin 18" } ),
            new SignalDescription( 0, "РКВо5", "", "УПС-10 word 252 pin 19",
                new List< string > { "PCI-1753 2 pin 22 port PC05", "УПС-10 word 252 pin 19" } ),
            new SignalDescription( 0, "РКВо6", "", "УПС-10 word 252 pin 20",
                new List< string > { "PCI-1753 2 pin 23 port PC06", "УПС-10 word 252 pin 20" } ),
            new SignalDescription( 0, "РКВо7", "", "УПС-10 word 252 pin 21",
                new List< string > {  "PCI-1753 2 pin 24 port PC07", "УПС-10 word 252 pin 21" } ),
            new SignalDescription( 0, "РКВо8", "", "УПС-10 word 252 pin 22",
                new List< string > {  "PCI-1753 2 pin 26 port PA10", "УПС-10 word 252 pin 22" } ),
            new SignalDescription( 0, "РКВо9", "", "УПС-10 word 252 pin 23",
                new List< string > { "PCI-1753 2 pin 27 port PA11", "УПС-10 word 252 pin 23" } ),
            new SignalDescription( 0, "РКВо10", "", "УПС-10 word 252 pin 24",
                new List< string > { "PCI-1753 2 pin 28 port PA12", "УПС-10 word 252 pin 24" } ),
            new SignalDescription( 0, "РКВо11", "", "УПС-10 word 252 pin 25",
                new List< string > { "PCI-1753 2 pin 29 port PA13", "УПС-10 word 252 pin 25" } ),
            new SignalDescription( 0, "РКВо12", "", "УПС-10 word 252 pin 26",
                new List< string > { "PCI-1753 2 pin 30 port PA14", "УПС-10 word 252 pin 26" } ),
            new SignalDescription( 0, "РКВо13", "", "УПС-10 word 252 pin 27",
                new List< string > { "PCI-1753 2 pin 31 port PA15", "УПС-10 word 252 pin 27" } ),
            new SignalDescription( 0, "РКВо14", "", "УПС-10 word 252 pin 28",
                new List< string > { "PCI-1753 2 pin 32 port PA16", "УПС-10 word 252 pin 28" } ),
            //
            // Тестирование универсальные разовые команды выдачи 27В/Обрыв
            //
            new SignalDescription( 0, "РКВу1", "RKOM_0", "ВМ-7 word 0 pin 8", "PCI-1753 2 pin 34 port PB10" ),
            new SignalDescription( 0, "РКВу2", "RKOM_1", "ВМ-7 word 0 pin 9", "PCI-1753 2 pin 35 port PB11" ),
            new SignalDescription( 0, "РКВу3", "RKOM_2", "ВМ-7 word 0 pin 10", "PCI-1753 2 pin 36 port PB12" ),
            new SignalDescription( 0, "РКВу4", "RKOM_3", "ВМ-7 word 0 pin 11", "PCI-1753 2 pin 37 port PB13" ),
            //
            // Тестирование универсальные разовые команды выдачи 0В/Обрыв
            //
            new SignalDescription( 0, "РКВу5", "RKOM_4", "ВМ-7 word 0 pin 12", "PCI-1753 2 pin 38 port PB14" ),
            new SignalDescription( 0, "РКВу6", "RKOM_5", "ВМ-7 word 0 pin 13", "PCI-1753 2 pin 39 port PB15" ),
            new SignalDescription( 0, "РКВу7", "RKOM_6", "ВМ-7 word 0 pin 14", "PCI-1753 2 pin 40 port PB16" ),
            new SignalDescription( 0, "РКВу8", "RKOM_7", "ВМ-7 word 0 pin 15", "PCI-1753 2 pin 41 port PB17" )
        };

        ~TestRkvClass() {
            _testList?.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Start( int id ) {
            // Индекс СОМ-порта
            Index = id == ( int ) ConfigTestsClass.IdTest.Rkv1 ? 0 : 1;
            // Проверка заданного индекса
            if ( Index < 0 || Index >= PortRs232.MaxDevice ) {
                throw new Exception< ParameterNotFoundExceptionArgs >( new ParameterNotFoundExceptionArgs(),
                    $"Ошибка допустимости данных индекса {Index}" );
            }
            // Проверка используемых идентификаторов
            if ( !App.TaskManager.IdEnabledsDictionary[ Index ].Rs232 ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    $"Ошибка! Инициализация идентификаторов {Index:D} порта RS232 не пройдена" );
            }
            if ( !App.TaskManager.IdEnabledsDictionary[ Index ].Rk ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    $"Ошибка! Инициализация идентификаторов {Index:D} канала RK не пройдена" );
            }
            if ( !App.TaskManager.IdEnabledsDictionary[ Index ].Arinc ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    $"Ошибка! Инициализация идентификаторов {Index:D} канала Arinc429 не пройдена" );
            }
            // Контрольные значения
            DefaultOffConfig  = 0;
            DefaultOnConfig   = 1;
            ConfigList        = new List<byte> { 0, 1 };
            FullTestIsChecked = true;
            // Список аналоговых сигналов приема
            SignalList = _testList.Where( o => o.Device != null  &&
                                               o.Device == Index &&
                                               !string.IsNullOrWhiteSpace( o.Name ) ).ToList();
            return Design();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="value"></param>
        public void Set( int device, byte value ) {
            // Список аналоговых сигналов приема
            SignalList = _testList.Where( o => o.Device != null   &&
                                               o.Device == device &&
                                               !string.IsNullOrWhiteSpace( o.Name ) ).ToList();
            Set( value );
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        protected override void Set( PlaceSignalDescription signal, byte value ) {
            if ( signal.Protocol == Protocol.Upc10 ) {
                Upc10Func.SetRk( signal, value );
            } else {
                RkFunc.Set( signal, value );
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        protected override byte Get( PlaceSignalDescription signal ) {
            switch ( signal.Protocol ) {
                case Protocol.Pci1753: return Pci1753Func.Get( signal );
                case Protocol.Rs232:   return RkFunc.Get( signal );
                case Protocol.Upc10:   return Upc10Func.GetRk( signal );
                default:               throw new System.ArgumentException();
            }
        }
    }
}