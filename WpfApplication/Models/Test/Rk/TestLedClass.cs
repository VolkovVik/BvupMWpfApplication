using System;
using System.Threading;
using System.Windows;
using WpfApplication.Models.Function;
using WpfApplication.Models.Main;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.Rk {
    /// <summary>
    /// Класс содержащий тесты светодиода
    /// </summary>
    internal class TestLedClass {
        /// <summary>
        /// Индекс устройства
        /// </summary>
        private int _indexDevice;

        /// <summary>
        /// Период работы светодиода
        /// </summary>
        private const int Period = 500;

        /// <summary>
        /// Объект работы с устройством
        /// </summary>
        private readonly RkFunctionClass _rs232Func = new RkFunctionClass();

        /// <summary>
        /// Тест светодиода
        /// </summary>
        /// <returns></returns>
        public int Start( int index ) {
            // Индекс СОМ-порта
            _indexDevice = index == ( int ) ConfigTestsClass.IdTest.Led1 ? 0 : 1;
            // Проверка заданного индекса
            if ( _indexDevice < 0 || _indexDevice >= PortRs232.MaxDevice ) {
                throw new Exception< ParameterNotFoundExceptionArgs >( new ParameterNotFoundExceptionArgs(),
                    $"Ошибка допустимости данных индекса {_indexDevice}" );
            }
            // Проверка используемых идентификаторов
            if ( !App.TaskManager.IdEnabledsDictionary[ _indexDevice ].Rs232 ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    $"Ошибка! Инициализация идентификаторов {_indexDevice:D} порта RS232 не пройдена" );
            }
            if ( !App.TaskManager.IdEnabledsDictionary[ _indexDevice ].Rk ) {
                throw new Exception< IdEnabledsNotValidExceptionArgs >( new IdEnabledsNotValidExceptionArgs(),
                    $"Ошибка! Инициализация идентификаторов {_indexDevice:D} канала RK не пройдена" );
            }
            // Контроль интерфейса
            return Test();
        }

        /// <summary>
        /// Подпрограмма запуска теста 
        /// </summary>
        /// <returns></returns>
        private int Test() {
            // Количество ошибок в тесте
            var count_err = 0;
            // Настройка режима работы ProgressBar
            App.MyWindows.IndeterminateProgress = true;

            // Создание сигнала
            var signal = new PlaceSignalDescription( _indexDevice, "РКВ LED", "РК СВЕТОДИОДА", "ВМ-7 word 0 pin 31" );
            // Выключение светодиода
            _rs232Func.Set( signal, 0 );
            // Создание объекта Timer c периодом 500 миллисекунд
            byte count_led = 1;
            // Включение/выключение светодиода
            var timer = new Timer( o  => { _rs232Func.Set( signal, count_led++ ); }, null, 0, Period );
            // Выдача сообщения пользователю
            var result = MessageBox.Show(
                messageBoxText:
                App.TaskManager.NameChanneList[ _indexDevice ]    +
                "Визуально проконтролируйте моргание светодиода." +
                Environment.NewLine                               +
                "При отсутствии моргания нажмите кнопку НЕТ."     +
                Environment.NewLine                               +
                "При наличии моргания нажмите кнопку ДА."         +
                Environment.NewLine                               +
                "При нажатии кнопки тест будет завершен.",
                caption: @"Контроль работоспособности светодиода",
                button: MessageBoxButton.YesNo,
                icon: MessageBoxImage.Question,
                defaultResult: MessageBoxResult.Yes,
                options: MessageBoxOptions.DefaultDesktopOnly );
            count_err += result == MessageBoxResult.Yes ? 0 : 1;
            // Сброс таймера
            timer.Dispose();
            // Выключение светодиода
            _rs232Func.Set( signal, 0 );
            // Настройка режима работы ProgressBar
            App.MyWindows.IndeterminateProgress = false;
            return count_err;
        }
    }
}