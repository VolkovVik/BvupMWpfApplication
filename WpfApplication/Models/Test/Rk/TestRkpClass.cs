using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.Rk {
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    internal class TestRkpClass : TestRkClass {
        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        private readonly List< SignalDescription > _testList = new List< SignalDescription > {
            // Устройство ВМ-7 канал
            //
            // Тестирование разовые команды приема 27В/Обрыв
            //
            new SignalDescription( 0, "РКП1", "", "PCI-1753 1 pin 1 port PA00", "ВМ-7 word 1 byte 0 pin 00" ),
            new SignalDescription( 0, "РКП2", "", "PCI-1753 1 pin 2 port PA01", "ВМ-7 word 1 byte 0 pin 01" ),
            new SignalDescription( 0, "РКП3", "", "PCI-1753 1 pin 3 port PA02", "ВМ-7 word 1 byte 0 pin 01" ),
            new SignalDescription( 0, "РКП4", "", "PCI-1753 1 pin 4 port PA03", "ВМ-7 word 1 byte 0 pin 03" ),
            new SignalDescription( 0, "РКП5", "", "PCI-1753 1 pin 5 port PA04", "ВМ-7 word 1 byte 0 pin 04" ),
            new SignalDescription( 0, "РКП6", "", "PCI-1753 1 pin 6 port PA05", "ВМ-7 word 1 byte 0 pin 05" ),
            new SignalDescription( 0, "РКП7", "", "PCI-1753 1 pin 7 port PA06", "ВМ-7 word 1 byte 0 pin 06" ),
            new SignalDescription( 0, "РКП8", "", "PCI-1753 1 pin 8 port PA07", "ВМ-7 word 1 byte 0 pin 07" ),
            new SignalDescription( 0, "РКП9", "", "PCI-1753 1 pin 9 port PB00", "ВМ-7 word 1 byte 1 pin 00" ),
            new SignalDescription( 0, "РКП10", "", "PCI-1753 1 pin 10 port PB01", "ВМ-7 word 1 byte 1 pin 01" ),
            new SignalDescription( 0, "РКП11", "", "PCI-1753 1 pin 11 port PB02", "ВМ-7 word 1 byte 1 pin 02" ),
            new SignalDescription( 0, "РКП12", "", "PCI-1753 1 pin 12 port PB03", "ВМ-7 word 1 byte 1 pin 03" ),
            new SignalDescription( 0, "РКП13", "", "PCI-1753 1 pin 13 port PB04", "ВМ-7 word 1 byte 1 pin 04" ),
            new SignalDescription( 0, "РКП14", "", "PCI-1753 1 pin 14 port PB05", "ВМ-7 word 1 byte 1 pin 05" ),
            new SignalDescription( 0, "РКП15", "", "PCI-1753 1 pin 15 port PB06", "ВМ-7 word 1 byte 1 pin 06" ),
            new SignalDescription( 0, "РКП16", "", "PCI-1753 1 pin 16 port PB07", "ВМ-7 word 1 byte 1 pin 07" ),
            new SignalDescription( 0, "РКП17", "", "PCI-1753 1 pin 17 port PC00", "ВМ-7 word 1 byte 2 pin 00" ),
            new SignalDescription( 0, "РКП18", "", "PCI-1753 1 pin 18 port PC01", "ВМ-7 word 1 byte 2 pin 01" ),
            new SignalDescription( 0, "РКП19", "", "PCI-1753 1 pin 19 port PC02", "ВМ-7 word 1 byte 2 pin 02" ),
            new SignalDescription( 0, "РКП20", "", "PCI-1753 1 pin 20 port PC03", "ВМ-7 word 1 byte 2 pin 03" ),
            new SignalDescription( 0, "РКП21", "", "PCI-1753 1 pin 21 port PC04", "ВМ-7 word 1 byte 2 pin 04" ),
            new SignalDescription( 0, "РКП22", "", "PCI-1753 1 pin 22 port PC05", "ВМ-7 word 1 byte 2 pin 05" ),
            new SignalDescription( 0, "РКП23", "", "PCI-1753 1 pin 23 port PC06", "ВМ-7 word 1 byte 2 pin 06" ),
            new SignalDescription( 0, "РКП24", "", "PCI-1753 1 pin 24 port PC07", "ВМ-7 word 1 byte 2 pin 07" ),
            new SignalDescription( 0, "РКП25", "", "PCI-1753 1 pin 26 port PA10", "ВМ-7 word 1 byte 3 pin 00" ),
            new SignalDescription( 0, "РКП26", "", "PCI-1753 1 pin 27 port PA11", "ВМ-7 word 1 byte 3 pin 01" ),
            new SignalDescription( 0, "РКП27", "", "PCI-1753 1 pin 28 port PA12", "ВМ-7 word 1 byte 3 pin 02" ),
            new SignalDescription( 0, "РКП28", "", "PCI-1753 1 pin 29 port PA13", "ВМ-7 word 1 byte 3 pin 03" ),
            new SignalDescription( 0, "РКП29", "", "PCI-1753 1 pin 30 port PA14", "ВМ-7 word 1 byte 3 pin 04" ),
            new SignalDescription( 0, "РКП30", "", "PCI-1753 1 pin 31 port PA15", "ВМ-7 word 1 byte 3 pin 05" ),
            new SignalDescription( 0, "РКП31", "", "PCI-1753 1 pin 32 port PA16", "ВМ-7 word 1 byte 3 pin 06" ),
            new SignalDescription( 0, "РКП32", "", "PCI-1753 1 pin 33 port PA17", "ВМ-7 word 1 byte 3 pin 07" ),
            new SignalDescription( 0, "РКП33", "", "PCI-1753 1 pin 34 port PB10", "ВМ-7 word 2 byte 0 pin 00" ),
            new SignalDescription( 0, "РКП34", "", "PCI-1753 1 pin 35 port PB11", "ВМ-7 word 2 byte 0 pin 01" ),
            new SignalDescription( 0, "РКП35", "", "PCI-1753 1 pin 36 port PB12", "ВМ-7 word 2 byte 0 pin 02" ),
            new SignalDescription( 0, "РКП36", "", "PCI-1753 1 pin 37 port PB13", "ВМ-7 word 2 byte 0 pin 03" ),
            new SignalDescription( 0, "РКП37", "", "PCI-1753 1 pin 38 port PB14", "ВМ-7 word 2 byte 0 pin 04" ),
            new SignalDescription( 0, "РКП38", "", "PCI-1753 1 pin 39 port PB15", "ВМ-7 word 2 byte 0 pin 05" ),
            new SignalDescription( 0, "РКП39", "", "PCI-1753 1 pin 40 port PB16", "ВМ-7 word 2 byte 0 pin 06" ),
            new SignalDescription( 0, "РКП40", "", "PCI-1753 1 pin 41 port PB17", "ВМ-7 word 2 byte 0 pin 07" ),
            new SignalDescription( 0, "РКП41", "", "PCI-1753 1 pin 42 port PC10", "ВМ-7 word 2 byte 1 pin 00" ),
            new SignalDescription( 0, "РКП42", "", "PCI-1753 1 pin 43 port PC11", "ВМ-7 word 2 byte 1 pin 01" ),
            new SignalDescription( 0, "РКП43", "", "PCI-1753 1 pin 44 port PC12", "ВМ-7 word 2 byte 1 pin 02" ),
            new SignalDescription( 0, "РКП44", "", "PCI-1753 1 pin 45 port PC13", "ВМ-7 word 2 byte 1 pin 03" ),
            new SignalDescription( 0, "РКП45", "", "PCI-1753 1 pin 46 port PC14", "ВМ-7 word 2 byte 1 pin 04" ),
            new SignalDescription( 0, "РКП46", "", "PCI-1753 1 pin 47 port PC15", "ВМ-7 word 2 byte 1 pin 05" ),
            //
            // Тестирование разовые команды приема 0В/Обрыв
            //
            new SignalDescription( 0, "РКПо1", "", "PCI-1753 1 pin 51 port PA20", "ВМ-7 word 2 byte 2 pin 00" ),
            new SignalDescription( 0, "РКПо2", "", "PCI-1753 1 pin 52 port PA21", "ВМ-7 word 2 byte 2 pin 01" ),
            new SignalDescription( 0, "РКПо3", "", "PCI-1753 1 pin 53 port PA22", "ВМ-7 word 2 byte 2 pin 02" ),
            new SignalDescription( 0, "РКПо4", "", "PCI-1753 1 pin 54 port PA23", "ВМ-7 word 2 byte 2 pin 03" ),
            new SignalDescription( 0, "РКПо5", "", "PCI-1753 1 pin 55 port PA24", "ВМ-7 word 2 byte 2 pin 04" ),
            new SignalDescription( 0, "РКПо6", "", "PCI-1753 1 pin 56 port PA25", "ВМ-7 word 2 byte 2 pin 05" ),
            new SignalDescription( 0, "РКПо7", "", "PCI-1753 1 pin 57 port PA26", "ВМ-7 word 2 byte 2 pin 06" ),
            new SignalDescription( 0, "РКПо8", "", "PCI-1753 1 pin 58 port PA27", "ВМ-7 word 2 byte 2 pin 07" ),
            new SignalDescription( 0, "РКПо9", "PLACE_DEVICE_0", "PCI-1753 1 pin 59 port PB20",
                "ВМ-7 word 0 pin 16" ),
            new SignalDescription( 0, "РКПо10", "PLACE_DEVICE_1", "PCI-1753 1 pin 60 port PB21",
                "ВМ-7 word 0 pin 17" ),
            new SignalDescription( 0, "РКПо11", "", "PCI-1753 1 pin 61 port PB22", "ВМ-7 word 2 byte 3 pin 00" ),
            new SignalDescription( 0, "РКПо12", "", "PCI-1753 1 pin 62 port PB23", "ВМ-7 word 2 byte 3 pin 01" ),
            //
            // Тестирование универсальные разовые команды приема 27В/Обрыв
            //
            new SignalDescription( 0, "РКПу1", "RKIM_0", "PCI-1753 1 pin 65 port PB26", "ВМ-7 word 0 pin 08" ),
            new SignalDescription( 0, "РКПу2", "RKIM_1", "PCI-1753 1 pin 66 port PB27", "ВМ-7 word 0 pin 09" ),
            new SignalDescription( 0, "РКПу3", "RKIM_2", "PCI-1753 1 pin 67 port PC20", "ВМ-7 word 0 pin 10" ),
            new SignalDescription( 0, "РКПу4", "RKIM_3", "PCI-1753 1 pin 68 port PC21", "ВМ-7 word 0 pin 11" ),
            //
            // Тестирование универсальные разовые команды приема 0В/Обрыв
            //
            new SignalDescription( 0, "РКПу5", "RKIM_4", "PCI-1753 1 pin 69 port PC22", "ВМ-7 word 0 pin 12" ),
            new SignalDescription( 0, "РКПу6", "RKIM_5", "PCI-1753 1 pin 70 port PC23", "ВМ-7 word 0 pin 13" ),
            new SignalDescription( 0, "РКПу7", "RKIM_6", "PCI-1753 1 pin 71 port PC24", "ВМ-7 word 0 pin 14" ),
            new SignalDescription( 0, "РКПу8", "RKIM_7", "PCI-1753 1 pin 72 port PC25", "ВМ-7 word 0 pin 15" )
        };

        /// <summary>
        /// 
        /// </summary>
        ~TestRkpClass() {
            _testList?.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Start( int id ) {
            // Индекс СОМ-порта
            Index = id == ( int ) ConfigTestsClass.IdTest.Rkp1 ? 0 : 1;
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
            // Контрольные значения
            DefaultOffConfig  = 0;
            DefaultOnConfig   = 1;
            ConfigList        = new List< byte > { 0, 1 };
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
        protected override void Set( PlaceSignalDescription signal, byte value ) => Pci1753Func.Set( signal, value );

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        protected override byte Get( PlaceSignalDescription signal ) => RkFunc.Get( signal );
    }
}