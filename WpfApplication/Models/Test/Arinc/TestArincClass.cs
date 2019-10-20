using System;
using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.PciCard.arinc429;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.Arinc {
    /// <summary>
    /// Тип канала: 
    /// 1 - входной канал
    /// 0 - выходной канал
    /// </summary>
    internal enum TypeChannel : byte {
        Tx = 0,
        Rx = 1
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ArincDevice {
        /// <summary>
        /// Максимальное количество каналов приема
        /// </summary>
        public const int RxChannel = 32;

        /// <summary>
        /// Максимальное количество данных приема
        /// </summary>
        public const int RxData = 256;

        /// <summary>
        /// Максимальное количество каналов выдачи
        /// </summary>
        public const int TxChannel = 8;

        /// <summary>
        /// Максимальное количество данных выдачи
        /// </summary>
        public const int TxData = 256;

        /// <summary>
        /// Время выдачи одного слова на частоте 12.5 КГц, в мс
        /// 4T синхросигнал перед пакетом + 32T данные + 4...40T задержка между словами
        /// </summary>
        public const double Time12 = 1 / 12.5 * ( 32 + 40 );

        /// <summary>
        /// Время выдачи одного слова на частоте 100 КГц, в мс
        /// 4T синхросигнал перед пакетом + 32T данные + 4...40T задержка между словами
        /// </summary>
        public const double Time100 = ( double ) 1 / 100 * ( 32 + 40 );

        /// <summary>
        /// Подпрограмма рассчета бита четности слова Arinc429
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static uint GetWordParity( uint word ) {
            var count = 0;
            // Цикл подсчета единиц
            for ( var i = 0; i < 31; i++ )
                if ( ( ( word >> i ) & 0x1 ) > 0 )
                    count++;
            // Проверка количества единиц
            return ( count & 0x1 ) == 0 ? word | 0x80000000U : word & 0x7FFFFFFFU;
        }
    }

    /// <summary>
    /// Тест каналов с изменяемым напряжением
    /// </summary>
    public class ArincTestConfig {
        /// <summary>
        /// Частота обмена
        /// </summary>
        public readonly FREQ Frequency;

        /// <summary>
        /// Шаблон
        /// </summary>
        public readonly uint Template;

        /// <summary>
        /// Начальный адрес выдачи данных
        /// </summary>
        public readonly int Address;

        /// <summary>
        /// Количество выдаваемых слов Arinc
        /// </summary>
        public readonly int Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="template"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        public ArincTestConfig( FREQ frequency, uint template, int address, int count ) {
            Frequency = frequency;
            Template  = template;
            Address   = address;
            Count     = count;
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        public ArincTestConfig() : this( FREQ.OFF, 0, 0, 0 ) { }

        public override string ToString() =>
            $"шаблона 0x{Template,8:X8} {Count,3:000} слов с адреса {Address,3:0000} " +
            $"на частоте {( FREQ.F12 == Frequency ? "12.5 кГц" : "100 кГц " )}";
    }

    /// <summary>
    /// Входные данные
    /// </summary>
    public class ArincTestResultData {
        /// <summary>
        /// Массив обновленных данных
        /// </summary>
        public uint[] Data;

        /// <summary>
        /// Счетчик обновленных слов
        /// </summary>
        public uint Counter;
    }

    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    internal abstract class TestArincClass : 
        AbstractTestClass< SignalDescription, ArincTestConfig, ArincTestResultData > {

        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        protected List< int > UsedPci429 = new List< int >();

        /// <summary>
        /// 
        /// </summary>
        protected List< int > UsedRs232 = new List< int >();

        /// <summary>
        /// Подпрограмма установки состояния устройства
        /// </summary>
        /// <returns></returns>
        protected int Design() {
            //
            // Получение списка индексов используемых интерфейсных плат
            //
            UsedPci429.Clear();
            foreach ( var i_signal in SignalList ) {
                UsedPci429.AddRange(
                    i_signal.Output.Where( i => i.Protocol == Protocol.Pci429 &&
                                                i.Device   != null            &&
                                                i.Device   >= 0               &&
                                                i.Device   < App.TaskManager.PortArinc.MaxDevice )
                            .Select( i => ( int ) i.Device ) );
                UsedPci429.AddRange(
                    i_signal.Input.Where( i => i.Protocol == Protocol.Pci429 &&
                                               i.Device   != null            &&
                                               i.Device   >= 0               &&
                                               i.Device   < App.TaskManager.PortArinc.MaxDevice )
                            .Select( i => ( int ) i.Device ) );
            }
            UsedPci429 = UsedPci429.Distinct().ToList();
            //
            // Получение списка индексов используемых устройств
            //
            UsedRs232.Clear();
            foreach ( var i_signal in SignalList ) {
                UsedRs232.AddRange( i_signal.Input.Where( i => i.Protocol == Protocol.Rs232 &&
                                                               i.Device   != null           &&
                                                               i.Device   >= 0              &&
                                                               i.Device   < PortRs232.MaxDevice )
                                            .Select( i => ( int ) i.Device ) );
            }
            UsedRs232 = UsedRs232.Distinct().ToList();

            DefaultOffConfig = new ArincTestConfig();
            DefaultOnConfig  = new ArincTestConfig();
            // Запуск тестирования
            return Start();
        }


        ///<remarks>
        /// Перегруженные функции
        ///</remarks>


        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        protected override int InitTest() {
            // Сброс плат PCI429 
            //foreach ( var i in UsedPci429 ) App.TaskManager.PortArinc.Reset( i );
            return 0;
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        protected override void ResetTest() {
            // Сброс плат PCI429 
            //foreach ( var i in UsedPci429 ) App.TaskManager.PortArinc.Reset( i );
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        protected override void InitStepTest( SignalDescription signal ) {
            // Запуск плат PCI429
            //foreach( var i in UsedPci429 ) App.TaskManager.PortArinc.Start( i );
            // Сброс обновленных слов
            foreach ( var i_signal in signal.Input ) {
                App.TaskManager.Log.WriteLineAsync(
                    $"{"Сброс данных",-22} {i_signal?.Name,-10} в  {i_signal?.Protocol,8}#{i_signal?.Device,2:D2} channel#{i_signal?.Channel,2:D2}" );
                ResetData( i_signal );
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        protected override void ResetStepTest( SignalDescription signal ) {
            // Сброс плат PCI429 
            //foreach ( var i in UsedPci429 ) App.TaskManager.PortArinc.Reset( i );
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        protected override void Config( ArincTestConfig config ) {
            // Цикл сигналов
            foreach ( var signal in SignalList ) {
                // Цикл каналов выдачи
                foreach ( var i_signal in signal.Output ) {
                    App.TaskManager.Log.WriteLineAsync($"{"Настройка выдачи",-22} {i_signal?.Name,-10} в  {i_signal?.Protocol,8}#{i_signal?.Device,2:D2} channel#{i_signal?.Channel,2:D2}  = {config.Frequency:D}" );
                    Config( i_signal, TypeChannel.Tx, config.Frequency );
                }
                // Цикл каналов приема
                foreach ( var i_signal in signal.Input ) {
                    App.TaskManager.Log.WriteLineAsync($"{"Настройка приема",-22} {i_signal?.Name,-10} в  {i_signal?.Protocol,8}#{i_signal?.Device,2:D2} channel#{i_signal?.Channel,2:D2}  = {config.Frequency:D}" );
                    Config( i_signal, TypeChannel.Rx, config.Frequency );
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="config"></param>
        protected override void Set( SignalDescription signal, ArincTestConfig config ) {
            foreach ( var i_signal in signal.Output ) {
                App.TaskManager.Log.WriteLineAsync(
                    $"{"Запись данных",-22} {i_signal?.Name,-10} в  {i_signal?.Protocol,8}#{i_signal?.Device,2:D2} channel#{i_signal?.Channel,2:D2} => {config.Template,8:X8}" );
                var time = ( FREQ.F12 == config.Frequency ? ArincDevice.Time12 : ArincDevice.Time100 ) * config.Count +
                           50;
                SetData( i_signal, config.Template, config.Address, config.Count, ( int ) time );
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="config"></param>
        /// <param name="isTestedChannel"></param>
        /// <returns></returns>
        protected override List< ArincTestResultData > Get( SignalDescription signal, ArincTestConfig config,
            bool isTestedChannel = true ) {
            var list = new List< ArincTestResultData >();

            foreach ( var i_signal in signal.Input ) {
                var result = new ArincTestResultData();

                string str;
                if ( isTestedChannel ) {
                    result.Data = GetData( i_signal, config.Count );
                    str =
                         $"{"Чтение данных",-22} {i_signal?.Name,-10} из {i_signal?.Protocol,8}#{i_signal?.Device,2:D2} channel#{i_signal?.Channel,2:D2} <= ";
                    str += Environment.NewLine;
                    for( var i = 0; i < result.Data.Length; i++ ) {
                        str += ( i != 0 && ( i & 0x7 ) == 0 ? Environment.NewLine : "" ) + $"{result.Data[i]:X8} ";
                    }
                } else {
                    result.Counter = GetCounters( i_signal );
                    str =
                         $"{"Чтение счетчиков",-22} {i_signal?.Name,-10} из {i_signal?.Protocol,8}#{i_signal?.Device,2:D2} {"<=",13} {result.Counter,8:X8}";
                }
                App.TaskManager.Log.WriteLineAsync( str );
                list.Add( result );
            }
            return list;
        }


        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="result"></param>
        /// <param name="config"></param>
        /// <param name="isTestedChannel"></param>
        /// <returns></returns>
        protected override int Check( SignalDescription signal, List< ArincTestResultData > result,
            ArincTestConfig config, bool isTestedChannel = true ) =>
            signal.Input.Select( ( t, i ) => isTestedChannel
                      ? CheckData( t, config, result[ i ].Data )
                      : CheckCounters( t, result[ i ].Counter ) )
                  .Sum();


        ///<remarks>
        /// Абстрактные функции
        ///</remarks>


        /// <summary>
        /// Конфигурация канала
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        protected abstract void Config( PlaceSignalDescription signal, TypeChannel type, FREQ frequency );

        /// <summary>
        /// Сброс обновленных данных
        /// </summary>
        protected abstract void ResetData();

        /// <summary>
        /// Сброс обновленных данных
        /// </summary>
        /// <param name="signal"></param>
        protected abstract void ResetData( PlaceSignalDescription signal );

        /// <summary>
        /// Выдача данных
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="template"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        protected abstract void SetData( PlaceSignalDescription signal, uint template, int address, int count,
            int time );

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected abstract uint[] GetData( PlaceSignalDescription signal, int count );

        /// <summary>
        /// Получение счетчика обновленных слов
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        protected abstract uint GetCounters( PlaceSignalDescription signal );


        /// <remarks>
        ///  Реализованные функции
        /// </remarks>
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="config"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int CheckData( PlaceSignalDescription signal, ArincTestConfig config,
            IReadOnlyList< uint > data ) {
            var str       = string.Empty;
            var count_err = 0;
            // Сброс адресной части 256 слов = 0xFF
            var template          = config.Template & 0xFFFFFF00U;
            var count_update_word = 0;
            // Проверка принятых данных
            for ( var i = 0; i < ArincDevice.RxData; i++ ) {
                // Подсчет количества обновленных слов
                count_update_word += ( data[ i ] & 0xFF ) == i ? 1 : 0;
                // Проверка слов
                if ( i < config.Address || i >= config.Address + config.Count ) {
                    // В данной области памяти не должно быть обновленных слов
                    // Адреса всех слов в данной области должны быть инвертированны
                    // ReSharper disable once InvertIf
                    if ( ( ~i & 0xFF ) != ( data[ i ] & 0xFF ) ) {
                        // Ошибка адреса
                        count_err++;
                        str +=
                            $"{i,3:D3} {"Ошибка адреса",15}   тест = xxxxxx{~i                      & 0xFF,2:X2} " +
                            $"принято = {data[ i ],8:X8} xor = xxxxxx{( data[ i ] ^ ( ~i & 0xFF ) ) & 0xFF,2:X2}"  +
                            Environment.NewLine;
                    }
                } else {
                    // Все слова в данной области памяти должны быть обновлены
                    if ( i != ( data[ i ] & 0xFF ) ) {
                        // Ошибка обновления слова
                        count_err++;
                        str +=
                            $"{i,3:D3} {"Нет данных",15}   тест = xxxxxx{i,2:X2} "                    +
                            $"принято = {data[ i ],8:X8} xor = xxxxxx{( data[ i ] ^ i ) & 0xFF,2:X2}" +
                            Environment.NewLine;
                    } else {
                        // Проверка данных
                        var word_templ = ArincDevice.GetWordParity( template | ( uint ) ( i & 0xFF ) );
                        if ( word_templ == data[ i ] ) {
                            continue;
                        }
                        // Ошибка данных
                        count_err++;
                        str +=
                            $"{i,3:D3} {"Ошибка данных",15}   тест = {word_templ,8:X8} "      +
                            $"принято = {data[ i ],8:X8} xor = {data[ i ] ^ word_templ,8:X8}" +
                            Environment.NewLine;
                    }
                }
            }
            // Проверка счетчика обновленных слов
            if ( config.Count != count_update_word ) {
                // Ошибка
                // Количество обновленных слов не совпадает с количеством выданных
                count_err++;
                str += $"{"Ошибка размера",24} выдано {config.Count:D4} => принято {count_update_word:D4}" +
                       Environment.NewLine;
            }
            // Вывод пользователю
            if( count_err != 0 )
                App.TaskManager.Log.WriteAsync(
                    $"{"Контроль данных",-22} {signal?.Name,-10} - {signal?.Signal,-24} - {"отказ" + Environment.NewLine + str}" );
            return count_err;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="counters"></param>
        /// <returns></returns>
        private static int CheckCounters( PlaceSignalDescription signal, IEnumerable< uint > counters ) {
            var count_err = 0;
            var str       = string.Empty;

            // Проверка занчений счетчиков обновленных слов
            foreach ( var t in counters ) {
                // Проверка отсуствия лишних слов
                if ( t == 0 ) continue;
                count_err++;
                str += $"{signal.Channel,3:D3} {"Ошибка данных",15} обновлено {t,1:D} слов" + Environment.NewLine;
            }
            // Вывод пользователю
            if ( count_err != 0 )
                App.TaskManager.Log.WriteAsync(
                    $"{"Контроль счетчиков",-22} {signal?.Name,-10} - {signal?.Signal,-24} - {"отказ" + Environment.NewLine + str}" );
            return count_err;
        }

        private static int CheckCounters( PlaceSignalDescription signal, uint counters ) =>
            CheckCounters( signal, new[] { counters } );
    }
}