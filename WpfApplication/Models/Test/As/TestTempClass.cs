using System;
using System.Windows;
using WpfApplication.Models.Function;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.As {
    /// <summary>
    /// Класс контроля датчика температуры
    /// </summary>
    internal class TestTempClass {
        /// <summary>
        /// Индекс проверяемого устройства
        /// </summary>
        private int _index;

        /// <summary>
        /// Максимально допустимое отклонение температуры
        /// </summary>
        private const double OffsetTemperature = 1.5;

        /// <summary>
        /// Объект работы с устройством
        /// </summary>
        private readonly AsFunctionClass _rs232Func = new AsFunctionClass();

        /// <summary>
        /// 
        /// </summary>
        private readonly PlaceSignalDescription _signalTemp =
            new PlaceSignalDescription( 0, "Сигн.темп. датч. UT", "", "IN_ANALOG_3_1 Код управления 0000 АЦП №3" );

        /// <summary>
        /// Подпрограмма запуска контроля датчика температуры
        /// </summary>
        /// <returns></returns>
        public int Start( int indexTask ) {
            _index = 0;
            // Проверка заданного индекса
            if ( _index < 0 || _index >= PortRs232.MaxDevice ) {
                throw new Exception< ParameterNotFoundExceptionArgs >( new ParameterNotFoundExceptionArgs(),
                    $"Ошибка допустимости данных индекса {_index}" );
            }
            // Проверка используемых идентификаторов
            if ( !App.TaskManager.IdEnabledsDictionary[ _index ].Rs232 ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    "Ошибка! Инициализация идентификаторов RS232 не пройдена" );
            }
            if ( !App.TaskManager.IdEnabledsDictionary[ _index ].Adc ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    $"Ошибка! Инициализация идентификатора АЦП {_index:D} канала не пройдена" );
            }
            // Контроль интерфейса
            return Test();
        }

        /// <summary>
        /// Подпрограмма контроля датчика температуры
        /// </summary>
        /// <returns></returns>
        private int Test() {
            var count_err = 0;

            // Установка РКП и РКВ в исходное состояние
            // Установка АСП и АСВ в исходное состояние
            count_err += DeviceCommandClass.Passive( _index );
            CommonClass.SetText( "Контроль датчика температуры", 1, false );
            // Тест 
            // Получение температуры в устройстве
            // 0.1В - 1°C, точность датчика ±1°C
            var temperature1 = _rs232Func.Get( _signalTemp, 11 );
            //var temperature1 = AsCommon.device_get_adc( _index, "Сигн.темп.датч.UT", 10, pass: 11 );
            // Установка РКП и РКВ в активное состояние
            // Установка АСП и АСВ в активное состояние
            count_err += DeviceCommandClass.Activ( _index );
            // Тест 
            var temperature2 = _rs232Func.Get( _signalTemp, 11 );
            //var temperature2 = AsCommon.device_get_adc( _index, "Сигн.темп.датч.UT", 10, pass: 11 );
            // Установка РКП и РКВ в исходное состояние
            // Установка АСП и АСВ в исходное состояние
            count_err += DeviceCommandClass.Passive( _index );
            // Контроль температуры до и после
            var offset = Math.Abs( temperature1 - temperature2 );
            App.TaskManager.Log.WriteLineAsync(
                $"Датчик температура 1   = {temperature1,9:00.00000} °C"      + Environment.NewLine +
                $"Датчик температура 2   = {temperature2,9:00.00000} °C"      + Environment.NewLine +
                $"Допуск температуры     = {OffsetTemperature,9:00.00000} °C" + Environment.NewLine +
                $"Отклонение температуры = {offset,9:00.00000} °C" );
            count_err += offset > OffsetTemperature ? 1 : 0;
            // Выдача сообщения пользователю
            var result = MessageBox.Show(
                messageBoxText:
                App.TaskManager.NameChanneList[ _index ]                     +
                $"Температура устройства {temperature1,9:00.00000} °C"       +
                Environment.NewLine                                          +
                $"Температура устройства {temperature2,9:00.00000} °C"       +
                Environment.NewLine                                          +
                "При неправильно полученной температуре нажмите кнопку НЕТ." +
                Environment.NewLine                                          +
                "При правильно полученной температуре нажмите кнопку ДА."    +
                Environment.NewLine                                          +
                "При нажатии кнопки тест будет завершен.",
                caption: @"Контроль датчика температуры",
                button: MessageBoxButton.YesNo,
                icon: MessageBoxImage.Question,
                defaultResult: MessageBoxResult.Yes,
                options: MessageBoxOptions.DefaultDesktopOnly );
            count_err += result == MessageBoxResult.Yes ? 0 : 1;
            CommonClass.SetResText( "Контроль датчика температуры", 1, 60, count_err == 0 );
            App.TaskManager.Log.WriteLineAsync(  CommonClass.Separator );
            return count_err;
        }
    }
}