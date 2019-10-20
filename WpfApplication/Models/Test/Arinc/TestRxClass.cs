using System;
using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.arinc429;

namespace WpfApplication.Models.Test.Arinc {
    /// <inheritdoc />
    /// <summary>
    /// Класс содержащий тест каналов приема ПБК
    /// </summary>
    internal class TestRxClass : TestArincClass {
        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        private readonly List< SignalDescription > _testList = new List< SignalDescription > {
            // Устройство ВМ-7 канал
            //
            // Тестирование приемников
            //
            new SignalDescription( 0, "Вх.ПБК01", "", "PCI-429-4-3 №1 pin 32/12 КВ01", "A34/B34 IN_ARINC_01" ),
            new SignalDescription( 0, "Вх.ПБК02", "", "PCI-429-4-3 №1 pin 33/13 КВ02", "C34/D34 IN_ARINC_02" ),
            new SignalDescription( 0, "Вх.ПБК03", "", "PCI-429-4-3 №1 pin 34/14 КВ03", "A35/B35 IN_ARINC_03" ),
            new SignalDescription( 0, "Вх.ПБК04", "", "PCI-429-4-3 №1 pin 35/15 КВ04", "C35/D35 IN_ARINC_04" ),
            new SignalDescription( 0, "Вх.ПБК05", "", "PCI-429-4-3 №1 pin 36/16 КВ05", "A36/B36 IN_ARINC_05" ),
            new SignalDescription( 0, "Вх.ПБК06", "", "PCI-429-4-3 №1 pin 37/17 КВ06", "C36/D36 IN_ARINC_06" ),
            new SignalDescription( 0, "Вх.ПБК07", "", "PCI-429-4-3 №1 pin 38/18 КВ07", "A37/B37 IN_ARINC_07" ),
            new SignalDescription( 0, "Вх.ПБК08", "", "PCI-429-4-3 №1 pin 20/19 КВ08", "C37/D37 IN_ARINC_08" ),
            new SignalDescription( 0, "Вх.ПБК09", "", "PCI-429-4-3 №1 pin 59/39 КВ09", "A38/B38 IN_ARINC_09" ),
            new SignalDescription( 0, "Вх.ПБК10", "", "PCI-429-4-3 №1 pin 58/78 КВ10", "C38/D38 IN_ARINC_10" ),
            new SignalDescription( 0, "Вх.ПБК11", "", "PCI-429-4-3 №1 pin 57/77 КВ11", "A39/B39 IN_ARINC_11" ),
            new SignalDescription( 0, "Вх.ПБК12", "", "PCI-429-4-3 №1 pin 56/76 КВ12", "C39/D39 IN_ARINC_12" ),
            new SignalDescription( 0, "Вх.ПБК13", "", "PCI-429-4-3 №1 pin 54/74 КВ13", "A40/B40 IN_ARINC_13" ),
            new SignalDescription( 0, "Вх.ПБК14", "", "PCI-429-4-3 №1 pin 53/73 КВ14", "C40/D40 IN_ARINC_14" ),
            new SignalDescription( 0, "Вх.ПБК15", "", "PCI-429-4-3 №1 pin 52/72 КВ15", "A41/B41 IN_ARINC_15" ),
            new SignalDescription( 0, "Вх.ПБК16", "", "PCI-429-4-3 №1 pin 51/71 КВ16", "C41/D41 IN_ARINC_16" ),
            new SignalDescription( 0, "Вх.ПБК17", "", "PCI-429-4-3 №2 pin 32/12 КВ01", "A42/B42 IN_ARINC_17" ),
            new SignalDescription( 0, "Вх.ПБК18", "", "PCI-429-4-3 №2 pin 33/13 КВ02", "C42/D42 IN_ARINC_18" ),
            new SignalDescription( 0, "Вх.ПБК19", "", "PCI-429-4-3 №2 pin 34/14 КВ03", "A43/B43 IN_ARINC_19" ),
            new SignalDescription( 0, "Вх.ПБК20", "", "PCI-429-4-3 №2 pin 35/15 КВ04", "C43/D43 IN_ARINC_20" ),
            new SignalDescription( 0, "Вх.ПБК21", "", "PCI-429-4-3 №2 pin 36/16 КВ05", "A44/B44 IN_ARINC_21" ),
            new SignalDescription( 0, "Вх.ПБК22", "", "PCI-429-4-3 №2 pin 37/17 КВ06", "C44/D44 IN_ARINC_22" ),
            new SignalDescription( 0, "Вх.ПБК23", "", "PCI-429-4-3 №2 pin 38/18 КВ07", "A45/B45 IN_ARINC_23" ),
            new SignalDescription( 0, "Вх.ПБК24", "", "PCI-429-4-3 №2 pin 20/19 КВ08", "C45/D45 IN_ARINC_24" ),
            new SignalDescription( 0, "Вх.ПБК25", "", "PCI-429-4-3 №2 pin 59/39 КВ09", "A46/B46 IN_ARINC_25" )
        };

        /// <summary>
        /// Список конфигураций данных
        /// </summary>
        private readonly List< ArincTestConfig > _testConfig = new List< ArincTestConfig > {
            new ArincTestConfig( FREQ.F12, 0x11111111, 0, 1 ),
            new ArincTestConfig( FREQ.F12, 0x22222222, ArincDevice.RxData >> 3, ArincDevice.RxData >> 2 ),
            new ArincTestConfig( FREQ.F12, 0x33333333, ArincDevice.RxData >> 2, ArincDevice.RxData >> 1 ),
            new ArincTestConfig( FREQ.F12, 0x00000000, 0, ArincDevice.RxData ),
            new ArincTestConfig( FREQ.F12, 0x55555555, 0, ArincDevice.RxData ),
            new ArincTestConfig( FREQ.F12, 0xAAAAAAAA, 0, ArincDevice.RxData ),
            new ArincTestConfig( FREQ.F12, 0xFFFFFFFF, 0, ArincDevice.RxData ),
            new ArincTestConfig( FREQ.F100, 0x44444444, 0, 1 ),
            new ArincTestConfig( FREQ.F100, 0x55555555, ArincDevice.RxData >> 3, ArincDevice.RxData >> 2 ),
            new ArincTestConfig( FREQ.F100, 0x66666666, ArincDevice.RxData >> 2, ArincDevice.RxData >> 1 ),
            new ArincTestConfig( FREQ.F100, 0x00000000, 0, ArincDevice.RxData ),
            new ArincTestConfig( FREQ.F100, 0x55555555, 0, ArincDevice.RxData ),
            new ArincTestConfig( FREQ.F100, 0xAAAAAAAA, 0, ArincDevice.RxData ),
            new ArincTestConfig( FREQ.F100, 0xFFFFFFFF, 0, ArincDevice.RxData ),
        };

        ~TestRxClass() {
            _testList?.Clear();
            _testConfig?.Clear();
        }

        /// <summary>
        /// Подпрограмма теста каналов приема Arinc429
        /// </summary>
        /// <returns></returns>
        public int Start( int index ) {

            throw new ArgumentException( "САМ ДУРАК" );


            // Индекс СОМ-порта
            Index = index == ( int ) ConfigTestsClass.IdTest.A429Rx1 ? 0 : 1;
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        protected override void ResetData() => ArincFunc.ResetData( UsedRs232 );

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        protected override void ResetData( PlaceSignalDescription signal ) => ArincFunc.ResetData( signal );

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="template"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        protected override void SetData( PlaceSignalDescription signal, uint template, int address, int count,
            int time ) => Pci429Func.SetData( signal, template, address, count, time );

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override uint[] GetData( PlaceSignalDescription signal, int count ) => ArincFunc.GetData( signal );

        /// <summary>
        /// 
        /// </summary>
        protected List< uint[][] > UpdateCounter() => new List< uint[][] > { ArincFunc.GetCounters( UsedRs232 ) };

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        protected override uint GetCounters( PlaceSignalDescription signal ) => ArincFunc.GetCounters( signal );

    }
}