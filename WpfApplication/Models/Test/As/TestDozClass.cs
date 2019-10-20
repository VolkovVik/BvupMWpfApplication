using System;
using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.As {
    /// <summary>
    /// Класс контроля приема сигналов от датчика отклонения закрылков (ДОЗ) типа СКТ-220-1Д
    /// </summary>
    internal class TestDozClass : TestAsClass {
        /// <summary>
        /// Максимально возможное отклонение угла ДОЗ
        /// </summary>
        private const double Delta = .3;

        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        private readonly List< SignalDescription > _testList = new List< SignalDescription > {
            // Устройство ВМ-7 канал
            //
            // Тестирование аналоговых сигналов приема ±10В
            //
            new SignalDescription( 0, "Угол ДОЗ", "Синусно-косинусный трансформатор",
                new List< string > {
                    "ПК PCI-1721: 68-pin SCSI pin 67 port VOUT0",
                    "ПК PCI-1721: 68-pin SCSI pin 67 port VOUT1"
                }, "УПС-10 word 250 pin 0" )
        };

        ~TestDozClass() {
            _testList?.Clear();
        }
        
        /// <summary>
        /// Подпрограмма контроля аналоговых сигналов приема
        /// </summary>
        /// <returns></returns>
        public int Start( int id ) {
            // Индекс СОМ-порта
            Index = id == ( int ) ConfigTestsClass.IdTest.Doz1 ? 0 : 1;
            // Проверка заданного индекса
            if( Index < 0 || Index >= PortRs232.MaxDevice ) {
                throw new Exception<ParameterNotFoundExceptionArgs>( new ParameterNotFoundExceptionArgs(),
                    $"Ошибка допустимости данных индекса {Index}" );
            }
            // Проверка используемых идентификаторов
            if( !App.TaskManager.IdEnabledsDictionary[Index].Rs232 ) {
                throw new Exception<IdEnabledsNotValidExceptionArgs>( new IdEnabledsNotValidExceptionArgs(),
                    "Ошибка! Инициализация идентификаторов RS232 не пройдена" );
            }
            if( !App.TaskManager.IdEnabledsDictionary[Index].Adc ) {
                throw new Exception<IdEnabledsNotValidExceptionArgs>( new IdEnabledsNotValidExceptionArgs(),
                    "Ошибка! Инициализация идентификаторов АС не пройдена" );
            }
            if( !App.TaskManager.IdEnabledsDictionary[Index].Arinc ) {
                throw new Exception<IdEnabledsNotValidExceptionArgs>( new IdEnabledsNotValidExceptionArgs(),
                    "Ошибка! Инициализация идентификаторов Arinc429 не пройдена" );
            }
            // Контрольные значения
            DefaultOffConfig   = 0.0;
            DefaultOnConfig    = 90.0;
            OneSignalIsChecked = true;
            ConfigList         = new List< double > { 0, 30.0, 45.0, 60.0, 90.0 };
            // Список аналоговых сигналов приема
            foreach ( var item in _testList ) item.Delta = Delta;
            SignalList = _testList.Where( o => o.Device != null  &&
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
        protected override void Set( PlaceSignalDescription signal, double value ) {
            // Рассчет контрольного занчения синуса и косинуса угла поворота ДОЗ
            var ideal_angle = Math.PI * value / 180.0;
            if ( signal.Signal == "ПК PCI-1721: 68-pin SCSI pin 67 port VOUT0" ) {
                var sin_voltage = ( ushort ) ( 5 * Math.Sin( ideal_angle ) );
                // Выдача контрольных значений в виде аналоговых сигналов из платы PCI-1721
                Pci1721Func.Set( signal, sin_voltage );
            } else {
                var cos_voltage = ( ushort ) ( 5 * Math.Cos( ideal_angle ) );
                // Выдача контрольных значений в виде аналоговых сигналов из платы PCI-1721
                Pci1721Func.Set( signal, cos_voltage );
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override double Get( PlaceSignalDescription signal, int count ) {
            // Получение эхо-РКВ по каналу Arinc429
            var arinc_value = Upc10Func.GetWord( signal );
            // Проверка значения угла ДОЗ
            var test_value = ( double ) ( ( arinc_value >> 16 ) & 0xFFFU );
            return test_value * 4096 / 360;
        }
    }
}