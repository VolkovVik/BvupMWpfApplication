using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.As {
    /// <inheritdoc />
    /// <summary>
    /// Класс контроля аналоговых сигналов выдачи
    /// </summary>
    internal class TestDacClass : TestAsClass {
        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        private readonly List< SignalDescription > _testList = new List< SignalDescription > {
            // Устройство ВМ-7 канал
            //
            // Тестирование аналоговых сигналов выдачи ±10В
            //
            new SignalDescription( 0, "АСВ1", "", "OUT_DAC_0", "ПК PCI-1747U: 68-pin SCSI pin 68 port AI0" ),
            new SignalDescription( 0, "АСВ2", "", "OUT_DAC_1", "ПК PCI-1747U: 68-pin SCSI pin 34 port AI1" ),
            new SignalDescription( 0, "АСВ3", "", "OUT_DAC_2", "ПК PCI-1747U: 68-pin SCSI pin 67 port AI2" ),
            new SignalDescription( 0, "АСВ4", "", "OUT_DAC_3", "ПК PCI-1747U: 68-pin SCSI pin 33 port AI3" ),
            new SignalDescription( 0, "АСВ5", "", "OUT_DAC_4", new List< string > {
                "ПК PCI-1747U: 68-pin SCSI pin 66 port AI4",
                "IN_ANALOG_3_2 Код управления 0001 АЦП №3"
            } ),
            new SignalDescription( 0, "АСВ6", "", "OUT_DAC_5", new List< string > {
                "ПК PCI-1747U: 68-pin SCSI pin 32 port AI5",
                "IN_ANALOG_3_3 Код управления 0010 АЦП №3"
            } ),
            new SignalDescription( 0, "АСВ7", "", "OUT_DAC_6", new List< string > {
                "ПК PCI-1747U: 68-pin SCSI pin 65 port AI6",
                "IN_ANALOG_3_4 Код управления 0011 АЦП №3"
            } )
        };

        ~TestDacClass() {
            _testList?.Clear();
        }

        /// <summary>
        /// Подпрограмма запуска контроля аналоговых сигналов выдачи
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Start( int id ) {
            // Индекс СОМ-порта
            Index = id == ( int ) ConfigTestsClass.IdTest.Dac1 ? 0 : 1;
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
            if ( !App.TaskManager.IdEnabledsDictionary[ Index ].Dac ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    "Ошибка! Инициализация идентификаторов АС не пройдена" );
            }
            if ( !App.TaskManager.IdEnabledsDictionary[ Index ].Arinc ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    "Ошибка! Инициализация идентификаторов Arinc429 не пройдена" );
            }
            // Контрольные значения
            DefaultOffConfig = 0.0;
            DefaultOnConfig  = 9.9;
            ConfigList       = new List<double> { -9.9, -5, -0.1, 0.1, 5, 9.9 };
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
        protected override void Set(  PlaceSignalDescription signal, double value ) => AsFunc.Set( signal, value );

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override double Get( PlaceSignalDescription signal, int count ) {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch ( signal.Protocol ) {
                case Protocol.Pci1747U:
                    return Pci1747UFunc.Get( signal, count );
                case Protocol.Rs232:
                    return AsFunc.Get( signal, count );
            }
            throw new System.NotImplementedException();
        }
    }
}