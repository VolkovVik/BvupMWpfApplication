using System;
using System.Collections.Generic;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.arinc429;
using WpfApplication.Models.PciCard.rs232;
using WpfApplication.Models.Test;
using WpfApplication.Models.Test.Arinc;

namespace WpfApplication.Models.Function {
    internal class Arinc429FunctionClass  {
        /// <summary>
        /// Команда ESC
        /// Encoding.UTF8.GetString( new byte[] {0x1B}, 0, 1 )
        /// </summary>
        public const string CmdEsc = "\u001b";

        /// <summary>
        /// Команда запуска теста каналов Arinc
        /// </summary>
        private const string CmdArinc = "arinc";

        /// <summary>
        /// Команда выдачи данных из устройства
        /// </summary>
        private const string CmdArincSetData = "set data";

        /// <summary>
        /// Команда задания скорости обмена каналам приема
        /// </summary>
        private const string CmdArincSetBaud = "set baud";

        /// <summary>
        /// Команда получения счетчиков принятых слов
        /// </summary>
        private const string CmdArincGetCounter = "get counter byte";

        /// <summary>
        /// Команда получения количества обновлненных слов в канале
        /// </summary>
        private const string CmdArincGetUpdate = "get update byte";

        /// <summary>
        /// Команда получения данных 
        /// </summary>
        private const string CmdArincGetData = "get data byte";

        /// <summary>
        /// Команда получения данных 
        /// </summary>
        private const string CmdArincGetWord = "get word byte";

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
        /// Задание настроек
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        public void Config( PlaceSignalDescription signal, TypeChannel type, FREQ frequency ) {
            Check( signal, type == TypeChannel.Rx
                ? ArincDevice.RxChannel
                : ArincDevice.TxChannel );
            // Задание частоты обмена
            Config( signal.Device ?? 0, signal.Channel ?? 0, ( byte ) type, frequency );
        }

        /// <summary>
        /// Задание настроек
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Local
        private void Config( FREQ frequency ) {
            // Настройка устройства
            for ( var device = 0; device < PortRs232.MaxDevice; device++ ) {
                // Задание частоты работы каналам выдачи
                for ( byte channel = 0; channel < ArincDevice.TxChannel; channel++ ) {
                    Config( device, channel, ( byte ) TypeChannel.Tx, frequency );
                }
                // Задание частоты работы каналам приема
                for ( byte channel = 0; channel < ArincDevice.RxChannel; channel++ ) {
                    Config( device, channel, ( byte ) TypeChannel.Rx, frequency );
                }
            }
        }

        /// <summary>
        /// Задание настроек
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public int Config( int index, int channel, byte type, FREQ frequency ) {
            var cmd = new List< string > {
                CmdEsc,
                CmdArinc,
                CmdArincSetBaud,
                type.ToString( "D" ),
                channel.ToString( "D" ),
                frequency == FREQ.F12 ? "12" : "100"
            };
            return App.TaskManager.PortCom.Send( index, cmd );
        }

        /// <summary>
        /// Выдача данных
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="template"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        public void SetData( PlaceSignalDescription signal, uint template, int address, int count ) {
            Check( signal, ArincDevice.TxChannel );
            // Выдача данных устройством
            SetData( signal.Device ?? 0, signal.Channel ?? 0, address, template, count );
        }

        /// <summary>
        /// Выдача данных
        /// </summary>
        /// <param name="index"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <param name="channel"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public int SetData( int index, int channel, int address, uint template, int count ) {
            var cmd = new List< string > {
                CmdEsc,
                CmdArinc,
                CmdArincSetData,
                channel.ToString( "D" ),
                $"0x{template:X8}",
                address.ToString( "D" ),
                count.ToString( "D" )
            };
            return App.TaskManager.PortCom.Send( index, cmd, time: 500 );
        }

        /// <summary>
        /// Сброс обновленных данных
        /// </summary>
        /// <param name="signal"></param>
        public void ResetData( PlaceSignalDescription signal ) {
            Check( signal, ArincDevice.RxChannel );
            // Сброс обновлненных данных
            GetCounters( signal.Device ?? 0, signal.Channel ?? 0 );
        }

        /// <summary>
        /// Сброс обновленных данных
        /// </summary>
        /// <param name="usedDevice"></param>
        public void ResetData( List< int > usedDevice = null ) {
            // Сброс обновлненных данных
            for ( var device = 0; device < PortRs232.MaxDevice; device++ ) {
                if ( usedDevice == null || usedDevice.Count == 0 || usedDevice.Contains( device ) ) {
                    GetCounters( device );
                }
            }
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public uint[] GetData( PlaceSignalDescription signal ) {
            Check( signal, ArincDevice.RxChannel );
            // Чтение принятых данных
            return GetData( signal.Device ?? 0, signal.Channel ?? 0 );
        }

        /// <summary>
        /// Подпрограмма получения принятых данных
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private static uint[] GetData( int index, int channel ) {
            var cmd = new List< string > {
                CmdEsc,
                CmdArinc,
                CmdArincGetData,
                channel.ToString( "D" )
            };
            var data = new uint[ ArincDevice.RxData ];
            // ReSharper disable once UnusedVariable
            var readed = App.TaskManager.PortCom.GetData( index, cmd, data, 0, ArincDevice.RxData, 6000 );
            return data;
        }

        /// <summary>
        /// Подпрограмма получения принятых данных
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        private static uint[] GetData( int index, int channel, int address, int count ) {
            var cmd = new List< string > {
                CmdEsc,
                CmdArinc,
                CmdArincGetWord,
                channel.ToString( "D" ),
                address.ToString( "D" ),
                count.ToString( "D" )
            };
            var data = new uint[ count ];
            // ReSharper disable once UnusedVariable
            var readed = App.TaskManager.PortCom.GetData( index, cmd, data, 0, count, 6000 );
            return data;
        }

        /// <summary>
        /// Подпрограмма получения принятых данных
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static uint GetWord( int index, int channel, int address ) => GetData( index, channel, address, 1 )[ 0 ];

        /// <summary>
        /// Подпрограмма получения принятых данных
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <param name="address"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        private uint GetPin( int index, int channel, int address, int pin ) =>
            ( byte ) ( ( GetWord( index, channel, address ) >> pin ) & 0x1 );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedDevice"></param>
        /// <returns></returns>
        public uint[][] GetCounters( List< int > usedDevice = null ) {
            // Чтение счетчика обновленных слов
            var counters = new uint[ PortRs232.MaxDevice ][];
            // Цикл устройств
            for ( var device = 0; device < PortRs232.MaxDevice; device++ ) {
                counters[ device ] = usedDevice != null && usedDevice.Count > 0 && usedDevice.Contains( device )
                    ? GetCounters( device )
                    : new uint[ ArincDevice.RxChannel ];
            }
            return counters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public uint GetCounters( PlaceSignalDescription signal ) {
            Check( signal, ArincDevice.RxChannel );
            // Чтение принятых данных
            return GetCounters( signal.Device ?? 0, signal.Channel ?? 0 );
        }

        /// <summary>
        /// Подпрограмма проверки выданных данных
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static uint[] GetCounters( int index ) {
            var cmd = new List< string > { CmdEsc, CmdArinc, CmdArincGetCounter };

            var data = new uint[ ArincDevice.RxChannel ];
            // ReSharper disable once UnusedVariable
            var readed = App.TaskManager.PortCom.GetData( index, cmd, data, 0, ArincDevice.RxChannel, 2000 );
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static uint GetCounters( int index, int channel ) {
            var cmd = new List< string > {
                CmdEsc,
                CmdArinc,
                CmdArincGetUpdate,
                channel.ToString( "D" )
            };
            var data = new uint[ 1 ];
            // ReSharper disable once UnusedVariable
            var readed = App.TaskManager.PortCom.GetData( index, cmd, data, 0, 1, 2000 );
            return data[ 0 ];
        }
    }
}