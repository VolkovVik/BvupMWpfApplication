using System;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.arinc429;
using WpfApplication.Models.PciCard.rs232;
using WpfApplication.Models.Test;
using WpfApplication.Models.Test.Arinc;

namespace WpfApplication.Models.Function {
    internal class Upc10FunctionClass {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        private void Check( PlaceSignalDescription signal ) {
            const string protocol = Protocol.Upc10;
            // ReSharper disable once ConvertToConstant.Local
            var       max_device = PortRs232.MaxDevice;
            const int maxWord    = 256;
            const int maxPin     = 32;

            // Проверка инициализации СОМ-портов 
            if ( null == App.TaskManager.PortCom || !App.TaskManager.PortCom.IsInit ) {
                throw new Exception< DeviceNotInitializedExceptionArgs >( new DeviceNotInitializedExceptionArgs(),
                    "Ошибка! СОМ-порт не инициализирован" );
            }
            // Проверка допустимости сигнала
            if ( signal == null ) {
                throw new ArgumentOutOfRangeException( $"Ошибка допустимости данных {nameof( signal )} = null" );
            }
            if ( signal.Protocol != protocol ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости протокола {signal.Name} - {signal.Protocol}" );
            }
            // Проверка допустимости индекса устройства
            if ( signal.Device == null || signal.Device < 0 || signal.Device >= max_device ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости номера устройства {signal.Name} - {signal.Device}" );
            }
            // Проверка допустимости номера слова
            if ( signal.Word == null || signal.Word < 0 || signal.Word >= maxWord ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости номера слова {signal.Name} - {signal.Word}" );
            }
            // Проверка допустимости пина слова
            if ( signal.Pin == null || signal.Pin < 0 || signal.Pin >= maxPin ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости номера пина {signal.Name} - {signal.Pin}" );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly Arinc429FunctionClass _func = new Arinc429FunctionClass();

        /// <summary>
        /// 
        /// </summary>
        private const int ChannelArincVm7ToUpc10 = 7;

        /// <summary>
        /// 
        /// </summary>
        private const int ChannelArincUpc10ToVm7 = 29;

        /// <summary>
        /// Задание настроек
        /// </summary>
        public void Config( int device ) {
            // Задание частоты обмена
            _func.Config( device, ChannelArincVm7ToUpc10, ( byte ) TypeChannel.Tx, FREQ.F100 );
            _func.Config( device, ChannelArincUpc10ToVm7, ( byte ) TypeChannel.Rx, FREQ.F100 );
        }

        /// <summary>
        /// Массив слов выдачи РКВ
        /// </summary>
        private readonly uint[] _rkoWord = new uint[ 256 ];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        public void SetRk( PlaceSignalDescription signal, byte value ) {
            // Проверка допустимости данных сигнала
            Check( signal );

            // Формирование шаблона РК
            // Сброс значение РК в 0 
            _rkoWord[ signal.Word ?? 0 ] &= ~( uint ) ( 1 << ( signal.Pin ?? 0 ) );
            // Установка значения РК в заданное
            _rkoWord[ signal.Word ?? 0 ] |= ( uint ) ( value << ( signal.Pin ?? 0 ) );
            // Формирование слова Arinc429
            // Идентификатор канала  = 00
            // Матрица состояния     = 00 ( Нормальная работа )
            _rkoWord[ signal.Word                                                      ?? 0 ] =
                ( _rkoWord[ signal.Word ?? 0 ] & 0x1FFFF000 ) | ( uint ) ( signal.Word ?? 0 );
            // Вычисление бита четности
            _rkoWord[ signal.Word ?? 0 ] = Pci429FunctionClass.GetWordParity( _rkoWord[ signal.Word ?? 0 ] );
            // Установка РКВ из УПС-10
            _func.SetData( signal.Device ?? 0, ChannelArincVm7ToUpc10, signal.Word ?? 0, _rkoWord[ signal.Word ?? 0 ],
                1 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public byte GetRk( PlaceSignalDescription signal ) {
            // Проверка допустимости данных сигнала
            Check( signal );
            var word = Arinc429FunctionClass.GetWord( signal.Device ?? 0, ChannelArincUpc10ToVm7, signal.Word ?? 0 );
            CheckWord( signal, word );
            return ( byte ) ( ( word >> ( signal.Pin ?? 0 ) ) & 0x1 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public uint GetWord( PlaceSignalDescription signal ) {
            // Проверка допустимости данных сигнала
            Check( signal );
            var word = Arinc429FunctionClass.GetWord( signal.Device ?? 0, ChannelArincUpc10ToVm7, signal.Word ?? 0 );
            CheckWord( signal, word );
            return word;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        private static void CheckWord( PlaceSignalDescription signal, uint word ) {
            // Проверка идентификатора канала
            var ik = ( word >> 8 ) & 0x3U;
            // ReSharper disable once InvertIf
            if ( ik != 0 ) {
                throw new Exception< Upc10ExceptionArgs >( new Upc10ExceptionArgs(),
                    $"Ошибка! Слово {word:X8} проверка идентификатора канала {ik:D2} - отказ" );
            }
            if ( signal?.Word == 251 || signal?.Word == 252 ) {
                CheckMcWord( word );
            } else {
                CheckMcCommand( word );
            }
        }

        /// <summary>
        /// Проверка матрицы состояния для слов команд и признаков
        /// </summary>
        /// <remarks>
        /// Проверка слов Угол ДОЗ            #372₈  250₁₀  0xFA₁₆ 
        /// Проверка слов Контрольный счетчик #375₈  253₁₀  0xFD₁₆ 
        /// </remarks>
        /// <param name="word"></param>
        /// <returns></returns>
        private static void CheckMcCommand( uint word ) {
            // Проверка матрицы состояния для слов команд и признаков
            var mc = ( word >> 29 ) & 0x3U;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch ( mc ) {
                case 0x0:
                    // Нормальная работа
                    break;
                case 0x1:
                    // Не применяется
                    // Нет вычисленных данных
                    throw new Exception< Upc10ExceptionArgs >( new Upc10ExceptionArgs(),
                        $"Ошибка! Слово {word:X8} проверка матрицы состояния = 01 ( Нет вычисленных данных )    - отказ" );

                case 0x2:
                    // Не применяется
                    // Функциональный тест
                    throw new Exception< Upc10ExceptionArgs >( new Upc10ExceptionArgs(),
                        $"Ошибка! Слово {word:X8} проверка матрицы состояния = 10 ( Функциональный тест )       - отказ" );
                case 0x3:
                    // Не применяется
                    // Не определено
                    throw new Exception< Upc10ExceptionArgs >( new Upc10ExceptionArgs(),
                        $"Ошибка! Слово {word:X8} проверка матрицы состояния = 11 ( Не определено )             - отказ" );
            }
        }

        /// <summary>
        /// Проверка матрицы состояния для слов двоичного кода
        /// </summary>
        /// <remarks>
        /// Проверка слов Состояние эхо-РКВ +27В/Обрыв    #373₈  251₁₀  0xFB₁₆ 
        /// Проверка слов Состояние эхо-РКВ +0В/Обрыв     #374₈  252₁₀  0xFC₁₆ 
        /// </remarks>
        /// <param name="word"></param>
        /// <returns></returns>
        private static void CheckMcWord( uint word ) {
            // Проверка матрицы состояния для слов двоичного кода
            var mc = ( word >> 29 ) & 0x3U;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch ( mc ) {
                case 0x0:
                    // Предупреждение об отказе
                    throw new Exception< Upc10ExceptionArgs >( new Upc10ExceptionArgs(),
                        $"Ошибка! Слово {word:X8} проверка матрицы состояния = 00 ( Предупреждение об отказе )  - отказ" );
                case 0x1:
                    // Не применяется
                    // Нет вычисленных данных
                    throw new Exception< Upc10ExceptionArgs >( new Upc10ExceptionArgs(),
                        $"Ошибка! Слово {word:X8} проверка матрицы состояния = 01 ( Нет вычисленных данных )    - отказ" );

                case 0x2:
                    // Не применяется
                    // Функциональный тест
                    throw new Exception< Upc10ExceptionArgs >( new Upc10ExceptionArgs(),
                        $"Ошибка! Слово {word:X8} проверка матрицы состояния = 10 ( Функциональный тест )       - отказ" );

                case 0x3:
                    // Нормальная работа
                    break;
            }
        }
    }
}