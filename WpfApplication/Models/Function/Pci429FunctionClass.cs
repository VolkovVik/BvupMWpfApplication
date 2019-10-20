using System;
using System.Collections.Generic;
using System.Threading;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.arinc429;
using WpfApplication.Models.Test;
using WpfApplication.Models.Test.Arinc;

namespace WpfApplication.Models.Function {
    internal class Pci429FunctionClass {
        /// <summary>
        /// Проверка допустимости данных сигнала
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="maxChannel"></param>
        private void Check( PlaceSignalDescription signal, int maxChannel ) {
            const string protocol   = Protocol.Pci429;
            var          max_device = App.TaskManager.PortArinc.MaxDevice;

            // Проверка инициализации интерфейсной платы 
            if ( null == App.TaskManager.PortArinc || !App.TaskManager.PortArinc.IsInit ) {
                throw new Exception< DeviceNotInitializedExceptionArgs >( new DeviceNotInitializedExceptionArgs(),
                    "Ошибка! Плата PCI-429 не инициализирована" );
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
        /// Задание настроек каналу
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        public void Config( PlaceSignalDescription signal, TypeChannel type, FREQ frequency ) {
            Check( signal, type == TypeChannel.Rx ? Port429.MaxRxChannel : Port429.MaxTxChannel );
            // Задание частоты обмена
            App.TaskManager.PortArinc.Config( signal.Device ?? 0, ( byte ) type, ( byte ) ( signal.Channel ?? 0 ),
                frequency, true );
        }

        /// <summary>
        /// Задание настроек
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        private void Config( FREQ frequency ) {
            // Настройка плат PCI429 
            for ( var device = 0; device < App.TaskManager.PortArinc.MaxDevice; device++ ) {
                // Сброс плат PCI429 
                App.TaskManager.PortArinc.Reset( device );
                // Задание частоты работы каналам выдачи
                for ( byte channel = 0; channel < Port429.MaxTxChannel; channel++ ) {
                    App.TaskManager.PortArinc.Config( device, ( byte ) TypeChannel.Tx, channel, frequency, true );
                }
                // Задание частоты работы каналам приема
                for ( byte channel = 0; channel < Port429.MaxRxChannel; channel++ ) {
                    App.TaskManager.PortArinc.Config( device, ( byte ) TypeChannel.Rx, channel, frequency, true );
                }
            }
        }

        /// <summary>
        /// Выдача данных
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="template"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        public void SetData( PlaceSignalDescription signal, uint template, int address, int count, int time ) {
            if ( count == 0 ) return;
            // Проверка допустимости данных сигнала
            Check( signal, Port429.MaxTxChannel );
            // Формирования данных для выдачи
            // Сброс бита четности и адреса
            template &= 0x7FFFFF00;
            var data = new uint[ count ];
            for ( var i = 0; i < count; i++ ) {
                data[ i ] = GetWordParity( template | ( uint ) ( ( address + i ) & 0xFF ) );
            }
            // Выдача данных платой
            App.TaskManager.PortArinc.Write(  signal.Device ?? 0, ( ushort ) ( signal.Channel ?? 0 ), data, count );
            // Ожидание завершения выдачи данных
            Thread.Sleep( time );
            // TODO НАДО думать
            //// Ожидание завершения выдачи
            //App.MyWork.PortArinc.CheckTx((int)index, (byte)channel, time);
            //// Ожидание завершения выдачи
            //Thread.Sleep(50);
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public uint[] GetData( PlaceSignalDescription signal, int count, int time ) {
            // Проверка допустимости данных сигнала
            Check( signal, Port429.MaxRxChannel );

            // Чтение полученных данных из платы PCI429
            int readed;
            var data = App.TaskManager.PortArinc.Read( signal.Device ?? 0, ( ushort ) ( signal.Channel ?? 0 ),
                count, time, out readed );
            // Из интерфейсной платы принимаются только выданные слова
            // Алгоритм же проверяет весь фрейм
            // Формирование массива
            var value = new uint[ ArincDevice.RxData ];
            for ( var i = 0; i < value.Length; i++ ) {
                value[ i ] = ( uint ) ( 0xFF - i );
            }
            foreach ( var t in data ) {
                value[ t & 0xFF ] = t;
            }
            return value;
        }

        /// <summary>
        /// Получение счетчиков обновленных слов
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public uint GetCounters( PlaceSignalDescription signal ) {
            Check( signal, Port429.MaxRxChannel );
            // Чтение данных
            int readed;
            App.TaskManager.PortArinc.Read( signal.Device ?? 0, ( ushort ) ( signal.Channel ?? 0 ), out readed );
            return ( uint ) readed;
        }

        /// <summary>
        /// Получение счетчиков обновленных слов
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private uint[] GetCounters( int device ) {
            var counters = new uint[ Port429.MaxRxChannel ];
            // Цикл каналов
            for ( ushort channel = 0; channel < Port429.MaxRxChannel; channel++ ) {
                int readed;
                App.TaskManager.PortArinc.Read( device, channel, out readed );
                counters[ channel ] = ( uint ) readed;
            }
            return counters;
        }

        /// <summary>
        /// Получение счетчиков обновленных слов
        /// </summary>
        /// <param name="usedDevice"></param>
        /// <returns></returns>
        public uint[][] GetCounters( List< int > usedDevice = null ) {
            // Чтение счетчика обновленных слов
            var counters = new uint[  App.TaskManager.PortArinc.MaxDevice ][];
            // Цикл устройств
            for ( var device = 0; device < App.TaskManager.PortArinc.MaxDevice; device++ ) {
                counters[ device ] = usedDevice != null && usedDevice.Count > 0 && usedDevice.Contains( device )
                    ? GetCounters( device )
                    : new uint[ Port429.MaxRxChannel ];
            }
            return counters;
        }

        /// <summary>
        /// Рассчет бита четности слова Arinc429
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static uint GetWordParity( uint word ) {
            var count = 0;
            // Цикл подсчета количества единиц
            for ( var i = 0; i < 31; i++ ) count += ( int ) ( word >> i ) & 0x1;
            //for( var i = 0; i < 31; i++ )
            //    if ( ( ( word >> i ) & 0x1 ) > 0 )
            //        count++;
            return ( count & 0x1 ) == 0 ? word | 0x80000000U : word & 0x7FFFFFFFU;
        }
    }
}