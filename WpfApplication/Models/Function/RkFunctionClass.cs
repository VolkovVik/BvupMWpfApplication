using System;
using System.Collections.Generic;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;
using WpfApplication.Models.Test;

namespace WpfApplication.Models.Function {
    internal class RkFunctionClass {
#region Команды интерфейса разовых команд

        // ReSharper disable UnusedMember.Local
        /// <summary>
        /// Параметры состояния разовых команд
        /// </summary>
        private const uint Vm7CDartRkom0Mode27Z = 1U << 16;

        private const uint Vm7CDartRkom1Mode27Z = 1U << 17;
        private const uint Vm7CDartRkom2Mode27Z = 1U << 18;
        private const uint Vm7CDartRkom3Mode27Z = 1U << 19;
        private const uint Vm7CDartRkom4Mode27Z = 1U << 20;
        private const uint Vm7CDartRkom5Mode27Z = 1U << 21;
        private const uint Vm7CDartRkom6Mode27Z = 1U << 22;
        private const uint Vm7CDartRkom7Mode27Z = 1U << 23;
        private const uint Vm7CDartRkim0Mode27Z = 1U << 24;
        private const uint Vm7CDartRkim1Mode27Z = 1U << 25;
        private const uint Vm7CDartRkim2Mode27Z = 1U << 26;
        private const uint Vm7CDartRkim3Mode27Z = 1U << 27;
        private const uint Vm7CDartRkim4Mode27Z = 1U << 28;
        private const uint Vm7CDartRkim5Mode27Z = 1U << 29;
        private const uint Vm7CDartRkim6Mode27Z = 1U << 30;
        private const uint Vm7CDartRkim7Mode27Z = 1U << 31;
        private const uint Vm7CDartRkom0Mode0Z  = 0U;
        private const uint Vm7CDartRkom1Mode0Z  = 0U;
        private const uint Vm7CDartRkom2Mode0Z  = 0U;
        private const uint Vm7CDartRkom3Mode0Z  = 0U;
        private const uint Vm7CDartRkom4Mode0Z  = 0U;
        private const uint Vm7CDartRkom5Mode0Z  = 0U;
        private const uint Vm7CDartRkom6Mode0Z  = 0U;
        private const uint Vm7CDartRkom7Mode0Z  = 0U;
        private const uint Vm7CDartRkim0Mode0Z  = 0U;
        private const uint Vm7CDartRkim1Mode0Z  = 0U;
        private const uint Vm7CDartRkim2Mode0Z  = 0U;
        private const uint Vm7CDartRkim3Mode0Z  = 0U;
        private const uint Vm7CDartRkim4Mode0Z  = 0U;
        private const uint Vm7CDartRkim5Mode0Z  = 0U;
        private const uint Vm7CDartRkim6Mode0Z  = 0U;

        private const uint Vm7CDartRkim7Mode0Z = 0U;
        // ReSharper restore UnusedMember.Local

        //
        // Задание конфигурации канала
        //
        // RKIM0...3 установлены в 27В/Обрыв
        // RKIM4...8 установлены в 0В/Обрыв
        // RKOM0...3 установлены в 27В/Обрыв
        // RKOM4...8 установлены в 0В/Обрыв
        private const uint SafeConfigParameter =
            Vm7CDartRkim0Mode27Z | Vm7CDartRkim1Mode27Z | Vm7CDartRkim2Mode27Z | Vm7CDartRkim3Mode27Z |
            Vm7CDartRkim4Mode0Z  | Vm7CDartRkim5Mode0Z  | Vm7CDartRkim6Mode0Z  | Vm7CDartRkim7Mode0Z  |
            Vm7CDartRkom0Mode27Z | Vm7CDartRkom1Mode27Z | Vm7CDartRkom2Mode27Z | Vm7CDartRkom3Mode27Z |
            Vm7CDartRkom4Mode0Z  | Vm7CDartRkom5Mode0Z  | Vm7CDartRkom6Mode0Z  | Vm7CDartRkom7Mode0Z;

        /// <summary>
        /// Команда запуска теста разовых команд
        /// </summary>
        private const string CmdRk = "rk";

        /// <summary>
        /// Команда задания настроек разовым командам
        /// </summary>
        private const string CmdRkCfgExt = "ext mode";

        /// <summary>
        /// Команда задания выходных разовых команд
        /// </summary>
        private const string CmdRkSet = "set";

        /// <summary>
        /// Команда получения входных разовых команд
        /// </summary>
        private const string CmdRkGet = "get byte";

        /// <summary>
        /// Команда получения входных разовых команд
        /// </summary>
        private const string CmdRkGetAll = "get all byte";

        /// <summary>
        /// Подпрограмма задания настроек разовым командам
        /// </summary>
        /// <param name="device"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static int Config( int device, uint code = SafeConfigParameter ) {
            // Создание словаря
            IList< string > cmd = new List< string > {
                Arinc429FunctionClass.CmdEsc,
                CmdRk,
                CmdRkCfgExt,
                $"0x{code:X8}"
            };
            return App.TaskManager.PortCom.Send( device, cmd, time: 200 );
        }

#endregion Команды интерфейса разовых команд

        /// <summary>
        /// Проверка допустимости данных сигнала
        /// </summary>
        /// <param name="signal"></param>
        private void Check( PlaceSignalDescription signal ) {
            const string protocol = Protocol.Rs232;
            // ReSharper disable once ConvertToConstant.Local
            var       max_device = PortRs232.MaxDevice;
            const int maxWord    = 3;
            const int maxPin     = 32;

            // Проверка инициализации СОМ-портов 
            if ( null == App.TaskManager.PortCom || !App.TaskManager.PortCom.IsInit ) {
                throw new Exception< DeviceNotInitializedExceptionArgs >( new DeviceNotInitializedExceptionArgs(),
                    "Ошибка! СОМ-порт не инициализирован" );
            }
            // Проверка допустимости количества устройств
            if ( max_device <= 0 )
                throw new ArgumentOutOfRangeException( $"Максимальное количество устройств = {max_device}" );
            // Проверка допустимости сигнала
            if ( signal == null ) {
                throw new ArgumentOutOfRangeException( $"Ошибка допустимости сигнала {nameof( signal )} = null" );
            }
            // Проверка допустимости протокола
            if ( signal.Protocol != protocol ) {
                throw new ArgumentOutOfRangeException(
                    $"Ошибка допустимости протокола {signal.Name} - {signal.Protocol}" );
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
        /// Слово выдачи РКВ
        /// </summary>
        private uint _rkoWord;

        /// <summary>
        /// Выдача РКВ
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        public void Set( PlaceSignalDescription signal, byte value ) {
            Check( signal );
            // Формирование шаблона РК
            // Сброс значение РК в 0 
            // Установка значения РК в заданное
            _rkoWord = ( _rkoWord & ~( uint ) ( 1 << ( signal.Pin ?? 0 ) ) ) |
                       ( uint ) ( ( value & 1 ) << ( signal.Pin ?? 0 ) );
            // Установка РКВ из ВМ-7
            Set( signal.Device ?? 0, _rkoWord );
        }

        /// <summary>
        /// Подпрограмма записи разовых команд
        /// </summary>
        /// <param name="index"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static int Set( int index, uint code ) {
            var cmd = new List< string > { Arinc429FunctionClass.CmdEsc, CmdRk, CmdRkSet, $"0x{code:X8}" };
            return App.TaskManager.PortCom.Send( index, cmd );
        }

        /// <summary>
        /// Прием РКП
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public byte Get( PlaceSignalDescription signal ) {
            Check( signal );
            // Прием разовой команды
            return ( byte ) Get( signal.Device ?? 0, signal.Word ?? 0, signal.Pin ?? 0 );
        }

        /// <summary>
        /// Подпрограмма чтения разовой команды
        /// </summary>
        /// <param name="index"></param>
        /// <param name="word"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        private static uint Get( int index, int word, int pin ) {
            var cmd = new List< string > {
                Arinc429FunctionClass.CmdEsc,
                CmdRk,
                CmdRkGet,
                word.ToString( "D" ),
                pin.ToString( "D" )
            };

            var data = new uint[ 1 ];
            // ReSharper disable once UnusedVariable
            var readed = App.TaskManager.PortCom.GetData( index, cmd, data, 0, 1, 1000 );
            return data[ 0 ];
        }

        /// <summary>
        /// Подпрограмма чтения разовых команд
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        private static uint[] Get( int index ) {
            var cmd = new List< string > { Arinc429FunctionClass.CmdEsc, CmdRk, CmdRkGetAll };

            var data = new uint[ 3 ];
            // ReSharper disable once UnusedVariable
            var readed = App.TaskManager.PortCom.GetData( index, cmd, data, 0, 3, 1000 );
            return data;
        }
    }
}