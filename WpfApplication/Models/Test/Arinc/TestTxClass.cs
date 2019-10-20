using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.arinc429;

namespace WpfApplication.Models.Test.Arinc {
    /// <inheritdoc />
    /// <summary>
    /// Класс содержащий тест каналов передачи ПБК
    /// </summary>
    internal class TestTxClass : TestArincClass {
        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        private readonly List< SignalDescription > _testList = new List< SignalDescription > {
            // Устройство ВМ-7 канал
            //
            // Тестирование передатчиков
            //
            new SignalDescription( 0, "Вых.ПБК1", "", "A50/B50 OUT_ARINC_25",
                new List< string > { "PCI-429-4-3 №2 pin 40/60 КП01", "A47/B47 IN_ARINC_27" } ),
            new SignalDescription( 0, "Вых.ПБК2", "", "C50/D50 OUT_ARINC_26",
                new List< string > {  "PCI-429-4-3 №2 pin 41/61 КП02", "C46/D46 IN_ARINC_26" } ),
            new SignalDescription( 0, "Вых.ПБК3", "", "A51/B51 OUT_ARINC_27",
                new List< string > { "PCI-429-4-3 №2 pin 42/62 КП03", "A48/B48 IN_ARINC_29" } ),
            new SignalDescription( 0, "Вых.ПБК4", "", "C51/D51 OUT_ARINC_28",
                new List< string > { "PCI-429-4-3 №2 pin 43/63 КП04", "C47/D47 IN_ARINC_28" } ),
            new SignalDescription( 0, "Вых.ПБК5", "", "A52/B52 OUT_ARINC_29", "PCI-429-4-3 №2 pin 23/24 КП05" ),
            new SignalDescription( 0, "Вых.ПБК6", "", "C52/D52 OUT_ARINC_30", "PCI-429-4-3 №2 pin 3/4 КП06" )
        };

        /// <summary>
        /// Список конфигураций данных
        /// </summary>
        private readonly List< ArincTestConfig > _testConfig = new List< ArincTestConfig > {
            new ArincTestConfig( FREQ.F12, 0x11111111, 0, 1 ),
            new ArincTestConfig( FREQ.F12, 0x22222222, ArincDevice.TxData >> 3, ArincDevice.TxData >> 2 ),
            new ArincTestConfig( FREQ.F12, 0x33333333, ArincDevice.TxData >> 2, ArincDevice.TxData >> 1 ),
            new ArincTestConfig( FREQ.F12, 0x00000000, 0, ArincDevice.TxData ),
            new ArincTestConfig( FREQ.F12, 0x55555555, 0, ArincDevice.TxData ),
            new ArincTestConfig( FREQ.F12, 0xAAAAAAAA, 0, ArincDevice.TxData ),
            new ArincTestConfig( FREQ.F12, 0xFFFFFFFF, 0, ArincDevice.TxData ),
            new ArincTestConfig( FREQ.F100, 0x44444444, 0, 1 ),
            new ArincTestConfig( FREQ.F100, 0x55555555, ArincDevice.TxData >> 3, ArincDevice.TxData >> 2 ),
            new ArincTestConfig( FREQ.F100, 0x66666666, ArincDevice.TxData >> 2, ArincDevice.TxData >> 1 ),
            new ArincTestConfig( FREQ.F100, 0x00000000, 0, ArincDevice.TxData ),
            new ArincTestConfig( FREQ.F100, 0x55555555, 0, ArincDevice.TxData ),
            new ArincTestConfig( FREQ.F100, 0xAAAAAAAA, 0, ArincDevice.TxData ),
            new ArincTestConfig( FREQ.F100, 0xFFFFFFFF, 0, ArincDevice.TxData ),
        };

        ~TestTxClass() {
            _testList?.Clear();
            _testConfig?.Clear();
        }

        /// <summary>
        /// Подпрограмма теста каналов приема Arinc429
        /// </summary>
        /// <returns></returns>
        public int Start( int index ) {
            // Индекс СОМ-порта
            Index = index == ( int ) ConfigTestsClass.IdTest.A429Tx1 ? 0 : 1;
            //// Проверка заданного индекса
            //if ( Index < 0 || Index >= PortRs232.MaxDevice ) {
            //    throw new Exception< ParameterNotFoundExceptionArgs >( new ParameterNotFoundExceptionArgs(),
            //        $"Ошибка допустимости данных индекса {Index}" );
            //}
            ////Проверка используемых идентификаторов
            //if ( !App.TaskManager.IdEnabledsDictionary[ Index ].Rs232 ) {
            //    throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
            //        $"Ошибка! Инициализация идентификаторов {Index:D} порта RS232 не пройдена" );
            //}
            //if ( !App.TaskManager.IdEnabledsDictionary[ Index ].Arinc ) {
            //    throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
            //        $"Ошибка! Инициализация идентификаторов {Index:D} канала Arinc429 не пройдена" );
            //}
            // Список тестируемых каналов
            SignalList = _testList.Where( o => o.Device != null  &&
                                               o.Device == Index &&
                                               !string.IsNullOrWhiteSpace( o.Name ) ).ToList();
            // Список конфигураций данных
            ConfigList = _testConfig;
            return Design();
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        protected override void Config( PlaceSignalDescription signal, TypeChannel type, FREQ frequency ) {
            if ( signal.Protocol == Protocol.Pci429 ) {
                Pci429Func.Config( signal, type, frequency );
            } else {
                ArincFunc.Config( signal, type, frequency );
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        protected override void ResetData() => ArincFunc.ResetData( UsedRs232 );

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        protected override void ResetData( PlaceSignalDescription signal ) {
            if ( signal.Protocol == Protocol.Rs232 ) ArincFunc.ResetData( signal );
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="template"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        protected override void SetData( PlaceSignalDescription signal, uint template, int address, int count,
            int time ) => ArincFunc.SetData( signal, template, address, count );

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override uint[] GetData( PlaceSignalDescription signal, int count ) =>
            signal.Protocol == Protocol.Pci429
                ? Pci429Func.GetData( signal, count, 5000 )
                : ArincFunc.GetData( signal );

        /// <summary>
        /// 
        /// </summary>
        protected List< uint[][] > UpdateCounters() =>
            new List< uint[][] > { ArincFunc.GetCounters( UsedRs232 ), Pci429Func.GetCounters( UsedPci429 ) };

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        protected override uint GetCounters( PlaceSignalDescription signal ) =>
            signal.Protocol == Protocol.Pci429 ? Pci429Func.GetCounters( signal ) : ArincFunc.GetCounters( signal );

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        protected override void ResetTest() { }
    }
}