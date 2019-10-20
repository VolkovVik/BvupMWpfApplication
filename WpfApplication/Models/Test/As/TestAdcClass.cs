using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.As {
    /// <inheritdoc />
    /// <summary>
    /// Класс контроля аналоговых сигналов приема
    /// </summary>
    internal class TestAdcClass : TestAsClass {
        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        private readonly List< SignalDescription > _testList = new List< SignalDescription > {
            // Устройство ВМ-7 канал
            //
            // Тестирование аналоговых сигналов приема ±10В
            //
            new SignalDescription( 0, "АСП1", "", "ПК PCI-1724U: 62-pin SCSI pin 2 port AO0",
                "IN_ANALOG_1_1 Код управления 0000 АЦП №1" ),
            new SignalDescription( 0, "АСП2", "", "ПК PCI-1724U: 62-pin SCSI pin 4 port AO1",
                "IN_ANALOG_2_1 Код управления 0000 АЦП №2" ),
            new SignalDescription( 0, "АСП3", "", "ПК PCI-1724U: 62-pin SCSI pin 6 port AO2",
                "IN_ANALOG_1_3 Код управления 0010 АЦП №1" ),
            new SignalDescription( 0, "АСП4", "", "ПК PCI-1724U: 62-pin SCSI pin 8 port AO3",
                "IN_ANALOG_1_4 Код управления 0011 АЦП №1" ),
            new SignalDescription( 0, "АСП5", "", "ПК PCI-1724U: 62-pin SCSI pin 10 port AO4",
                "IN_ANALOG_1_5 Код управления 0100 АЦП №1" ),
            new SignalDescription( 0, "АСП6", "", "ПК PCI-1724U: 62-pin SCSI pin 12 port AO5",
                "IN_ANALOG_1_6 Код управления 0101 АЦП №1" ),
            new SignalDescription( 0, "АСП7", "", "ПК PCI-1724U: 62-pin SCSI pin 14 port AO6",
                "IN_ANALOG_1_7 Код управления 0110 АЦП №1" ),
            new SignalDescription( 0, "АСП8", "", "ПК PCI-1724U: 62-pin SCSI pin 16 port AO7",
                "IN_ANALOG_2_1 Код управления 0000 АЦП №2" ),
            new SignalDescription( 0, "АСП9", "", "ПК PCI-1724U: 62-pin SCSI pin 23 port AO8",
                "IN_ANALOG_2_2 Код управления 0001 АЦП №2" ),
            new SignalDescription( 0, "АСП10", "", "ПК PCI-1724U: 62-pin SCSI pin 25 port AO9",
                "IN_ANALOG_2_3 Код управления 0010 АЦП №2" ),
            new SignalDescription( 0, "АСП11", "", "ПК PCI-1724U: 62-pin SCSI pin 27 port AO10",
                "IN_ANALOG_2_4 Код управления 0011 АЦП №2" ),
            new SignalDescription( 0, "АСП12", "", "ПК PCI-1724U: 62-pin SCSI pin 29 port AO11",
                "IN_ANALOG_2_5 Код управления 0100 АЦП №2" )
        };

        ~TestAdcClass() {
            _testList?.Clear();
        }

        /// <summary>
        /// Подпрограмма контроля аналоговых сигналов приема
        /// </summary>
        /// <returns></returns>
        public int Start( int id ) {
            // Индекс СОМ-порта
            Index = id == ( int ) ConfigTestsClass.IdTest.Adc1 ? 0 : 1;
            // Проверка заданного индекса
            if ( Index < 0 || Index >= PortRs232.MaxDevice ) {
                throw new Exception< ParameterNotFoundExceptionArgs >( new ParameterNotFoundExceptionArgs(),
                    $"Ошибка допустимости данных индекса {Index}" );
            }
            // Проверка используемых идентификаторов
            if ( !App.TaskManager.IdEnabledsDictionary[ Index ].Rs232 ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    "Ошибка! Инициализация идентификаторов RS232 не пройдена" );
            }
            if ( !App.TaskManager.IdEnabledsDictionary[ Index ].Adc ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    "Ошибка! Инициализация идентификаторов АС не пройдена" );
            }
            // Контрольные значения
            DefaultOffConfig = 0.0;
            DefaultOnConfig  = 9.9;
            ConfigList       = new List< double > { -9.9, -5, -0.1, 0.1, 5, 9.9 };
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
        protected override void Set( PlaceSignalDescription signal, double value ) =>
            Pci1724UFunc.Set( signal, value );

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override double Get( PlaceSignalDescription signal, int count ) =>
            AsFunc.Get( signal, count );
    }
}