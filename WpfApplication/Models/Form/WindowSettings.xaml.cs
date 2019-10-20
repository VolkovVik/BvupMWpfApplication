using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Automation.BDaq;
using WpfApplication.Models.Main;
using WpfApplication.Models.Main.XML;
using ComboBox = System.Windows.Controls.ComboBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WpfApplication.Models.Form {
    /// <summary>
    /// Interaction logic for WindowSettings.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class WindowSettings : Window {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public WindowSettings() {
            InitializeComponent();
            try {
                Init();
            }
            catch ( Win32Exception exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка получения списка доступных СОМ-портов" );
            }
            catch ( DllNotFoundException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка обращения к платам Advantech" );
            }
        }

        /// <summary>
        /// Подпрограмма инициализации формы
        /// </summary>
        private void Init() {
            // Настройка изображений
            //var path = App.MyGmainTest.ConfigProgram.ImageUri;
            Icon          = new IconClass().get_icon();
            Image1.Source = new IconClass().get_image();
            // Get a list of serial port names
            var list = SerialPort.GetPortNames();
            // Формирования коллекции ComboBox
            ComboBoxCom1.Items.Clear();
            foreach ( var str in list ) {
                ComboBoxCom1.Items.Add( str );
            }
            // Получение списка плат работающих с разовыми командами
            var base_dio_ctrl = new DioCtrlBase();
            // Список плат PCI-1716, PCI-1721, PCI-1753
            var device_list = base_dio_ctrl.SupportedDevices;
            foreach ( var str in device_list ) {
                //if ( string.Equals( "PCI-1716", str.Description.Substring( 0, "PCI-1716".Length ) ) ) {
                //    comboBoxPci1716.Items.Add( str );
                //}
                if ( string.Equals( "PCI-1721", str.Description.Substring( 0, "PCI-1721".Length ) ) ) {
                    ComboBoxPci1721.Items.Add( str );
                }
                // ReSharper disable once InvertIf
                if ( string.Equals( "PCI-1753", str.Description.Substring( 0, "PCI-1753".Length ) ) ) {
                    ComboBoxPci1753N1.Items.Add( str );
                    ComboBoxPci1753N2.Items.Add( str );
                }
            }
            // Получение списка плат раб отающихс аналоговыми сигналами
            var instant_ao_contrl = new InstantAoCtrl();
            // Список плат PCI-1716, PCI-1721, PCI-1724
            device_list = instant_ao_contrl.SupportedDevices;
            foreach ( var str in device_list ) {
                if ( string.Equals( "PCI-1724", str.Description.Substring( 0, "PCI-1724".Length ) ) ) {
                    ComboBoxPci1724U.Items.Add( str );
                }
            }
            var instant_ai_contrl = new InstantAiCtrl();
            // Получение списка плат работающих с разовыми командами
            device_list = instant_ai_contrl.SupportedDevices;
            foreach ( var str in device_list ) {
                if ( string.Equals( "PCI-1747", str.Description.Substring( 0, "PCI-1747".Length ) ) ) {
                    ComboBoxPci1747U.Items.Add( str );
                }
            }
            // Получение списка плат Arinc429
            var devices = new List< ushort >();
            App.TaskManager.PortArinc.GetDevice( devices );
            TextBoxPci429N1.Items.Clear();
            TextBoxPci429N2.Items.Clear();
            foreach ( var item in devices ) {
                TextBoxPci429N1.Items.Add( item.ToString() );
                TextBoxPci429N2.Items.Add( item.ToString() );
            }
            // Установка настроек
            var xml = new XmlClass();
            if ( xml.Exist ) {
                fill_form_file();
            } else {
                fill_form_default();
            }
        }

        /// <summary>
        /// Подпрограмма заполнения формы по умолчанию
        /// </summary>
        private void fill_form_default() {
            // Файл настроек не найден
            ComboBoxCom1.Text = string.Empty;
            ComboBoxPci1721.Text =
                ComboBoxPci1724U.Text =
                    ComboBoxPci1747U.Text =
                        ComboBoxPci1753N1.Text =
                            ComboBoxPci1753N2.Text = string.Empty;
            TextBoxPci429N1.Text =
                TextBoxPci429N2.Text = @"00000";
            // Настройки НЕХ-файлов по умолчанию
            TextType.Text = @"None";
            TextCs.Text   = @"0x00000000";
            var directory = AppDomain.CurrentDomain.BaseDirectory + App.TaskManager.ConfigProgram.PathHexFiles;
            TextPath.Text = directory;
        }

        /// <summary>
        /// Подпрограмма заполнения формы из файла
        /// </summary>
        private void fill_form_file() {
            var xml = new XmlClass();
            //
            // Настройки PCI-карт
            //
            // Считывание имени порта 1 RS232 из .ini файла  
            set_items( ComboBoxCom1, xml.Read( XmlClass.NameRs232, XmlClass.ElementPort + 0 ) );
            // Считывание номера платы PCI1721 из .ini файла
            set_items( ComboBoxPci1721, xml.Read( XmlClass.NamePci1721, XmlClass.ElementBid + 0 ) );
            // Считывание номера платы PCI1724U из .ini файла
            set_items( ComboBoxPci1724U, xml.Read( XmlClass.NamePci1724, XmlClass.ElementBid + 0 ) );
            // Считывание номера платы PCI1747U из .ini файла
            set_items( ComboBoxPci1747U, xml.Read( XmlClass.NamePci1747, XmlClass.ElementBid + 0 ) );
            // Считывание номера платы PCI1753 из .ini файла
            set_items( ComboBoxPci1753N1, xml.Read( XmlClass.NamePci1753, XmlClass.ElementBid + 0 ) );
            // Считывание номера платы PCI1753 из .ini файла
            set_items( ComboBoxPci1753N2, xml.Read( XmlClass.NamePci1753, XmlClass.ElementBid + 1 ) );
            // Считывание номера платы PCI429 из .ini файла 
            TextBoxPci429N1.Text = xml.Read( XmlClass.NamePci429, XmlClass.ElementSn + 0 );
            // Считывание номера платы PCI429 из .ini файла 
            TextBoxPci429N2.Text = xml.Read( XmlClass.NamePci429, XmlClass.ElementSn + 1 );
            //
            // Чтение настроек НЕХ-файла
            //
            // Считывание типа процессора
            TextType.Text = xml.Read( XmlClass.NameHexFiles, "type" );
            // Считывание контрольной суммы первоначального загрузчика процессора
            TextCs.Text = xml.Read( XmlClass.NameHexFiles, "checksum" );
            // Считывание пути доступа к НЕХ-файлу процессора
            TextPath.Text = xml.Read( XmlClass.NameHexFiles, "path" );
            //
            // Чтение настроек LOG-файла
            //
            // Считывание пути доступа для хранеия тестов модулей прошедших ПСИ
            TextPathLog.Text = xml.Read( XmlClass.NameLogFiles,  "directory" );
            // Считывание признака сохранения лога в pdf-файл
            CheckBoxPdfEnable.IsChecked = xml.Read( XmlClass.NameLogFiles, "enable" ) == "YES";
            //
            // Чтение настроек LOG-файла
            //
            // Считывание признака загрузки НЕХ-файла в основной канал
            CheckBoxEnable1.IsChecked = xml.Read( XmlClass.NameDebug, "load" + 0 ) == "YES";
            // Считывание признака загрузки НЕХ-файла в канал-модель
            CheckBoxEnable2.IsChecked = xml.Read( XmlClass.NameDebug, "load" + 1 ) == "YES";
        }

        /// <summary>
        /// Подпрограмма вывода заданого значения ComboBox
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="str"></param>
        private static void set_items( ComboBox comboBox, string str ) {
            if ( string.IsNullOrWhiteSpace( str ) ) return;
            // Поиск строки среди значений Items
            var res = comboBox.Items.Cast< object >().Any( i => i.ToString() == str );
            if ( false == res ) {
                // Добавление Item
                comboBox.Items.Add( str );
            }
            // Вывод на экран Item
            comboBox.Text = str;
        }

        /// <summary>
        /// Подпрограмма задания пути доступа к НЕХ-файлу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textPath_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e ) {
            // Директория содержащая НЕХ-файлы
            var path = AppDomain.CurrentDomain.BaseDirectory + App.TaskManager.ConfigProgram.PathHexFiles;
            if ( !Directory.Exists( path ) ) return;
            // Создание диалогового окна
            var open_file = new OpenFileDialog {
                RestoreDirectory = true,
                InitialDirectory = path,
                Filter           = @"HEX files (*.mot,*.s19 )|*.mot;*.s19|" + @"All files (*.*)|*.*"
            };
            // Настройка диалогового окна
            if ( open_file.ShowDialog() == false ) return;
            // Путь доступа к выбранному НЕХ-файлу
            var file_name = open_file.FileName;
            // Директория выбранного НЕХ-файла
            var dir_name = Path.GetDirectoryName( file_name ) + "\\";
            // Сравнение директории выбранного файла с директорией по умолчанию
            if ( string.Equals( dir_name, path, StringComparison.CurrentCultureIgnoreCase ) ) {
                // Директории совпадают
                // Указываем только имя НЕХ-файла
                file_name = Path.GetFileName( file_name );
            }
            // Сохранение пути к НЕХ-файлу
            TextPath.Text = file_name;
        }

        /// <summary>
        /// Подпрограмма обработки нажатия кнопки Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click( object sender, RoutedEventArgs e ) {
            var xml = new XmlClass();
            //
            // Настройки PCI-карт
            //
            // Считывание имени порта 1 RS232 из .ini файла  
            xml.Write( XmlClass.NameRs232, XmlClass.ElementPort + 0, ComboBoxCom1.Text );
            // Считывание номера платы PCI1721 из .ini файла
            xml.Write( XmlClass.NamePci1721, XmlClass.ElementBid + 0, ComboBoxPci1721.Text );
            // Считывание номера платы PCI1724U из .ini файла
            xml.Write( XmlClass.NamePci1724, XmlClass.ElementBid + 0, ComboBoxPci1724U.Text );
            // Считывание номера платы PCI1747U из .ini файла
            xml.Write( XmlClass.NamePci1747, XmlClass.ElementBid + 0, ComboBoxPci1747U.Text );
            // Считывание номера платы PCI1753 из .ini файла
            xml.Write( XmlClass.NamePci1753, XmlClass.ElementBid + 0, ComboBoxPci1753N1.Text );
            // Считывание номера платы PCI1753 из .ini файла
            xml.Write( XmlClass.NamePci1753, XmlClass.ElementBid + 1, ComboBoxPci1753N2.Text );
            // Считывание номера платы PCI429 из .ini файла 
            xml.Write( XmlClass.NamePci429, XmlClass.ElementSn + 0, TextBoxPci429N1.Text );
            // Считывание номера платы PCI429 из .ini файла 
            xml.Write( XmlClass.NamePci429, XmlClass.ElementSn + 1, TextBoxPci429N2.Text );
            //
            // Чтение настроек НЕХ-файла
            //
            // Считывание типа процессора
            xml.Write( XmlClass.NameHexFiles, "type", TextType.Text );
            // Считывание контрольной суммы первоначального загрузчика процессора
            xml.Write( XmlClass.NameHexFiles, "checksum", TextCs.Text );
            // Считывание пути доступа к НЕХ-файлу процессора
            xml.Write( XmlClass.NameHexFiles, "path", TextPath.Text );
            //
            // Чтение настроек LOG-файла
            //
            // Считывание пути доступа для хранеия тестов модулей прошедших ПСИ
            xml.Write( XmlClass.NameLogFiles, "directory", TextPathLog.Text );
            // Считывание признака сохранения лога в pdf-файл
            xml.Write( XmlClass.NameLogFiles, "enable", CheckBoxPdfEnable.IsChecked == true ? "YES" : "NOT" );
            xml.Write( XmlClass.NameLogFiles, "e-mail", "volkov.vik1@gmail.com" );
            //
            // Чтение настроек LOG-файла
            //
            // Считывание признака отладки ПО
            // Считывание признака загрузки НЕХ-файла в основной канал
            xml.Write( XmlClass.NameDebug, "load" + 0, CheckBoxEnable1.IsChecked == true ? "YES" : "NOT" );
            // Считывание признака загрузки НЕХ-файла в канал-модель
            xml.Write( XmlClass.NameDebug, "load" + 1, CheckBoxEnable2.IsChecked == true ? "YES" : "NOT" );
            // Проверка необходимости изменения настроек СОМ-портов
            com_change();
            Close();
        }

        private void buttonCancel_Click( object sender, RoutedEventArgs e ) {
            Close();
        }

        /// <summary>
        /// Подпрограмма изменение настройе СОМ-портов
        /// </summary>
        private static void com_change() {
            var ports = App.TaskManager.PortCom;
            if ( ports == null || !ports.IsInit ) return;
            try {
                ports.Close();
                ports.Open();
                // Проверка инициализации портов
                if ( ports.IsInit ) {
                    App.MyWindows.TextLine += "Настройки портов RS232 изменены успешно";
                } else {
                    App.MyWindows.TextLine += "Настройки портов RS232 изменены c ошибками";
                }
            }
            catch ( Exception< Rs232ExceptionArgs > exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc,
                    "Изменение настроек СОМ-портов завершено с ошибками" + Environment.NewLine +
                    "Перезапустите программу" );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDelUser_Click( object sender, RoutedEventArgs e ) {
            // Удалить xml-файл пользователей
            //_xmlUsers.Delete();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextPathLog_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e ) {
            // Создание диалогового окна
            var dialog = new FolderBrowserDialog {
                SelectedPath = AppDomain.CurrentDomain.BaseDirectory
            };
            if ( dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK  ) {
                TextPathLog.Text = dialog.SelectedPath;
            }
        }
    }
}