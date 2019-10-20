using System.Collections.Generic;
using System.Linq;

namespace WpfApplication.Models.Test.Rk
{
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    internal class TestVoltageClass : TestRkClass {
        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        private readonly List< SignalDescription > _testList = new List< SignalDescription > {
            // Устройство ВМ-7 канал
            new SignalDescription( 0, "РКВо15", "Вых.АСП-1", "", "PCI-1753 2 pin 33 port PA17" ),
            new SignalDescription( 0, "+27В лев.б.", "RKIS_0", "", "ВМ-7 word 0 pin 0" ),
            new SignalDescription( 0, "+27В прав.б.", "RKIS_1", "", "ВМ-7 word 0 pin 1" )
        };

        /// <summary>
        /// 
        /// </summary>
        ~TestVoltageClass() {
            _testList?.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Start( int id ) {
            // Индекс СОМ-порта
            Index = 0;
            // Контрольные значения
            DefaultOffConfig = 1;
            DefaultOnConfig  = 1;
            OneSignalIsChecked = true;
            ConfigList       = new List< byte > { 1 };
            // Список аналоговых сигналов приема
            SignalList = _testList.Where( o => o.Device != null  &&
                                               o.Device == Index &&
                                               !string.IsNullOrWhiteSpace( o.Name ) ).ToList();
            return Design();
        }

        protected override int InitTest() => 0;
        protected override void ResetTest() { }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        protected override void Set( PlaceSignalDescription signal, byte value ) { }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        protected override byte Get( PlaceSignalDescription signal ) => 0;//Pci1753Func.Get( signal );

    }
}