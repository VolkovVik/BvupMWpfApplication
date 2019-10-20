using System.Collections.Generic;
using WpfApplication.Models.Function;

namespace WpfApplication.Models.Test.Rk
{
    internal abstract class TestRkClass : AbstractTestClass<SignalDescription, byte, byte>
    {

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
        protected override int InitTest() {
            // Задание настроек каналам Arinc429
            Upc10Func.Config( Index );
            // Задание настроек разовым командам
            return RkFunctionClass.Config( Index );
        }

        /// <inheritdoc />
        /// <summary>
        /// Сброс всех сигналов по окончанию теста
        /// </summary>
        protected override void ResetTest() => Set();

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="config"></param>
        protected override void Set( SignalDescription signal, byte config ) {
            // Цикл каналов выдачи
            foreach( var i_signal in signal.Output ) {
                App.TaskManager.Log.WriteLineAsync(
                    $"{"Запись",-10} "                                                                 +
                    $"{i_signal?.Name,-10} в  {i_signal?.Protocol,10}#{i_signal?.Device,2:D2} "        +
                    ( i_signal?.Channel == null ? $"{"",-10} " : $"channel#{i_signal.Channel,2:D2} " ) +
                    ( i_signal?.Word    == null ? $"{"",-8} " : $"word#{i_signal.Word,3:D3} " )        +
                    ( i_signal?.Pin     == null ? $"{"",-6} " : $"pin#{i_signal.Pin,2:D2} " )          +
                    $" => {config & 1,2:X2}" );
                Set( i_signal, config );
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
        protected override List<byte> Get( SignalDescription signal, byte config, bool isTestedChannel = true ) {
            var list = new List< byte >();
            foreach( var i_signal in signal.Input ) {
                // Получение сигнала
                var value = Get( i_signal );
                list.Add( value );
                App.TaskManager.Log.WriteLineAsync(
                    $"{"Чтение",-10} "                                                                 +
                    $"{i_signal?.Name,-10} из {i_signal?.Protocol,10}#{i_signal?.Device,2:D2} "        +
                    ( i_signal?.Channel == null ? $"{"",-10} " : $"channel#{i_signal.Channel,2:D2} " ) +
                    ( i_signal?.Word    == null ? $"{"",-8} " : $"word#{i_signal.Word,3:D3} " )        +
                    ( i_signal?.Pin     == null ? $"{"",-6} " : $"pin#{i_signal.Pin,2:D2} " )          +
                    $" <= {value & 1,2:X2}" );
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
        protected override int Check( SignalDescription signal, List<byte> result, byte config, bool isTestedChannel = true ) {
            var count_err = 0;
            // Проверка принятых сигналов
            for( var i = 0; i < signal.Input.Count; i++ ) {
                var i_signal = signal.Input[ i ];
                // Вывод пользователю
                App.TaskManager.Log.WriteLineAsync(
                    $"{"Контроль",-10} "                                  +
                    $"{i_signal?.Name + " - " + i_signal?.Signal,-52} - " +
                    ( config == result[i] ? "норма" : "отказ" ) );
                count_err += config == result[i] ? 0 : 1;
            }
            return count_err;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected void Set( byte value = 0 ) {
            foreach ( var signal in SignalList ) Set( signal, value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        protected abstract void Set( PlaceSignalDescription signal, byte value );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        protected abstract byte Get( PlaceSignalDescription signal );
    }
}
