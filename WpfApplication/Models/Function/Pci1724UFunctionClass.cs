using System;
using WpfApplication.Models.Main;
using WpfApplication.Models.Test;

namespace WpfApplication.Models.Function {
    internal class Pci1724UFunctionClass  {
        /// <summary>
        /// Проверка допустимости данных сигнала
        /// </summary>
        /// <param name="signal"></param>
        private void Check( PlaceSignalDescription signal ) {
            const string protocol    = Protocol.Pci1724U;
            var          max_device  = App.TaskManager.Port1724U.MaxDevice;
            var          max_channel = App.TaskManager.Port1724U.MaxChannel;

            // Проверка инициализации интерфейсной платы 
            if ( null == App.TaskManager.Port1724U || !App.TaskManager.Port1724U.IsInit ) {
                throw new Exception< DeviceNotInitializedExceptionArgs >( new DeviceNotInitializedExceptionArgs(),
                    "Ошибка! Плата PCI-1724U не инициализирована" );
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
        /// Выдача сигнала
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        public void Set( PlaceSignalDescription signal, double value ) {
            Check( signal );
            // Выдача аналогового сигнала с интерфейсной платы PCI-1724U
            App.TaskManager.Port1724U.Write( signal.Device ?? 0, signal.Channel ?? 0, value );
        }
    }
}