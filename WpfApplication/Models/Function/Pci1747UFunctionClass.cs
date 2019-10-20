using System;
using System.Collections.Generic;
using System.Linq;
using WpfApplication.Models.Main;
using WpfApplication.Models.Test;
using WpfApplication.Models.Test.As;

namespace WpfApplication.Models.Function {
    internal class Pci1747UFunctionClass {
        /// <summary>
        /// Проверка допустимости данных сигнала
        /// </summary>
        /// <param name="signal"></param>
        private void Check( PlaceSignalDescription signal ) {
            const string protocol    = Protocol.Pci1747U;
            var          max_device  = App.TaskManager.Port1747U.MaxDevice;
            var          max_channel = App.TaskManager.Port1747U.MaxChannel;

            // Проверка инициализации интерфейсной платы 
            if ( null == App.TaskManager.Port1747U || !App.TaskManager.Port1747U.IsInit ) {
                throw new Exception< DeviceNotInitializedExceptionArgs >( new DeviceNotInitializedExceptionArgs(),
                    "Ошибка! Плата PCI-1747U не инициализирована" );
            }
            // Проверка допустимости количества устройств
            if ( max_device <= 0 )
                throw new ArgumentOutOfRangeException( $"Максимальное количество устройств = {max_device}" );
            // Проверка допустимости количества каналов
            if ( max_channel <= 0 )
                throw new ArgumentOutOfRangeException( $"Максимальное количество каналов = {max_channel}" );
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
            if ( signal.Channel == null || signal.Channel <= 0 || signal.Channel > max_channel ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости номера канала {signal.Name} - {signal.Channel}" );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public double Get( PlaceSignalDescription signal, int count = 3 ) {
            Check( signal );

            var data = new List< double >();
            // Получение аналогового сигнала с интерфейсной платы PCI-1747U
            for ( var i = 0; i < count; i++ ) {
                data.Add( App.TaskManager.Port1747U.Read( signal.Device ?? 0, signal.Channel ?? 0 ) );
            }
            // Среднее значение
            return data.Average() * AsDevice.Lsb * ( signal.Coefficient ?? 1 ) - ( signal.Offset ?? 0 );
        }
    }
}