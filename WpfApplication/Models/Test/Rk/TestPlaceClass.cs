using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.Rk
{
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    internal class TestPlaceClass : TestRkClass {
        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        private readonly List< SignalDescription > _testList = new List< SignalDescription > {
            // Устройство ВМ-7 канал
            new SignalDescription( 0, "РКПо9", "PLACE_DEVICE_0", "PCI-1753 1 pin 59 port PB20",
                "ВМ-7 word 0 pin 16" ),
            new SignalDescription( 0, "РКПо10", "PLACE_DEVICE_1", "PCI-1753 1 pin 60 port PB21",
                "ВМ-7 word 0 pin 17" )
        };

        /// <summary>
        /// 
        /// </summary>
        ~TestPlaceClass() {
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
            if( Index < 0 || Index >= PortRs232.MaxDevice ) {
                throw new Exception<ParameterNotFoundExceptionArgs>( new ParameterNotFoundExceptionArgs(),
                    $"Ошибка допустимости данных индекса {Index}" );
            }
            // Проверка используемых идентификаторов
            if( !App.TaskManager.IdEnabledsDictionary[Index].Rs232 ) {
                throw new Exception<IdEnabledsNotValidExceptionArgs>( new IdEnabledsNotValidExceptionArgs(),
                    $"Ошибка! Инициализация идентификаторов {Index:D} порта RS232 не пройдена" );
            }
            if( !App.TaskManager.IdEnabledsDictionary[Index].Rk ) {
                throw new Exception<IdEnabledsNotValidExceptionArgs>( new IdEnabledsNotValidExceptionArgs(),
                    $"Ошибка! Инициализация идентификаторов {Index:D} канала RK не пройдена" );
            }
            // Контрольные значения
            DefaultOffConfig = 0;
            DefaultOnConfig = 1;
            ConfigList = new List<byte> { 0, 1 };
            FullTestIsChecked = true;
            // Список аналоговых сигналов приема
            SignalList = _testList.Where( o => o.Device != null &&
                                               o.Device == Index &&
                                               !string.IsNullOrWhiteSpace( o.Name ) ).ToList();
            return Design();
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