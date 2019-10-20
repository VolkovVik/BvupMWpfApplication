using System.Linq;
using System.Collections.Generic;
using WpfApplication.Models.Test.Arinc;
using WpfApplication.Models.Test.As;
using WpfApplication.Models.Test.InitPciCard;
using WpfApplication.Models.Test.LoadHexFile;
using WpfApplication.Models.Test.Rk;
using WpfApplication.ViewModels;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// Класс конфигурации программы
    /// </summary>
    public class ConfigProgramClass {
        /// <summary>
        /// Имя устройства
        /// </summary>
        public readonly string NameDevice;

        /// <summary>
        /// Путь доступа к НЕХ файлам
        /// </summary>
        public readonly string PathHexFiles;

        /// <summary>
        /// Картинка
        /// </summary>
        public readonly string ImageUri;

        /// <summary>
        /// Номер устройства
        /// </summary>
        public string Nomer;

        /// <summary>
        /// Оператор
        /// </summary>
        public string UserOperator;

        /// <summary>
        /// Представитель ОТК
        /// </summary>
        public string UserOtk;

        /// <summary>
        /// Представитель ВП МО
        /// </summary>
        public string UserVp;

        /// <summary>
        /// Температура
        /// </summary>
        public double Temp;

        /// <summary>
        /// Словарь настройки тестов
        /// </summary>
        public readonly Dictionary< int, ConfigTestsClass > DictCfgTest = new Dictionary< int, ConfigTestsClass >();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="pathHex"></param>
        /// <param name="imageUri"></param>
        public ConfigProgramClass( string deviceName = "блок", string pathHex = "hex\\",
            string imageUri = "pack://application:,,,/models/icon/icon.ico" ) {
            // Инициализация
            NameDevice   = deviceName;
            PathHexFiles = pathHex;
            ImageUri     = imageUri;
            Nomer        = "00000000";
            UserOperator = string.Empty;
            UserOtk      = string.Empty;
            UserVp       = string.Empty;
            Temp         = 20.0;
            SetTaskDict();
        }

        /// <summary>
        /// 
        /// </summary>
        ~ConfigProgramClass() {
            DictCfgTest?.Clear();
        }

        /// <summary>
        /// Подпрограмма добавления описания тестов устройства БВУП-М
        /// </summary>
        private void SetTaskDict() {
            // Заполнение словаря
            // Вкладка БВУП-М 
            // ReSharper disable once ConvertToConstant.Local
            var index_tab_l1 = 0;
            // ReSharper disable once ConvertToConstant.Local
            var name_tab_l1  = "БВУП-М";
            var index_tab_l2 = 0;

            // ReSharper disable RedundantAssignment
            //
            // Вкладка РК
            //
            var name_tab_l2 = "РК";
            var index_page  = 0;

            // Очистка словаря
            DictCfgTest.Clear();

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Rkv1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль разовых команд выдачи",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    48, new TestRkvClass().Start ) );

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Rkp1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль разовых команд приема",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    135, new TestRkpClass().Start ) );

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Place1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль кода признака места",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    9, new TestPlaceClass().Start ) );

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Led1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль светодиода",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    1, new TestLedClass().Start ) );
            //
            // Вкладка ПБК ( ВМ-7 ( К ), МУП-7 )
            //
            index_tab_l2++;
            name_tab_l2 = "ПБК";
            index_page  = 0;

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.A429Rx1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль каналов приема ПБК",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    351, new TestRxClass().Start ) );

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.A429Tx1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль каналов выдачи ПБК",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    56, new TestTxClass().Start ) );
            //
            // Вкладка АС ( ВМ-7 ( К ), МУП-7 )
            //
            index_tab_l2++;
            name_tab_l2 = "AС";
            index_page  = 0;

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Adc1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль входных АС",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    73, new TestAdcClass().Start ) );

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Dac1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль выходных АС",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    43, new TestDacClass().Start ) );

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Doz1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль ДОЗ типа CKT-220-1Д",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    182, new TestDozClass().Start ) );

            //
            // Вкладка питание ( ВМ-7 ( К ), МУП-7 )
            //
            index_tab_l2++;
            name_tab_l2 = "UB";
            index_page  = 0;

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.SupplyVoltage1,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль напряжений питания",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    14, new TestVoltageClass().Start ) );

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Temp,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.Test,
                    "Контроль датчика температуры",
                    index_tab_l1, index_tab_l2, index_page++,
                    name_tab_l1, name_tab_l2,
                    2, new TestTempClass().Start ) );
            //
            // Системные задачи
            //
            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Init,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.SysTask,
                    "Инициализация интерфейсов",
                    null, null, null, string.Empty, string.Empty,
                    200, InitPciCardClass.Init ) );

            DictCfgTest.Add( ( int ) ConfigTestsClass.IdTest.Load,
                new ConfigTestsClass( ConfigTestsClass.TypeTask.SysTask,
                    "Загрузка НЕХ-файлов",
                    null, null, null, string.Empty, string.Empty,
                    200, new ManagerLoadHexFileClass().InitTaskManager ) );
            // ReSharper restore RedundantAssignment
        }

        /// <summary>
        /// Подпрограмма сохранения привязки тестов к форме в словарь
        /// </summary>
        /// <param name="indexTab1"></param>
        /// <param name="indexTab2"></param>
        /// <param name="indexPage"></param>
        /// <param name="data"></param>
        public void SetControl( int? indexTab1, int? indexTab2, int? indexPage, TabControlViewModel data ) {
            if ( indexTab1 == null || indexTab2 == null || indexPage == null ) return;
            // Сохранение данных наприсованных элементов
            foreach ( var item in DictCfgTest.Values.Where( i =>
                indexTab1 == i.IndexTabL1 &&
                indexTab2 == i.IndexTabL2 &&
                indexPage == i.IndexPage ) ) {
                item.SetControl( data );
            }
        }

        /// <summary>
        /// Подпрограмма создания конфигурации вкладок первого уровня
        /// </summary>
        /// <param name="name"></param>
        public int GetNameTabL1( List< string > name ) {
            name.Clear();
            name.AddRange( DictCfgTest.Values.Where( i => i.Type == ConfigTestsClass.TypeTask.Test )
                                      .Select( item => item.NameTabL1 ).Distinct().ToList() );
            return name.Count;
        }

        /// <summary>
        /// Подпрограмма создания конфигурации вкладок первого уровня
        /// </summary>
        /// <param name="indexTabL1"></param>
        /// <param name="name"></param>
        public int GetNameTabL2( int indexTabL1, List< string > name ) {
            name.Clear();
            name.AddRange( DictCfgTest
                           .Values.Where( i => i.IndexTabL1 == indexTabL1 && i.Type == ConfigTestsClass.TypeTask.Test )
                           .Select( item => item.NameTabL2 ).Distinct().ToList() );
            return name.Count;
        }

        /// <summary>
        /// Подпрограмма получения максимального количества тестов на странице
        /// </summary>
        /// <returns></returns>
        public int GetMaxTestOnPage() {
            // Получение списка всех вкладок 2-ого уровня
            var test = DictCfgTest.Values.Where( i => i.Type == ConfigTestsClass.TypeTask.Test )
                                  .Select( item => item.NameTabL2 ).Distinct().ToList();
            // Получение списка количества тестов на каждой вкладке
            var count = new List< int >();
            foreach ( var item in test.Distinct().ToList() ) {
                count.Add( DictCfgTest.Values.Count( i => i.NameTabL2 == item ) );
            }
            return count.Max();
        }
    }
}