using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WpfApplication.Models.Function;

namespace WpfApplication.Models.Test {

    
    /// <summary>
    /// Класс содержащий шаблон тестов
    /// </summary>
    internal abstract class AbstractTestClass< T, TConfig, TResult > where T : SignalDescription {
        /// <summary>
        /// Дефолтная конфигурация - сигнал отсутствует
        /// </summary>
        protected TConfig DefaultOffConfig { get; set; }

        /// <summary>
        /// Дефолтная конфигурация - сигнал присутствует
        /// </summary>
        protected TConfig DefaultOnConfig { get; set; }

        /// <summary>
        /// Время ожидания установки сигнала
        /// </summary>
        protected int SignalSetupTime { get; set; } = 0;

        /// <summary>
        /// Признак проверки одного сигнала
        /// Контроль других сигналов не производится
        /// Контроль сброса и установки всех сигналов не производится
        /// </summary>
        // ReSharper disable once RedundantDefaultMemberInitializer
        protected bool OneSignalIsChecked { private get; set; } = false;

        /// <summary>
        /// Признак полного теста
        /// Контроль других сигналов производится
        /// Контроль сброса и установки всех сигналов производится
        /// </summary>
        // ReSharper disable once RedundantDefaultMemberInitializer
        protected bool FullTestIsChecked { private get; set; } = false;
        
        /// <summary>
        /// Объект работы с устройством
        /// </summary>
        protected readonly RkFunctionClass RkFunc = new RkFunctionClass();

        /// <summary>
        /// Объект работы с устройством
        /// </summary>
        protected readonly AsFunctionClass AsFunc = new AsFunctionClass();

        /// <summary>
        /// Объект работы с УПС-10
        /// </summary>
        protected readonly Upc10FunctionClass Upc10Func = new Upc10FunctionClass();

        /// <summary>
        /// Объект работы с устройством
        /// </summary>
        protected readonly Arinc429FunctionClass ArincFunc = new Arinc429FunctionClass();

        /// <summary>
        /// Объект работы с интерфейсной платой
        /// </summary>
        protected readonly Pci429FunctionClass Pci429Func = new Pci429FunctionClass();
        
        /// <summary>
        /// Объект работы с интерфейсной платой PCI-1721
        /// </summary>
        protected readonly Pci1721FunctionClass Pci1721Func = new Pci1721FunctionClass();

        /// <summary>
        /// Объект работы с интерфейсной платой PCI-1724U
        /// </summary>
        protected readonly Pci1724UFunctionClass Pci1724UFunc = new Pci1724UFunctionClass();

        /// <summary>
        /// Объект работы с интерфейсной платой PCI-1747U
        /// </summary>
        protected readonly Pci1747UFunctionClass Pci1747UFunc = new Pci1747UFunctionClass();

        /// <summary>
        /// Объект работы с интерфейсной платой
        /// </summary>
        protected readonly Pci1753FunctionClass Pci1753Func = new Pci1753FunctionClass();
        
        /// <summary>
        /// Индекс проверяемого устройства
        /// </summary>
        protected int Index;

        /// <summary>
        /// Список тестируемых сигналов
        /// </summary>
        protected List< T > SignalList;

        /// <summary>
        /// Список конфигураций данных
        /// </summary>
        protected List< TConfig > ConfigList;

        /// <summary>
        /// Тип теста
        /// </summary>
        private enum TypeTest {
            /// <summary>
            /// Бегущий "0"
            /// </summary>
            RunNull = 0,

            /// <summary>
            /// Бегущая "1"
            /// </summary>
            RunOne = 1
        }

        /// <summary>
        /// Тип теста 
        /// 1 - бегущая единица
        /// 0 - бегущий нуль
        /// </summary>
        private List< TypeTest > _typeTestList;

        /// <summary>
        /// Подпрограмма установки состояния устройства
        /// </summary>
        /// <returns></returns>
        protected int Start() {
            _typeTestList = FullTestIsChecked
                ? new List< TypeTest > { TypeTest.RunOne, TypeTest.RunNull }
                : new List< TypeTest > { TypeTest.RunOne };
            
            return Test();

            var count_err = 0;

            // Установка РКП и РКВ в исходное состояние
            // Установка АСП и АСВ в исходное состояние
            count_err += DeviceCommandClass.Passive( Index );
            count_err += Test();
            // Установка РКП и РКВ в активное состояние
            // Установка АСП и АСВ в активное состояние
            count_err += DeviceCommandClass.Activ( Index );
            count_err += Test();
            // Установка РКП и РКВ в исходное состояние
            // Установка АСП и АСВ в исходное состояние
            count_err += DeviceCommandClass.Passive( Index );

            return count_err;
        }

        /// <summary>
        /// Подпрограмма теста каналов выдачи Arinc429
        /// </summary>
        /// <returns></returns>
        private int Test() {
            // Количество ошибок в тесте
            var count_err = 0;

            // Подготовка теста
            count_err += InitTest();
            //
            // Цикл типов тестов
            //
            foreach ( var type in _typeTestList ) {
                var default_config = type == TypeTest.RunOne ? DefaultOffConfig : DefaultOnConfig;

                if ( FullTestIsChecked ) {
                    // Счетчик ошибок при проходе части теста
                    var count_err_label = 0;
                    var str = type == TypeTest.RunOne
                        ? "Контроль сброса всех сигналов"
                        : "Контроль установки всех сигналов";
                    CommonClass.SetText( str, 1, false, true, true );
                    // Тестирование
                    count_err_label += KernTest( default_config );
                    // Вывод результатов пользователю
                    CommonClass.SetResText( str, 1, 40, count_err_label == 0, separator1: true );
                    count_err += count_err_label;
                }
                //
                // Цикл каналов
                //
                foreach ( var signal in SignalList.Where( o => o != null ) ) {
                    CommonClass.SetText( $"Контроль {signal}", 1, separator1: true );
                    App.TaskManager.Log.WriteAsync( signal.Circuit );
                    // Счетчик ошибок в проверяемом канале Arinc429
                    var count_err_channel = 0;
                    //
                    // Цикл данных теста
                    //
                    foreach ( var config in ConfigList ) {
                        CommonClass.SetText( $"Контроль {config}", 2, false, true, true );
                        // Проверка токена отмены операции
                        App.TaskManager.СheckСancelToken();
                        App.MyWindows.ValueProgress++;
                        // Задание настроек
                        Config( config );
                        // Контроль интерфейса
                        var count_err_step = KernTest( signal, config, default_config );
                        // Вывод результатов пользователю
                        CommonClass.SetResText( $"Контроль {config}", 2, 50, count_err_step == 0, separator1: true );
                        count_err_channel += count_err_step;
                    }
                    // Вывод результатов пользователю
                    CommonClass.SetResText( $"Контроль {signal}", 1, 50, count_err_channel == 0, true,
                        separator1: true );
                    count_err += count_err_channel;
                }
            }
            // Завершение теста
            ResetTest();
            App.TaskManager.Log.WriteAsync( CommonClass.Separator );
            return count_err;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signals"></param>
        /// <param name="config"></param>
        /// <param name="dropp"></param>
        /// <returns></returns>
        private int KernTest( IList< T > signals, TConfig config, TConfig dropp ) {
            // Количество ошибок в тесте
            var count_err = 0;

            // Процедура инициализации шага теста
            foreach ( var signal in signals ) InitStepTest( signal );
            // Установка сигнала
            foreach ( var signal in signals ) Set( signal, config );
            // Ожидание установки сигнала
            Thread.Sleep( SignalSetupTime );
            // Подготовка данных к считыванию
            GetPrepare();
            // Прием и проверка данных
            foreach ( var signal in SignalList ) {
                var channel_is_tested = signals.Contains( signal );
                if ( !channel_is_tested && OneSignalIsChecked ) continue;
                var current_config = channel_is_tested ? config : dropp;
                // Получение данных
                var result = Get( signal, current_config, channel_is_tested );
                // Проверка данных
                count_err += Check( signal, result, current_config, channel_is_tested );
            }
            // Сброс сигнала
            if ( config.Equals( dropp) )
                foreach ( var signal in signals )
                    Set( signal, dropp );
            // Процедура завершения шага теста
            foreach ( var i_signal in signals ) ResetStepTest( i_signal );
            //
            // Вывод результата
            //
            if ( count_err != 0 && signals.Count == 1 )
                App.TaskManager.Log.WriteLineAsync( $"{$"Контроль {signals[ 0 ].Name}",-60} - отказ" );
            if ( count_err != 0 && signals.Count == SignalList.Count )
                App.TaskManager.Log.WriteLineAsync( $"{"Контроль всех сигналов",-60} - отказ" );
            // ReSharper disable once InvertIf
            if ( count_err != 0 && signals.Count > 1 && signals.Count < SignalList.Count ) {
                var str = "Контроль следующих сигналов" + Environment.NewLine;
                str = signals.Aggregate( str,
                    ( current, isignal ) => current + $"    {isignal.Name}" + Environment.NewLine );
                App.TaskManager.Log.WriteLineAsync( $"{str}{"Контроль",-60} - отказ" );
            }
            return count_err;
        }

        private int KernTest( T signals, TConfig config, TConfig dropp ) =>
            KernTest( new List< T > { signals }, config, dropp );

        private int KernTest( TConfig config ) => KernTest( SignalList, config, config );

        /// <summary>
        /// Процедура инициализации теста
        /// </summary>
        /// <returns></returns>
        protected virtual int InitTest() => 0;

        /// <summary>
        /// Процедура завершения теста
        /// </summary>
        protected virtual void ResetTest() { }

        /// <summary>
        /// Процедура инициализации шага теста
        /// </summary>
        protected virtual void InitStepTest( T signal ) {}

        /// <summary>
        /// Процедура завершения шага теста
        /// </summary>
        protected virtual void ResetStepTest( T signal ) {}
        
        /// <summary>
        /// Конфигурация теста
        /// </summary>
        /// <param name="config"></param>
        protected virtual void Config( TConfig config ) {}

        /// <summary>
        /// Предварительное получение данных
        /// </summary>
        /// <returns></returns>
        protected virtual void GetPrepare() { }

        /// <summary>
        /// Установка сигнала
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        protected abstract void Set( T signal, TConfig value );

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="config"></param>
        /// <param name="isTestedChannel"></param>
        /// <returns></returns>
        protected abstract List< TResult > Get( T signal, TConfig config, bool isTestedChannel = true );

        /// <summary>
        /// Сравнение данных
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="result"></param>
        /// <param name="control"></param>
        /// <param name="isTestedChannel"></param>
        /// <returns></returns>
        protected abstract int Check( T signal, List<TResult> result, TConfig control, bool isTestedChannel = true );
    }
}