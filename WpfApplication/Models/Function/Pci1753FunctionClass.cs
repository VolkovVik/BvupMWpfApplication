using System;
using WpfApplication.Models.Main;
using WpfApplication.Models.Test;

namespace WpfApplication.Models.Function {
    internal class Pci1753FunctionClass {
        /// <summary>
        /// Проверка допустимости данных сигнала
        /// </summary>
        /// <param name="signal"></param>
        private void Check( PlaceSignalDescription signal ) {
            const string protocol    = Protocol.Pci1753;
            var          max_device  = App.TaskManager.Port1753.MaxDevice;
            var          max_channel = App.TaskManager.Port1753.MaxChannel;
            const int    maxPin      = 8;

            // Проверка инициализации интерфейсной платы 
            if ( null == App.TaskManager.Port1753 || !App.TaskManager.Port1753.IsInit ) {
                throw new Exception< DeviceNotInitializedExceptionArgs >( new DeviceNotInitializedExceptionArgs(),
                    "Ошибка! Плата PCI-1753 не инициализирована" );
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
            // Проверка допустимости номера пина
            if ( signal.Pin == null || signal.Pin < 0 || signal.Pin >= maxPin ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости номера пина {signal.Name} - {signal.Pin}" );
            }
        }

        /// <summary>
        /// Выдача разовой команды
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        public void Set( PlaceSignalDescription signal, byte value ) {
            Check( signal );
            // Задание РКП
            App.TaskManager.Port1753.WriteBit( signal.Device ?? 0, signal.Channel ?? 0, signal.Pin ?? 0,
                ( byte ) ( value & 0x1 ) );
        }

        /// <summary>
        /// Прием разовой команды
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public byte Get( PlaceSignalDescription signal ) {
            Check( signal );
            // Чтение РКВ
            return App.TaskManager.Port1753.ReadBit( signal.Device ?? 0, signal.Channel ?? 0, signal.Pin ?? 0 );
        }
    }
}