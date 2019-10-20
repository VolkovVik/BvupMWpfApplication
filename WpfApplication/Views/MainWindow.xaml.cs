using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using BvupMLinkLibrary.Views;
using WpfApplication.Models.Main;
using WpfApplication.Models.Main.XML;
using WpfApplication.ViewModels;
using WindowInfo = WpfApplication.Models.Form.WindowInfo;
using WindowSettings = WpfApplication.Models.Form.WindowSettings;

namespace WpfApplication.Views {
    public partial class MainWindow : Window {
        /// <summary>
        /// Ссылка на словарь тестов
        /// </summary>
        private readonly Dictionary< int, ConfigTestsClass > _dict = App.TaskManager.ConfigProgram.DictCfgTest;

        /// <summary>
        /// Закладки второго уровня
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private TabControl[] TabControlL2;

        /// <summary>
        /// Массив сигнальных полей
        /// </summary>
        private Rectangle[][] _rectangleControlL1;

        /// <summary>
        /// Конструктор
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            // Указатель на форму
            //App.MyGmainForm = this;
            // Задание шрифтов
            //set_fonts();
            // Создание формы программным образом
            CreateTestForm();
            // Установка фокуса на кнопке "Тест" 
            ButtonStart.Focus();
        }

        /// <summary>
        /// Подпрограмма задания шрифта 
        /// </summary>
        private void set_fonts() {
            // Шрифты должны быть сохранены как Build Action = Resource
            // Получение списка шрифтов заданной директории
            var fonts     = Fonts.GetFontFamilies( new Uri( "pack://application:,,,/" ), "./Views/font/" );
            var font_name = "Courier New";
            if ( 0 < fonts.Count ) {
                font_name = $"{fonts.First().Source}, {font_name}";
            }
            // Задание шрифтов
            // Fira Mono в качестве основного
            // Courier New как запасной
            // XAML FontFamily="/WpfApplication;component/Program/Font/#PT Mono"
            // XAML FontFamily="./program/font/#PT Mono"
            //TextBox.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./models/font/#PT Mono, Courier New");
            TextBox.FontFamily = new FontFamily( new Uri( "pack://application:,,,/" ), font_name );
        }

#region Подпрограммы создания интерфейса

        /// <summary>
        /// Подпрограмма инициализации элемента TabControl
        /// </summary>
        private void CreateTestForm() {
            // Заголовок программы
            Title = AssemblyClass.get_description_file_exe();
            // Заголовок меню
            //var menu_name = ( MenuItem ) Menu.Items[ 0 ];
            //menu_name.Header = AssemblyClass.get_product_file_exe();
            // Кнопка запуска полного теста
            //ButtonStart.Content = $"Тест {App.MyGmainTest.ConfigProgram.NameDevice}";
            // Настройка иконки
            set_icon( App.TaskManager.ConfigProgram.ImageUri );
            // Настройка рисунка
            set_image( App.TaskManager.ConfigProgram.ImageUri );
            // Создание страниц TabControl первого уровня
            create_tab_l1();
            // Добавление пункта меню
            var xml     = new XmlClass();
            var address = xml.Read( XmlClass.NameLogFiles, "e-mail" );
            if ( string.IsNullOrWhiteSpace( address ) ) return;
            // Создание меню для парсинга elf-файла
            var email_menu_item = new MenuItem {
                Header = "Отправить лог-файл по email"
            };
            email_menu_item.Click += emailMenuItem_Click;
            MenuItemFile.Items.Add( email_menu_item );
        }

        /// <summary>
        /// Подпрограмма создания вкладок первого уровня
        /// </summary>
        private void create_tab_l1() {
            var list = new List< string >();
            // Получение списка вкладок первого уровня
            App.TaskManager.ConfigProgram.GetNameTabL1( list );
            // Создание TabControl второго уровня
            TabControlL2 = new TabControl[ list.Count ];
            // Создание одномерного массива из массивов 
            _rectangleControlL1      = new Rectangle[ list.Count + 1 ][];
            _rectangleControlL1[ 0 ] = new Rectangle[ list.Count ];

            // Создание вкладок первого уровня
            for ( var tab = 0; tab < list.Count; tab++ ) {
                // Создание Grid
                var grid_content = new Grid {
                    //ShowGridLines = true,
                    Background = Brushes.Transparent,
                    RowDefinitions = {
                        // Изменение 2018-10-04 Удаление сигнального поля
                        new RowDefinition { Height = new GridLength( 0 ) },
                        new RowDefinition()
                    }
                };
                // Создание заголовка вкладки
                Grid grid_header;
                create_header_item( 0, tab, list[ tab ], out grid_header );
                // Создание вкладок
                var tab_item = new TabItem {
                    Name    = "Tab",
                    Header  = grid_header, // установка заголовка вкладки
                    Content = grid_content // установка содержимого вкладки
                };
                // Добавление вкладок TabControl первого уровня
                TabControlL1.Items.Add( tab_item );
                // Создание вкладок второго уровня
                create_tab_control2( grid_content, tab );
            }
        }

        /// <summary>
        /// Подпрограмма создания вкладок второго уровня
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="index"></param>
        private void create_tab_control2( Grid grid, int index ) {
            var list = new List< string >();
            // Получение списка вкладок второго уровня
            App.TaskManager.ConfigProgram.GetNameTabL2( index, list );
            // Создание TabControl второго уровня
            TabControlL2[ index ] = new TabControl {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin              = new Thickness( 0, 0, 0, 0 ),
                VerticalAlignment   = VerticalAlignment.Stretch,
                TabStripPlacement   = Dock.Top,
                MinHeight           = 10,
                MinWidth            = 10
            };

            // Создание привязки
            // Установка привязки для элемента-передатчика
            var binding_cycle_work = new Binding {
                Source = App.MyWindows,
                Path   = new PropertyPath( "Index2" ),
                Mode   = BindingMode.TwoWay
            };
            // Установка привязки для элемента-приемника
            TabControlL2[ index ].SetBinding( Selector.SelectedIndexProperty, binding_cycle_work );
            // Первоначальная инициализация
            App.MyWindows.Index2 = 0;

            Grid.SetRow( TabControlL2[ index ], 1 );
            Grid.SetColumn( TabControlL2[ index ], 0 );
            Grid.SetColumnSpan( TabControlL2[ index ], list.Count );
            grid.Children.Add( TabControlL2[ index ] );
            // Получение максимального количества тестов на вкладке
            var max_test_page = App.TaskManager.ConfigProgram.GetMaxTestOnPage();
            // Создание массива сигнальных полей
            _rectangleControlL1[ index + 1 ] = new Rectangle[ list.Count ];
            // Создание вкладок второго уровня
            for ( var tab = 0; tab < list.Count; tab++ ) {
                // Создание колонок по количеству вкладок
                grid.ColumnDefinitions.Add( new ColumnDefinition() );
                // Создание заголовка вкладки
                Grid grid_header;
                create_header_item( index + 1, tab, list[ tab ], out grid_header );
                // Создание содержимого вкладки
                // Создание панели Grid содержимого вкладки
                var grid_content = new Grid {
                    //ShowGridLines = true,
                    Margin     = new Thickness( 0, 0, 0, 2 ),
                    Background = Brushes.Transparent,
                    RowDefinitions = {
                        new RowDefinition { Height = new GridLength( 25 ) },
                        new RowDefinition {
                            Height = new GridLength( 1, GridUnitType.Star )
                            //Height = new GridLength( 22 )
                        }
                    },
                    ColumnDefinitions = {
                        new ColumnDefinition { Width = new GridLength( 2, GridUnitType.Star ) },
                        new ColumnDefinition { Width = new GridLength( 2, GridUnitType.Star ) },
                        new ColumnDefinition { Width = new GridLength( 5, GridUnitType.Star ) },
                        new ColumnDefinition { Width = new GridLength( 5, GridUnitType.Star ) },
                        new ColumnDefinition { Width = new GridLength( 5, GridUnitType.Star ) },
                        new ColumnDefinition { Width = new GridLength( 5, GridUnitType.Star ) },
                        new ColumnDefinition { Width = new GridLength( 2, GridUnitType.Star ) }
                    }
                };
                for ( var i = 0; i < max_test_page; i++ ) {
                    grid_content.RowDefinitions.Add( new RowDefinition {
                        Height = new GridLength( 1, GridUnitType.Star )
                        //Height = new GridLength(22)
                    } );
                }

                // Создание вкладок TabControl второго уровня
                TabControlL2[ index ].Items.Add( new TabItem {
                    Header  = grid_header, // установка заголовка вкладки
                    Content = grid_content // установка содержимого вкладки
                } );
                // Создание элементов теста
                create_test_control( grid_content, index, tab );
            }
        }

        /// <summary>
        /// Подпрограмма создания заголовка
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="text"></param>
        /// <param name="grid"></param>
        private void create_header_item( int index1, int index2, string text, out Grid grid ) {
            // Создание заголовка вкладки
            // Создание сигнального поля вкладки
            var rectangle = new Rectangle {
                RadiusX   = 5,
                RadiusY   = 5,
                MinHeight = 16,
                Fill      = new SolidColorBrush( Color.FromArgb( 0, 255, 255, 255 ) ) //Colors.Transparent
            };
            // Создание текстового блока вкладки
            var textblock = new TextBlock {
                Background    = new SolidColorBrush( Colors.Transparent ),
                Text          = text,
                TextAlignment = TextAlignment.Center
            };
            // Сохранение указателя на сигнальное поле
            _rectangleControlL1[ index1 ][ index2 ] = rectangle;
            // Создание Grid заголовка вкладки
            grid = new Grid();
            grid.Children.Add( rectangle );
            grid.Children.Add( textblock );
        }

        /// <summary>
        /// Подпрограмма заполнения вкладок второго уровня
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="indexTab1"></param>
        /// <param name="indexTab2"></param>
        private void create_test_control( Panel grid, int indexTab1, int indexTab2 ) {
            // Создание общих элементов
            // цикл.
            var text_c = new Label {
                Content             = "цикл.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            Grid.SetRow( text_c, 0 );
            Grid.SetColumn( text_c, 0 );
            grid.Children.Add( text_c );
            // ц.ош
            var text_err = new Label {
                Content             = "ц.ош",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            Grid.SetRow( text_err, 0 );
            Grid.SetColumn( text_err, 1 );
            grid.Children.Add( text_err );
            // тест
            var text_test = new Label {
                Content             = "наименование проверки",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            Grid.SetRow( text_test, 0 );
            Grid.SetColumn( text_test, 2 );
            Grid.SetColumnSpan( text_test, 3 );
            grid.Children.Add( text_test );
            // результат
            var text_result = new Label {
                Content             = "результат",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            Grid.SetRow( text_result, 0 );
            Grid.SetColumn( text_result, 5 );
            grid.Children.Add( text_result );
            // Создание элементов для каждого теста
            var i_column = 1;

            for ( var j = 0; j < _dict.Count; j++ ) {
                var i = _dict.Keys.ToList()[ j ];
                //
                if (  indexTab1       != _dict[ i ].IndexTabL1 ||
                      indexTab2       != _dict[ i ].IndexTabL2 ||
                      _dict[ i ].Type != ConfigTestsClass.TypeTask.Test  ) {
                    continue;
                }
                //
                var check_cycle = new CheckBox {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Center,
                };
                Grid.SetRow( check_cycle, i_column );
                Grid.SetColumn( check_cycle, 0 );
                check_cycle.Checked += CheckBox_Changed;
                grid.Children.Add( check_cycle );
                //
                var check_cycle_err = new CheckBox {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Center,
                };
                Grid.SetRow( check_cycle_err, i_column );
                Grid.SetColumn( check_cycle_err, 1 );
                check_cycle_err.Checked += CheckBox_Changed;
                grid.Children.Add( check_cycle_err );

                //
                var button = new Button {
                    Content = _dict[ i ].NameButton,
                    Uid =
                        $"{( _dict[ i ].IndexTabL1 << 24 ) | ( _dict[ i ].IndexTabL2 << 16 ) | ( _dict[ i ].IndexPage & 0xFFFF ):D}",
                    Tag                        = "Старт отдельного теста",
                    Margin                     = new Thickness( 5, 0, 0, 1 ),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment   = VerticalAlignment.Center,
                    Style                      = ( Style ) FindResource( "BaseButtonStyle" ),
                    Command                    = App.MyWindows.StartFullTestCommand,
                    CommandParameter = ( _dict[ i ].IndexTabL1 << 24 ) |
                                       ( _dict[ i ].IndexTabL2 << 16 ) |
                                       ( _dict[ i ].IndexPage & 0xFFFF )
                    //Template = ( ControlTemplate ) FindResource( "BaseButtonTemplate" )
                };
                Grid.SetRow( button, i_column );
                Grid.SetColumn( button, 2 );
                Grid.SetColumnSpan( button, 3 );
                grid.Children.Add( button );
                //
                var textbox = new TextBox {
                    Text                = "НЕТ ДАННЫХ",
                    TextAlignment       = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment   = VerticalAlignment.Center,
                    Background          = Brushes.Transparent,
                    BorderThickness     = new Thickness( 0 ),
                    Margin              = new Thickness( 3, 1, 3, 1 )
                };
                Grid.SetRow( textbox, i_column );
                Grid.SetColumn( textbox, 5 );
                //textbox.TextChanged += textLed_changed;
                grid.Children.Add( textbox );
                // Сохранение указателей на элементы в словаре
                create_binding_data( _dict[ i ].IndexTabL1, _dict[ i ].IndexTabL2, _dict[ i ].IndexPage,
                    textbox, check_cycle, check_cycle_err );

                // Следующий элемент
                i_column++;
            }

            // запуск группы тестов
            //
            var button_group = new Button {
                Content          = "Старт группы",
                Tag              = "Старт группы тестов",
                Margin           = new Thickness( 5, 0, 0, 0 ),
                Style            = ( Style ) FindResource( "BaseButtonStyle" ),
                Command          = App.MyWindows.StartGroupTestCommand,
                CommandParameter = ( indexTab1 << 24 ) | ( indexTab2 << 16 )
                //Template = (ControlTemplate)FindResource("BaseButtonTemplate")
            };
            Grid.SetRow( button_group, i_column );
            Grid.SetColumn( button_group, 2 );
            grid.Children.Add( button_group );
            //
            var group_box = new GroupBox {
                Margin = new Thickness( 1, 0, 1, 0 )
            };
            Grid.SetRow( group_box, 0 );
            Grid.SetRowSpan( group_box, i_column + 1 );
            Grid.SetColumn( group_box, 0 );
            grid.Children.Add( group_box );
            //
            var group_box_err = new GroupBox {
                Margin = new Thickness( 1, 0, 1, 0 )
            };
            Grid.SetRow( group_box_err, 0 );
            Grid.SetRowSpan( group_box_err, i_column + 1 );
            Grid.SetColumn( group_box_err, 1 );
            grid.Children.Add( group_box_err );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexTab1"></param>
        /// <param name="indexTab2"></param>
        /// <param name="indexPage"></param>
        /// <param name="textbox"></param>
        /// <param name="checkCycleWork"></param>
        /// <param name="checkCycleWorkToErr"></param>
        private void create_binding_data( int? indexTab1, int? indexTab2, int? indexPage,
            TextBox textbox, CheckBox checkCycleWork, CheckBox checkCycleWorkToErr ) {
            if ( indexTab1 == null || indexTab2 == null || indexPage == null ) {
                return;
            }
            // Создание oбъекта привязки
            var binding_data = new TabControlViewModel();

            // Создание привязки
            // Установка привязки для элемента-передатчика
            var binding_cycle_work = new Binding {
                Source = binding_data,
                Path   = new PropertyPath( "CycleWorkEnable" ),
                Mode   = BindingMode.TwoWay
            };
            var binding_cycle_work_to_err = new Binding {
                Source = binding_data,
                Path   = new PropertyPath( "CycleWorkToErrorEnable" ),
                Mode   = BindingMode.TwoWay
            };
            var binding_signal_field_1 = new Binding {
                Source = binding_data,
                Path   = new PropertyPath( "SignalFieldBrush1" ),
                Mode   = BindingMode.TwoWay
            };
            var binding_signal_field_2 = new Binding {
                Source = binding_data,
                Path   = new PropertyPath( "SignalFieldBrush2" ),
                Mode   = BindingMode.TwoWay
            };
            var binding_result_text = new Binding {
                Source = binding_data,
                Path   = new PropertyPath( "ResultText" ),
                Mode   = BindingMode.TwoWay
            };
            var binding_result_text_brush = new Binding {
                Source = binding_data,
                Path   = new PropertyPath( "ResultTextBrush" ),
                Mode   = BindingMode.TwoWay
            };
            // Установка привязки для элемента-приемника
            checkCycleWork.SetBinding( ToggleButton.IsCheckedProperty, binding_cycle_work );
            checkCycleWorkToErr.SetBinding( ToggleButton.IsCheckedProperty, binding_cycle_work_to_err );

            _rectangleControlL1[ 0 ][ ( int ) indexTab1 ].SetBinding( Shape.FillProperty, binding_signal_field_1 );
            _rectangleControlL1[ ( int ) indexTab1 + 1 ][ ( int ) indexTab2 ]
                .SetBinding( Shape.FillProperty, binding_signal_field_2 );
            textbox.SetBinding( TextBox.TextProperty, binding_result_text );
            textbox.SetBinding( BackgroundProperty, binding_result_text_brush );
            // Первоначальная инициализация
            binding_data.CycleWorkEnable        = false;
            binding_data.CycleWorkToErrorEnable = false;
            binding_data.ResultText             = @"НЕТ ДАННЫХ";
            binding_data.ResultTextBrush        = Brushes.Transparent;
            binding_data.SignalFieldBrush1      = Brushes.Transparent;
            binding_data.SignalFieldBrush2      = Brushes.Transparent;
            // Сохранение в словаре тестов
            App.TaskManager.ConfigProgram.SetControl( indexTab1, indexTab2, indexPage, binding_data );
        }

#endregion Подпрограммы создания интерфейса

#region Потокобезопасные вызовы элементов WPF

        /// <summary>
        /// Делегат для работы с рисунком 
        /// </summary>
        private delegate void DelgImage( string uri );

        /// <summary>
        /// Подпрограмма задания иконки окна
        /// </summary>
        /// <param name="path"></param>
        private void set_icon( string path = "pack://application:,,,/models/icon/icon.ico" ) {
            // Осуществление потокобезопасного вызова элементов 
            if ( !Dispatcher.CheckAccess() ) {
                var add = new DelgImage( set_icon );
                Dispatcher.Invoke( add, path );
            } else {
                // Настройка иконки
                Icon = new IconClass( path ).get_icon();
            }
        }

        /// <summary>
        /// Подпрограмма задания изображения
        /// </summary>
        /// <param name="path"></param>
        private void set_image( string path = "pack://application:,,,/models/icon/icon.ico" ) {
            // Осуществление потокобезопасного вызова элементов 
            if ( !Dispatcher.CheckAccess() ) {
                var add = new DelgImage( set_image );
                Image1.Dispatcher.Invoke( add, path );
            } else {
                // Настройка иконки
                Image1.Source = new IconClass( path ).get_image();
            }
        }

#endregion Потокобезопасные вызовы элементов WPF

#region Обработчики событий элементов WPF

        /// <summary>
        /// Подпрограмма обработчик события изменение размеров главной формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnSizeChanged( object sender, SizeChangedEventArgs e ) {
            // Рассчет ширины TabItems элемента TabControl
            var width_l1 = ( TabControlL1.ActualWidth - 4 ) / TabControlL1.Items.Count;
            if ( width_l1 > 0 ) {
                // Изменение ширин TabItems
                foreach ( TabItem item in TabControlL1.Items ) {
                    item.MinWidth = width_l1;
                }
                // Изменение ширин сигнальных полей 
                foreach ( var rectangle in _rectangleControlL1[ 0 ] ) {
                    rectangle.MinWidth = width_l1 - width_l1 / 7.5;
                }
            }
            double width_l2 = 0;
            // Рассчет ширины TabItems элемента TabControl
            foreach ( var cntr in TabControlL2.Where( cntr => cntr.IsVisible ) ) {
                // Рассчет осуществляется по параметрам активной вкладки
                width_l2 = cntr.ActualWidth - 4;
            }
            // Рассчет ширины TabItems элемента TabControl
            foreach ( var cntr in TabControlL2 ) {
                var width_tab_l2 = width_l2 / cntr.Items.Count;
                // Изменение ширин TabItems
                foreach ( var x in cntr.Items.Cast< TabItem >() ) {
                    x.MinWidth = width_tab_l2;
                }
                // Изменение ширин сигнальных полей 
                for ( var i = 1; i < _rectangleControlL1.Length; i++ ) {
                    foreach ( var rectangle in _rectangleControlL1[ i ] ) {
                        rectangle.MinWidth = width_tab_l2 - width_tab_l2 / 7.5;
                    }
                }
            }
        }

        /// <summary>
        /// Подпрограмма обработчик события двойной клик мышкой по картинке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image1_MouseDown( object sender, MouseButtonEventArgs e ) {
            if ( e.ChangedButton == MouseButton.Left && e.ClickCount == 2 ) {
                // Включаем режим отладки
            }
        }

        /// <summary>
        /// Номер символа при поиске ошибки
        /// </summary>
        private int _indexSymbolView;

        /// <summary>
        /// Подпрограмма обработчик события нажатие на кнопку Поиск ошибок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFindError_Click( object sender, RoutedEventArgs e ) {
            var str = "отказ";
            if ( !string.IsNullOrWhiteSpace( TextFindError.Text ) ) {
                str = TextFindError.Text;
            }
            FindText( str );
            e.Handled = true;
        }

        /// <summary>
        /// Подпрограмма обработчик события нажатие на кнопку Поиск ошибок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFindError_OnKeyDown( object sender, KeyEventArgs e ) {
            if ( e.Key == Key.Return ) {
                FindText( TextFindError.Text );
            }
        }

        /// <summary>
        /// Подпрограмма поиска заданного слова в текстовом поле TextBox
        /// </summary>
        private void FindText( string str ) {
            if ( false == CheckBoxLog.IsChecked ) {
                return;
            }
            // Поиск данной подстроки c последнего места
            var i = TextBox.Text.IndexOf( str, _indexSymbolView, StringComparison.OrdinalIgnoreCase );
            if ( -1 == i ) {
                // Повторная попытка с начала страницы 
                // На случай завершения страницы
                i = TextBox.Text.IndexOf( str, 0, StringComparison.OrdinalIgnoreCase );
            }
            if ( -1 == i ) {
                // Строка не найдена
                _indexSymbolView = 0;
            } else {
                // Строка найдена
                // Индекс символа
                _indexSymbolView = i + 1;
                // Индекс строки
                var i_string        = TextBox.GetLineIndexFromCharacterIndex( _indexSymbolView );
                var i_first_visible = TextBox.GetFirstVisibleLineIndex();
                var i_last_visible  = TextBox.GetLastVisibleLineIndex();
                var i_visible       = i_last_visible - i_first_visible + 1;
                if ( i_visible > TextBox.LineCount - i_string ) {
                    var i1      = i_visible - TextBox.LineCount + i_string;
                    var add_str = string.Empty;
                    for ( var j = 0; j < i1; j++ ) {
                        add_str += Environment.NewLine;
                    }
                    TextBox.Text += add_str;
                }
                if ( i_string >= i_first_visible && i_string <= i_last_visible ) {
                    // Текст сбрасываем в нижнюю часть
                    TextBox.ScrollToEnd();
                    // Обновление тестового поля
                    TextBox.UpdateLayout();
                }
                // Переход к нужной строке
                TextBox.ScrollToLine( i_string );
                // Обновление тестового поля
                TextBox.UpdateLayout();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool _signChangeCheckbox = true;

        /// <summary>
        /// Подпрограмма обработчик события изменения состояния CheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Changed( object sender, EventArgs e ) {
            if ( !_signChangeCheckbox ) {
                return;
            }
            _signChangeCheckbox = false;
            if ( true == ( ( CheckBox ) sender ).IsChecked ) {
                // Сброс всех элементов
                foreach ( var items in _dict.Where( i => i.Value.Control != null ).Select( i => i.Value.Control ) ) {
                    items.CycleWorkEnable        = false;
                    items.CycleWorkToErrorEnable = false;
                }
                // Установка требуемого
                ( ( CheckBox ) sender ).IsChecked = true;
            }
            _signChangeCheckbox = true;
        }

        /// <summary>
        /// Подпрограмма обработчик события нажатие на вкладку меню Выход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExit_Click( object sender, RoutedEventArgs e ) {
            Close();
        }

        private void MenuSettingItem_Click( object sender, RoutedEventArgs e ) {
            new WindowSettings().ShowDialog();
        }

        /// <summary>
        /// Подпрограмма обработчик события нажатие на вкладку меню Информация
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemInfo_Click( object sender, RoutedEventArgs e ) {
            new WindowInfo().ShowDialog();
        }

        /// <summary>
        /// Продпрограмма 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void emailMenuItem_Click( object sender, RoutedEventArgs e ) {
            // Получение пути доступа к отправляемому файлу
            var path = App.TaskManager.Log.FileName;
            // Получение адреса получателя сообщения

            var xml     = new XmlClass();
            var address = xml.Read( XmlClass.NameLogFiles, "e-mail" );
            if ( !File.Exists( path ) || string.IsNullOrWhiteSpace( address ) ) return;
            new MailClass( action: App.MyWindows.ShowFormErrorCommand.Execute ).SendEmailAsync( address, path );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemId_Click( object sender, RoutedEventArgs e ) {
            // Запуск диалогового окна
            const string title = "Окно идентификационных данных";
            const string text  = "Проверьте допустимость заданных данных";
            App.MyWindows.ShowFormInitCommand.Execute( new[] { title, text } );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged( object sender, TextChangedEventArgs e ) {
            TextBox.ScrollToEnd();
        }

        private void MenuCheckConnectionItem_OnClick( object sender, RoutedEventArgs e ) {
            new BvupMLinkWindow().ShowDialog();
        }

#endregion Обработчики событий элементов WPF
    }
}