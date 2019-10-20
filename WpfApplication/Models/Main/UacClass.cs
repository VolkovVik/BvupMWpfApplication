using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// Класс работы с учетными записями пользователей
    /// Контроль учётных записей пользователей 
    /// (англ. User Account Control, UAC)
    ///  — компонент операционных систем Microsoft Windows, 
    /// впервые появившийся в Windows Vista. 
    /// Этот компонент запрашивает подтверждение действий, 
    /// требующих прав администратора, в целях защиты от 
    /// несанкционированного использования компьютера. 
    /// </summary>
    internal static class UacClass {
        /// <summary>
        /// Подпрограмма проверки прав администратора
        /// </summary>
        /// <returns></returns>
        public static bool IsAdmin() {
            // https://habrahabr.ru/post/185264/
            // Проверка является ли пользователь администратором
            var id           = WindowsIdentity.GetCurrent();
            var my_principal = new WindowsPrincipal( id );
            return my_principal.IsInRole( WindowsBuiltInRole.Administrator );
        }

        /// <summary>
        /// Подпрограмма запуска процесса ПО с правами администратора
        /// </summary>
        public static void RunAsAdmin() {
            var process_info = new System.Diagnostics.ProcessStartInfo {
                UseShellExecute  = true,
                ErrorDialog      = true,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                FileName         = Assembly.GetExecutingAssembly().Location,
                Verb             = "runas"
            };
            try {
                // ReSharper disable once UnusedVariable
                var p = System.Diagnostics.Process.Start( process_info );
            }
            catch ( ObjectDisposedException exc ) {
                // Объект процесса уже удален.
                App.MyWindows.ShowFormErrorCommand.Execute( exc,
                    "Ошибка запуска процесса ПО с правами администратора" );
            }
            catch ( InvalidOperationException exc ) {
                // Имя файла не было указано в свойстве FileName параметра startInfo.
                // - или -
                // Свойство UseShellExecute параметраstartInfo имеет значение true 
                // и RedirectStandardInput, RedirectStandardOutput или свойство 
                // RedirectStandardError также имеет значение true.
                // - или -
                // Свойство UseShellExecute параметра startInfo имеет значение true, 
                // и свойство UserName не равно null или не является пустым 
                // или свойство Password не равно null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc,
                    "Ошибка запуска процесса ПО с правами администратора" );
            }
            catch ( ArgumentNullException exc ) {
                // Параметр startInfo имеет значение null.
                App.MyWindows.ShowFormErrorCommand.Execute( exc,
                    "Ошибка запуска процесса ПО с правами администратора" );
            }
            catch ( FileNotFoundException exc ) {
                // Файл, указанный в свойстве FileName параметра startInfo, не найден.
                App.MyWindows.ShowFormErrorCommand.Execute( exc,
                    "Ошибка запуска процесса ПО с правами администратора" );
            }
            catch ( Win32Exception exc ) {
                // Возникла ошибка при открытии связанного файла.
                // - или -
                // Сумма длины аргументов и длины полного пути к процессу превышает 2080.
                // Сообщение об ошибке, связанной с этим исключением, может иметь 
                // следующую формулировку: "Область данных, переданная системному вызову,
                // слишком мала" или "Отказано в доступе".
                App.MyWindows.ShowFormErrorCommand.Execute( exc,
                    "Ошибка запуска процесса ПО с правами администратора" );
            }
            catch ( Exception exc ) {
                // UAC elevation failed
                App.MyWindows.ShowFormErrorCommand.Execute( exc,
                    "Ошибка запуска процесса ПО с правами администратора" );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryRulesAccount( string path ) {
            var directory_name = new DirectoryInfo( path );
            var str            = $"Доступ к папке {directory_name.Name}" + Environment.NewLine;
            var ds1            = directory_name.GetAccessControl();
            // var ds1 = directoryName.GetAccessControl(AccessControlSections.Access);
            // С помощью метода GetAccessRules() можно указывать, 
            // что должны использоваться унаследованные правила доступа
            // и касающиеся не только доступа правила, определяемые с объектом.
            // В последнем параметре необходимо указывать тип идентификатора 
            // безопасности, который должен возвращаться. Этот тип должен быть
            // унаследован от базового класса IdentityReference. 
            // Возможными типами являются NTAccount и SecurityIdentifier. 
            // Оба эти класса представляют либо пользователей, либо группы пользователей: 
            // класс NTAccount предусматривает поиск объекта безопасности по его имени,
            // а класс SecurityIdentifier — по уникальному идентификатору безопасности.
            var rules = ds1.GetAccessRules( true, true, typeof( NTAccount ) );
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach ( FileSystemAccessRule rl in rules ) {
                str += $"{rl.IdentityReference.Value}"                                       + Environment.NewLine +
                       $"    right - {rl.FileSystemRights}  access - {rl.AccessControlType}" + Environment.NewLine;
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryRulesSecire( string path ) {
            var str = string.Empty;
            // Получение текущего пользователя Windows
            var wi = WindowsIdentity.GetCurrent();
            if ( wi.Groups != null ) {
                var directory_name = new DirectoryInfo( path );
                str += $"Доступ к папке {directory_name.Name}" + Environment.NewLine;
                var ds1 = directory_name.GetAccessControl();
                //var ds1 = directoryName.GetAccessControl( AccessControlSections.Access );
                // С помощью метода GetAccessRules() можно указывать, 
                // что должны использоваться унаследованные правила доступа
                // и касающиеся не только доступа правила, определяемые с объектом.
                // В последнем параметре необходимо указывать тип идентификатора 
                // безопасности, который должен возвращаться. Этот тип должен быть
                // унаследован от базового класса IdentityReference. 
                // Возможными типами являются NTAccount и SecurityIdentifier. 
                // Оба эти класса представляют либо пользователей, либо группы пользователей: 
                // класс NTAccount предусматривает поиск объекта безопасности по его имени,
                // а класс SecurityIdentifier — по уникальному идентификатору безопасности.
                var rules = ds1.GetAccessRules( true, true, typeof( SecurityIdentifier ) );
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach ( FileSystemAccessRule rl in rules ) {
                    var sid = ( SecurityIdentifier ) rl.IdentityReference;
                    if ( ( sid.IsAccountSid()  && wi.User == sid ) ||
                         ( !sid.IsAccountSid() && wi.Groups.Contains( sid ) ) ) {
                        str +=
                            $"{rl.IdentityReference.Value}"                                       +
                            Environment.NewLine                                                   +
                            $"    right - {rl.FileSystemRights}  access - {rl.AccessControlType}" + Environment.NewLine;
                    }
                }
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckDirectory( string path ) {
            var result = false;
            // Получение текущего пользователя Windows
            var wi = WindowsIdentity.GetCurrent();
            if ( wi.Groups != null ) {
                var directory_name = new DirectoryInfo( path );
                //var ds1 = directoryName.GetAccessControl( AccessControlSections.Access );
                var ds1 = directory_name.GetAccessControl();
                // С помощью метода GetAccessRules() можно указывать, 
                // что должны использоваться унаследованные правила доступа
                // и касающиеся не только доступа правила, определяемые с объектом.
                // В последнем параметре необходимо указывать тип идентификатора 
                // безопасности, который должен возвращаться. Этот тип должен быть
                // унаследован от базового класса IdentityReference. 
                // Возможными типами являются NTAccount и SecurityIdentifier. 
                // Оба эти класса представляют либо пользователей, либо группы пользователей: 
                // класс NTAccount предусматривает поиск объекта безопасности по его имени,
                // а класс SecurityIdentifier — по уникальному идентификатору безопасности.
                var rules = ds1.GetAccessRules( true, true, typeof( SecurityIdentifier ) );
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach ( FileSystemAccessRule rl in rules ) {
                    var sid = ( SecurityIdentifier ) rl.IdentityReference;
                    if ( ( sid.IsAccountSid()  && wi.User == sid ) ||
                         ( !sid.IsAccountSid() && wi.Groups.Contains( sid ) ) ) {
                        if ( ( rl.FileSystemRights & FileSystemRights.Write ) == FileSystemRights.Write ) {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Adds an ACL entry on the specified directory for the specified account.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="account"></param>
        /// <param name="rights"></param>
        /// <param name="controlType"></param>
        // ReSharper disable once UnusedMember.Local
        private static void AddDirectorySecurity( string fileName, string account, FileSystemRights rights,
            AccessControlType controlType ) {
            try {
                // Create a new DirectoryInfo object.
                var d_info = new DirectoryInfo( fileName );
                // Get a DirectorySecurity object that represents the 
                // current security settings.
                var d_security = d_info.GetAccessControl();
                // Add the FileSystemAccessRule to the security settings. 
                d_security.AddAccessRule( new FileSystemAccessRule( account, rights, controlType ) );
                // Set the new access settings.
                d_info.SetAccessControl( d_security );
            }
            catch ( PathTooLongException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( IOException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( ArgumentNullException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( ArgumentException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( System.Security.SecurityException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( UnauthorizedAccessException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( PlatformNotSupportedException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
        }

        /// <summary>
        /// Removes an ACL entry on the specified directory for the specified account.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="account"></param>
        /// <param name="rights"></param>
        /// <param name="controlType"></param>
        // ReSharper disable once UnusedMember.Local
        private static void RemoveDirectorySecurity( string fileName, string account, FileSystemRights rights,
            AccessControlType controlType ) {
            try {
                // Create a new DirectoryInfo object.
                var d_info = new DirectoryInfo( fileName );
                // Get a DirectorySecurity object that represents the 
                // current security settings.
                var d_security = d_info.GetAccessControl();
                // Add the FileSystemAccessRule to the security settings. 
                d_security.RemoveAccessRule( new FileSystemAccessRule( account, rights, controlType ) );
                // Set the new access settings.
                d_info.SetAccessControl( d_security );
            }
            catch ( PathTooLongException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( IOException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( ArgumentNullException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( ArgumentException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( System.Security.SecurityException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( UnauthorizedAccessException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
            catch ( PlatformNotSupportedException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc );
            }
        }

        /// <summary>
        /// Подпрограмма проверки прав доступа
        /// </summary>
        /// <returns></returns>
        public static string GetRole() {
            //https://msdn.microsoft.com/ru-ru/library/86wd8zba%28v=vs.110%29.aspx
            var my_domain = Thread.GetDomain();
            my_domain.SetPrincipalPolicy( PrincipalPolicy.WindowsPrincipal );
            var my_principal = ( WindowsPrincipal ) Thread.CurrentPrincipal;
            var str          = $"{my_principal.Identity.Name} belongs to: " + Environment.NewLine;
            var wbir_fields  = Enum.GetValues( typeof( WindowsBuiltInRole ) );
            foreach ( var role_name in wbir_fields ) {
                try {
                    // Cast the role name to a RID represented by the WindowsBuildInRole value.
                    str +=
                        $"RID 0x{( int ) role_name:X3}  {role_name,-15} - {my_principal.IsInRole( ( WindowsBuiltInRole ) role_name )}" +
                        Environment.NewLine;
                }
                catch ( Exception ) {
                    str += $"{role_name}: Could not obtain role for this RID" + Environment.NewLine;
                }
            }
            // Get the role using the WellKnownSidType.
            var sid = new SecurityIdentifier( WellKnownSidType.BuiltinAdministratorsSid, null );
            str += $"WellKnownSidType BuiltinAdministratorsSid  {sid.Value}? {my_principal.IsInRole( sid )}" +
                   Environment.NewLine;
            return str;
        }
    }
}