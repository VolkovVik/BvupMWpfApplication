using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApplication.Models.Main;
using WpfApplication.Models.Main.XML;

namespace WpfApplication.Models.Form {
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for WindowInit.xaml
    /// </summary>
    public partial class WindowInit : Window {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        public WindowInit( string title, string text ) {
            InitializeComponent();
            // Настройка изображений
            //var path = App.MyGmainTest.ConfigProgram.ImageUri;
            Icon          = new IconClass().get_icon();
            Image1.Source = new IconClass().get_image();
            ComboBoxOperatop.Items.Clear();
            ComboBoxPredOtk.Items.Clear();
            ComboBoxPredVp.Items.Clear();
            ComboBoxTemp.Items.Clear();
            var xml = new XmlClass( "users.xml" );
            if ( xml.Exist ) {
                // Список операторов
                var users = xml.Read( XmlClass.ElementOperator );
                foreach ( var i in users ) {
                    ComboBoxOperatop.Items.Add( i );
                }
                // Список представителей ОТК
                users = xml.Read( XmlClass.ElementOtk );
                foreach ( var i in users ) {
                    ComboBoxPredOtk.Items.Add( i );
                }
                // Список представителей ВП МО
                users = xml.Read( XmlClass.ElementVp );
                foreach ( var i in users ) {
                    ComboBoxPredVp.Items.Add( i );
                }
            }
            // Список температур
            ComboBoxTemp.Items.Add( "+70.0" );
            ComboBoxTemp.Items.Add( "+20.0" );
            ComboBoxTemp.Items.Add( "-55.0" );
            // Задание значений
            TextBoxNomer.Text = $"{App.TaskManager.ConfigProgram.Nomer}";
            ComboBoxTemp.Text = $"{App.TaskManager.ConfigProgram.Temp:F1}";
            if ( string.IsNullOrWhiteSpace( App.TaskManager.ConfigProgram.UserOperator ) ) {
                //ComboBoxOperatop.SelectedIndex = 0;
            } else {
                ComboBoxOperatop.Text = App.TaskManager.ConfigProgram.UserOperator;
            }
            if ( string.IsNullOrWhiteSpace( App.TaskManager.ConfigProgram.UserOtk ) ) {
                //ComboBoxPredOtk.SelectedIndex = 0;
            } else {
                ComboBoxPredOtk.Text = App.TaskManager.ConfigProgram.UserOtk;
            }
            if ( string.IsNullOrWhiteSpace( App.TaskManager.ConfigProgram.UserVp ) ) {
                // ComboBoxPredVp.SelectedIndex = 0;
            } else {
                ComboBoxPredVp.Text = App.TaskManager.ConfigProgram.UserVp;
            }
            TextBox.Text = text;
            Title        = title;
            Button.Focus();
            //var scope = FocusManager.GetFocusScope( Button );
            //FocusManager.SetFocusedElement( scope, null );
            //Keyboard.ClearFocus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click( object sender, RoutedEventArgs e ) {
            var xml = new XmlClass( "users.xml" );
            //
            // Проверка операторов
            //
            var name  = ComboBoxOperatop.Text;
            var users = xml.Read( XmlClass.ElementOperator );
            // Поиск строки среди значений users
            var res = users.Cast< object >().Any( i => i.ToString() == name );
            if ( false == res && !string.IsNullOrWhiteSpace( name ) ) {
                // Добавление оператора в список
                xml.Write( XmlClass.ElementOperator, "name", name, true );
            }
            //
            // Проверка представителей ОТК
            //
            name  = ComboBoxPredOtk.Text;
            users = xml.Read( XmlClass.ElementOtk );
            // Поиск строки среди значений users
            res = users.Cast< object >().Any( i => i.ToString() == name );
            if ( ( false == res ) && !string.IsNullOrWhiteSpace( name ) && !string.IsNullOrWhiteSpace( name ) ) {
                // Добавление представителей ОТК в список
                xml.Write( XmlClass.ElementOtk, "name", name, true );
            }
            //
            // Проверка представителей ВП МО
            //
            name  = ComboBoxPredVp.Text;
            users = xml.Read( XmlClass.ElementVp );
            // Поиск строки среди значений users
            res = users.Cast< object >().Any( i => i.ToString() == name );
            if ( ( false == res ) && !string.IsNullOrWhiteSpace( name ) && !string.IsNullOrWhiteSpace( name ) ) {
                // Добавление представителей ВП МО в список
                xml.Write( XmlClass.ElementVp, "name", name, true );
            }
            //
            // Задание значений
            //
            App.TaskManager.ConfigProgram.Nomer        = TextBoxNomer.Text;
            App.TaskManager.ConfigProgram.UserOperator = ComboBoxOperatop.Text;
            App.TaskManager.ConfigProgram.UserOtk      = ComboBoxPredOtk.Text;
            App.TaskManager.ConfigProgram.UserVp       = ComboBoxPredVp.Text;
            App.TaskManager.ConfigProgram.Temp         = ComboBoxTemp.Text.ToDoubleExt() ?? 0;
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ResetXml() {
            var xml = new XmlClass();
            // Включаем режим отладки
            if ( !xml.Exist ) {
                return;
            }
            // Считывание признаков загрузки НЕХ-файлов
            var list = xml.Read( XmlClass.NameDebug );
            var yes  = list.Count( item => item == "YES" );
            App.MyWindows.TextLine += yes == list.Count
                ? "Включен рабочий режим"
                : "Включен режим отладки";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_MouseDoubleClick( object sender, MouseButtonEventArgs e ) {
            ResetXml();
        }

        /// <summary>
        /// Подпрограмма обработчик события двойной клик мышкой по картинке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image1_MouseDown( object sender, MouseButtonEventArgs e ) {
            if ( e.ChangedButton == MouseButton.Left && e.ClickCount == 2 ) {
                ResetXml();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e ) {
            //if ( string.IsNullOrWhiteSpace( ComboBoxOperatop.Text ) ) {
            //    e.Cancel = true;
            //}
        }
    }
}