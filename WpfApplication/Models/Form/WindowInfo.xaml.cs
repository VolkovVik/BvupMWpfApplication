using System;
using System.Windows;
using WpfApplication.Models.Main;

namespace WpfApplication.Models.Form {
    /// <summary>
    /// Interaction logic for WindowInfo.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class WindowInfo : Window {
        public WindowInfo() {
            InitializeComponent();
            // Настройка изображений
            //var path = App.MyGmainTest.ConfigProgram.ImageUri;
            Icon          = new IconClass().get_icon();
            Image1.Source = new IconClass().get_image();
            // Задание текстовой информации
            LabelTitle.Text       = AssemblyClass.get_title_file_exe();
            LabelDescription.Text = AssemblyClass.get_description_file_exe();
            LabelCompany.Text     = AssemblyClass.get_company_file_exe();
            LabelName.Text        = AssemblyClass.get_product_file_exe();
            LabelCopyright.Text   = AssemblyClass.get_сopyright_file_exe();
            LabelVersion.Text     = AssemblyClass.get_version_exe();
            LabelGuid.Text        = AssemblyClass.get_guid_file_exe();
            LabelData.Text        = $"{AssemblyClass.get_data_exe():dd MMMM yyyy HH:mm:ss}";
            LabelDeveloper.Text   = "Волков В.А., отдел 312";
            LabelVersionNet.Text  = Environment.Version.ToString();
            LabelVersionOs.Text   = $"{Environment.OSVersion} ( {Environment.OSVersion.Version} )";
            LabelPathExe.Text     = Environment.CurrentDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click( object sender, RoutedEventArgs e ) {
            Close();
        }
    }
}