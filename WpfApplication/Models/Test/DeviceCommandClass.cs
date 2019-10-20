using System;
using WpfApplication.Models.Function;
using WpfApplication.Models.Test.As;
using WpfApplication.Models.Test.Rk;

namespace WpfApplication.Models.Test {
   
    internal static class DeviceCommandClass {
        /// <summary>
        /// Подпрограмма установки всех РК и АС в активное состояние
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static int Activ( int device ) {
            var count_err = 0;

            App.TaskManager.Log.WriteLineAsync(
                "Установка РКП и РКВ в активное состояние" + Environment.NewLine +
                "Установка АСП и АСВ в активное состояние" );
            // Настройка РК
            count_err += RkFunctionClass.Config( device );
            // Установка РКП в активное состояние
            new TestRkpClass().Set( device, 1 );
            // Установка РКВ в активное состояние
            new TestRkvClass().Set( device, 1 );
            // Установка аналоговых сигналов выдачи в активное состояние
            new TestDacClass().Set( device, 10 );
            // Установка аналоговых сигналов приема в активное состояние
            new TestAdcClass().Set( device, 10 );
            return count_err;
        }

        /// <summary>
        /// Подпрограмма установки всех РК и АС в активное состояние
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static int Passive( int device ) {
            var count_err = 0;

            App.TaskManager.Log.WriteLineAsync(
                "Установка РКП и РКВ в исходное состояние" + Environment.NewLine +
                "Установка АСП и АСВ в исходное состояние" );
            // Настройка РК
            count_err += RkFunctionClass.Config( device );
            // Установка РКВ в исходное состояние
            new TestRkvClass().Set( device, 0 );
            // Установка РКП в исходное состояние
            new TestRkpClass().Set( device, 0 );
            // Установка аналоговых сигналов выдачи в исходное состояние
            new TestDacClass().Set( device, 0 );
            // Установка аналоговых сигналов приема в исходное состояние
            new TestAdcClass().Set( device, 0 );
            return count_err;
        }
    }
}