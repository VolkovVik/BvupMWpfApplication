using System;
using WpfApplication.Models.Main.XML;
using WpfApplication.Models.PciCard.advantech.pci1716;
using WpfApplication.Models.PciCard.advantech.pci1721;
using WpfApplication.Models.PciCard.advantech.pci1724u;
using WpfApplication.Models.PciCard.advantech.pci1747u;
using WpfApplication.Models.PciCard.advantech.pci1753;
using WpfApplication.Models.PciCard.arinc429;
using WpfApplication.Models.PciCard.rs232;

namespace WpfApplication.Models.Test.InitPciCard {
    /// <summary>
    /// 
    /// </summary>
    internal static class InitPciCardClass {
        /// <summary>
        /// Подпрограмма инициализации интерфейсных плат
        /// </summary>
        public static int Init( int param = 0 ) {
            var count_err = 0;

            var str = "Инициализация интерфейсных плат...";
            App.MyWindows.TextLine += str;
            App.TaskManager.Log.WriteLineAsync( str );
            // Проверка существования файла настроек
            if ( !new XmlClass().Exist ) {
                App.MyWindows.TextLine += "Файл настроек не найден";
                count_err++;
            } else {
                App.MyWindows.ValueProgress++;
                // Инициализация СОМ портов
                count_err += open_com_port();
                // Инициализация интерфейсных плат PCI429
                count_err += open_pci429();
                // TODO 2017-02-16 Интерфейсная плата PCI-1716 отсутствует в данном ПО
                //count_err += open_pci1716();
                count_err += open_pci1721();
                count_err += open_pci1724u();
                count_err += open_pci1747u();
                count_err += open_pci1753();
            }
            // Проверка  результатов инициализации объектов
            // В случае возникновения ошибок выводится сообщение
            // загрузка основной формы не производится,
            // предлагается проверить настройки
            if ( count_err == 0 ) {
                str = "Инициализация интерфейсных плат завершена успешно.";
            } else {
                // Объекты не созданы
                str = "Инициализация интерфейсных плат завершена c ошибками." + Environment.NewLine +
                      "Проверьте правильность задания настроек!!!";
            }
            App.TaskManager.Log.WriteLineAsync( str );
            App.MyWindows.TextLine      += str;
            App.MyWindows.ValueProgress =  App.MyWindows.MinProgress;
            return count_err;
        }

        /// <summary>
        /// Подпрограмма создания объектов СОМ портов
        /// </summary>
        /// <returns></returns>
        private static int open_com_port() {
            App.TaskManager.PortCom = new PortRs232( "RS-232" );
            App.TaskManager.PortCom.Open();
            return App.TaskManager.PortCom.IsInit ? 0 : 1;
        }

        /// <summary>
        /// Подпрограмма создания объекта интерфейсная плата PCI-1716
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Local
        private static int open_pci1716() {
            App.TaskManager.Port1716 = new Port1716( "PCI-1716", 1 );
            App.TaskManager.Port1716.Open();
            return App.TaskManager.Port1716.IsInit ? 0 : 1;
        }

        /// <summary>
        /// Подпрограмма создания объекта интерфейсная плата PCI-1721
        /// </summary>
        /// <returns></returns>
        private static int open_pci1721() {
            App.TaskManager.Port1721 = new Port1721( "PCI-1721", 1 );
            App.TaskManager.Port1721.Open();
            return App.TaskManager.Port1721.IsInit ? 0 : 1;
        }

        /// <summary>
        /// Подпрограмма создания объекта интерфейсная плата PCI-1724U
        /// </summary>
        /// <returns></returns>
        private static int open_pci1724u() {
            App.TaskManager.Port1724U = new Port1724U( "PCI-1724U", 1 );
            App.TaskManager.Port1724U.Open();
            return App.TaskManager.Port1724U.IsInit ? 0 : 1;
        }

        /// <summary>
        /// Подпрограмма создания объекта интерфейсная плата PCI-1747U
        /// </summary>
        /// <returns></returns>
        private static int open_pci1747u() {
            App.TaskManager.Port1747U = new Port1747U( "PCI-1747U", 1 );
            App.TaskManager.Port1747U.Open();
            return App.TaskManager.Port1747U.IsInit ? 0 : 1;
        }

        /// <summary>
        /// Подпрограмма создания объекта интерфейсная плата PCI-1753
        /// </summary>
        /// <returns></returns>
        private static int open_pci1753() {
            App.TaskManager.Port1753 = new Port1753( "PCI-1753", 2 );
            App.TaskManager.Port1753.Open();
            return App.TaskManager.Port1753.IsInit ? 0 : 1;
        }

        /// <summary>
        /// Подпрограмма создания объекта интерфейсная плата PCI-429-4-3
        /// </summary>
        /// <returns></returns>
        private static int open_pci429() {
            App.TaskManager.PortArinc = new Port429( "PCI-429", 2 );
            App.TaskManager.PortArinc.Open();
            return App.TaskManager.PortArinc.IsInit ? 0 : 1;
        }
    }
}