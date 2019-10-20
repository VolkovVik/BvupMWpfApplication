using System;
using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;
using WpfApplication.Models.Test;
using WpfApplication.Models.Test.As;

namespace WpfApplication.Models.Function {
    internal class AsFunctionClass {
        /// <summary>
        /// Команда запуска теста аналоговых сигналов постоянного типа
        /// </summary>
        private const string CmdAs = "as";

        /// <summary>
        /// Команда задания напряжения в заданном канале
        /// </summary>
        private const string CmdAsSet = "set";

        /// <summary>
        /// Команда получения входных 1 аналогового сигнала
        /// </summary>
        private const string CmdAdcGet = "get byte";

        /// <summary>
        /// Команда получения входных 64 аналоговых сигналов
        /// </summary>
        private const string CmdAdcGetAll = "get all byte";

        /// <summary>
        /// Коэффициент перевода из напряжения в бинарных данных
        /// </summary>
        private const double Lsc = ( double ) 0xFFFF / 20;

        /// <summary>
        /// Проверка допустимости данных сигнала
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="maxChannel"></param>
        private void Check( PlaceSignalDescription signal, int maxChannel ) {
            const string protocol = Protocol.Rs232;
            // ReSharper disable once ConvertToConstant.Local
            var max_device = PortRs232.MaxDevice;

            // Проверка инициализации СОМ-портов 
            if ( null == App.TaskManager.PortCom || !App.TaskManager.PortCom.IsInit ) {
                throw new Exception< DeviceNotInitializedExceptionArgs >( new DeviceNotInitializedExceptionArgs(),
                    "Ошибка! СОМ-порт не инициализирован" );
            }
            // Проверка допустимости количества устройств
            if ( max_device <= 0 )
                throw new ArgumentOutOfRangeException( $"Максимальное количество устройств = {max_device}" );
            // Проверка допустимости количества каналов
            if ( maxChannel <= 0 )
                throw new ArgumentOutOfRangeException( $"Максимальное количество каналов = {maxChannel}" );
            // Проверка допустимости сигнала
            if ( signal == null ) {
                throw new ArgumentOutOfRangeException( $"Ошибка допустимости сигнала {nameof( signal )} = null" );
            }
            // Проверка допустимости протокола
            if ( signal.Protocol != protocol ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости протокола {signal.Name} - {signal.Protocol}" );
            }
            // Проверка допустимости номера устройства
            if ( signal.Device == null || signal.Device < 0 || signal.Device >= max_device ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости номера устройства {signal.Name} - {signal.Device}" );
            }
            // Проверка допустимости номера канала
            if ( signal.Channel == null || signal.Channel <= 0 || signal.Channel > maxChannel ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости номера канала {signal.Name} - {signal.Channel}" );
            }
        }

        /// <summary>
        /// Выдача аналогового сигнала
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        public void Set( PlaceSignalDescription signal, double value ) {
            Check( signal, AsDevice.DacChannel );
            // Выдача АСВ
            Set( signal.Device ?? 0, signal.Channel ?? 0, value );
        }

        /// <summary>
        /// Подпрограмма задания данных 1 каналу ЦАП
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="voltage"></param>
        /// <returns></returns>
        private static int Set( int index, int channel, double voltage ) {
            var cmd = new List< string > {
                Arinc429FunctionClass.CmdEsc,
                CmdAs,
                CmdAsSet,
                channel.ToString( "D" ),
                $"0x{( uint ) ( voltage * Lsc ) & 0xFFFF:X8}"
            };
            return App.TaskManager.PortCom.Send( index, cmd );
        }

        /// <summary>
        /// Прием аналогового сигнала
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public double Get( PlaceSignalDescription signal, int count = 3 ) {
            Check( signal, AsDevice.AdcChannel );

            // Цикл проходов
            var data = new List< double >();
            for ( var i = 0; i < count; i++ ) {
                // Чтение АСП
                var value = Get( signal.Device ?? 0, signal.Channel ?? 0 );
                // Преобразование в напряжение
                data.Add( ( short ) value >> 4 );
            }
            // Среднее значение
            return data.Average() * AsDevice.Lsb * ( signal.Coefficient ?? 1 ) - ( signal.Offset ?? 0 );
        }

        /// <summary>
        /// Подпрограмма получения данных заданного канала АЦП
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private static ushort Get( int index, int channel ) {
            var cmd = new List< string > { Arinc429FunctionClass.CmdEsc, CmdAs, CmdAdcGet, channel.ToString( "D" ) };

            var data = new ushort[ 1 ];
            // ReSharper disable once UnusedVariable
            var readed = App.TaskManager.PortCom.GetData( index, cmd, data, 0, 1, 100 );
            return data[ 0 ];
        }

        /// <summary>
        /// Подпрограмма получения данных 64 каналов АЦП
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Local
        private static ushort[] Get( int index ) {
            var cmd = new List< string > { Arinc429FunctionClass.CmdEsc, CmdAs, CmdAdcGetAll };

            var data = new ushort[ AsDevice.AdcChannel ];
            // ReSharper disable once UnusedVariable
            var readed = App.TaskManager.PortCom.GetData( index, cmd, data, 0, AsDevice.AdcChannel, 100 );
            return data;
        }
    }
}