using System;
using System.Collections.Generic;
using System.Threading;

namespace WpfApplication.Models.Test.As
{
    /// <summary>
    /// 
    /// </summary>
    public static class AsDevice
    {
        /// <summary>
        /// Максимальное количество каналов приема
        /// </summary>
        public const int AdcChannel = 64;

        /// <summary>
        /// Максимальное количество каналов выдачи
        /// </summary>
        public const int DacChannel = 8;

        /// <summary>
        /// Время ожидания установки сигнала
        /// </summary>
        public const int Time = 50;

        /// <summary>
        /// Коэффициент перевода из бинарных данных в напряжение
        /// Диапазон напряжений -10...10 = 20В
        /// 0x8000...0...0x7FF
        /// </summary>
        public const double Lsb = ( double ) 20 / 4096;
    }

    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    internal abstract class TestAsClass : AbstractTestClass< SignalDescription, double, double > {

        /// <summary>
        /// Подпрограмма установки состояния устройства
        /// </summary>
        /// <returns></returns>
        protected int Design() => Start();

        /// <inheritdoc />
        /// <summary>
        /// Рассчет смещения нуля при запуске теста
        /// </summary>
        /// <returns></returns>
        protected override int InitTest() => Offset();

        /// <inheritdoc />
        /// <summary>
        /// Сброс всех сигналов по окончанию теста
        /// </summary>
        protected override void ResetTest() =>Set();

        /// <inheritdoc />
        /// <summary>
        /// Выдача напряжения
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="config"></param>
        protected override void Set( SignalDescription signal, double config ) {
            // Цикл каналов выдачи
            foreach ( var i_signal in signal.Output ) {
                App.TaskManager.Log.WriteLineAsync(
                    $"{"Запись",-10} "                                                                 +
                    $"{i_signal?.Name,-10} в  {i_signal?.Protocol,10}#{i_signal?.Device,2:D2} "        +
                    ( i_signal?.Channel == null ? $"{"",-10} " : $"channel#{i_signal.Channel,2:D2} " ) +
                    ( i_signal?.Word    == null ? $"{"",-8} " : $"word#{i_signal.Word,3:D3} " )        +
                    ( i_signal?.Pin     == null ? $"{"",-6} " : $"pin#{i_signal.Pin,2:D2} " )          +
                    $" => {config,9:00.00000} В" );
                Set( i_signal, config );
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Получение напряжения
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="config"></param>
        /// <param name="isTestedChannel"></param>
        /// <returns></returns>
        protected override List< double > Get( SignalDescription signal, double config, bool isTestedChannel = true ) {
            var list = new List< double >();

            foreach( var i_signal in signal.Input ) {
                // Получение сигнала
                var readed_voltage = Get( i_signal, 3 );
                list.Add( readed_voltage );
                App.TaskManager.Log.WriteLineAsync(
                    $"{"Чтение",-10} "                                                                +
                    $"{i_signal.Name,-10} в  {i_signal.Protocol,10}#{i_signal.Device,2:D2} "          +
                    ( i_signal.Channel == null ? $"{"",-10} " : $"channel#{i_signal.Channel,2:D2} " ) +
                    ( i_signal.Word    == null ? $"{"",-8} " : $"word#{i_signal.Word,3:D3} " )        +
                    ( i_signal.Pin     == null ? $"{"",-6} " : $"pin#{i_signal.Pin,2:D2} " )          +
                    $" => {readed_voltage,9:00.00000} В" );
            }
            return list;
        }

        /// <inheritdoc />
        /// <summary>
        /// Проверка напряжения
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="result"></param>
        /// <param name="config"></param>
        /// <param name="isTestedChannel"></param>
        /// <returns></returns>
        protected override int Check( SignalDescription signal, List< double > result, double config, bool isTestedChannel = true ) {
            var count_err = 0;
            // Проверка принятых сигналов
            for ( var i = 0; i < signal.Input.Count; i++ ) {
                var i_signal = signal.Input[ i ];
                //
                // Отклонение напряжения
                //
                // Производиться проверка выданного сигнала 
                // Производиться проверка отсутствия других сигналов
                var delta = Math.Abs( result[ i ] - config - i_signal.Offset ?? 0 );
                App.TaskManager.Log.WriteLineAsync(
                    $"{"Отклонение",-10} "                                                            +
                    $"{i_signal.Name,-10} в  {i_signal.Protocol,10}#{i_signal.Device,2:D2} "          +
                    ( i_signal.Channel == null ? $"{"",-10} " : $"channel#{i_signal.Channel,2:D2} " ) +
                    ( i_signal.Word    == null ? $"{"",-8} " : $"word#{i_signal.Word,3:D3} " )        +
                    ( i_signal.Pin     == null ? $"{"",-6} " : $"pin#{i_signal.Pin,2:D2} " )          +
                    $" => {delta,9:00.00000} В" );
                //
                // Контроль отклонения
                //
                var count_err_as = signal.Delta < delta ? 1 : 0;
                App.TaskManager.Log.WriteLineAsync(
                    $"{"Контроль",-10} "                                +
                    $"{i_signal.Name + " - " + i_signal.Signal,-52} - " +
                    ( signal.Delta >= delta ? "норма" : "отказ" ) );
                count_err += count_err_as;
            }
            return count_err;
        }


        ///<remarks>
        /// Абстрактные функции
        ///</remarks>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        protected abstract void Set( PlaceSignalDescription signal, double value );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected abstract double Get( PlaceSignalDescription signal, int count );


        /// <remarks>
        ///  Реализованные функции
        /// </remarks>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Set( double value = 0 ) {
            // Цикл сигналов
            foreach( var signal in SignalList ) Set( signal, value );
        }

        /// <summary>
        /// Подпрограмма рассчета и контроля смещения 0
        /// </summary>
        /// <returns></returns>
        private int Offset() {
            // Количество ошибок в тесте
            var count_err = 0;

            // Рассчет смещения "0"
            // Контроль сброса всех сигналов
            CommonClass.SetText( "Рассчет смещения 0", 1, separator1: true, separator2: true );

            // Выдача сигнала
            Set();
            // Ожидание установки напряжения
            Thread.Sleep( AsDevice.Time );
            // Цикл cигналов списка
            foreach ( var signal in SignalList ) {
                // Проверка принятых сигналов
                foreach ( var i_signal in signal.Input ) {
                    if ( i_signal == null ) continue;
                    //
                    // Получение смещения 0
                    //
                    i_signal.Offset = Get( i_signal, 11 );
                    App.TaskManager.Log.WriteLineAsync(
                        $"{"Чтение",-10} "                                                                +
                        $"{i_signal.Name,-10} в  {i_signal.Protocol,10}#{i_signal.Device,2:D2} "          +
                        ( i_signal.Channel == null ? $"{"",-10} " : $"channel#{i_signal.Channel,2:D2} " ) +
                        ( i_signal.Word    == null ? $"{"",-8} " : $"word#{i_signal.Word,3:D3} " )        +
                        ( i_signal.Pin     == null ? $"{"",-6} " : $"pin#{i_signal.Pin,2:D2} " )          +
                        $" => {i_signal.Offset,9:00.00000} В" );
                    //
                    // Контроль отклонения
                    //
                    if ( signal.Delta < i_signal.Offset ) count_err++;
                    App.TaskManager.Log.WriteLineAsync(
                        $"{"Контроль",-10} "                                +
                        $"{i_signal.Name + " - " + i_signal.Signal,-52} - " +
                        ( signal.Delta >= i_signal.Offset ? "норма" : "отказ" ) );
                }
            }
            return count_err;
        }
    }
}
