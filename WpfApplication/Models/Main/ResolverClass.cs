using System;
using System.IO;
using System.Reflection;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// 
    /// </summary>
    public static class Resolver {
        /// <summary>
        /// Подпрограмма загрузки данных сборки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Assembly CurrentDomain_AssemblyResolve( object sender, ResolveEventArgs args ) {
            try {
                // Получаем сборку, которая содержит выполняемый в текущий момент код.
                var exec_assembly = Assembly.GetExecutingAssembly();
                var name          = new AssemblyName( args.Name ).Name;
                var resource_name = $"{exec_assembly.GetName().Name}.Models.DLL.{name}.dll";
                // Запускаем поток, который загружает указанный ресурс манифеста из сборки.
                using ( var stream = exec_assembly.GetManifestResourceStream( resource_name ) ) {
                    if ( stream != null ) {
#if DEBUG
                        App.MyWindows.TextLine += $"Загрузка сборки {resource_name}";
#endif
                        var assembly_data = new byte[ stream.Length ];
                        // Чтение данных
                        var readed = stream.Read( assembly_data, 0, assembly_data.Length );
                        if ( assembly_data.Length == readed ) {
                            // Загрузка данных
                            return Assembly.Load( assembly_data );
                        }
                    }
                }
            }
            catch ( ArgumentNullException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            catch ( ArgumentException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            catch ( FileLoadException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            catch ( FileNotFoundException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            catch ( BadImageFormatException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            catch ( IOException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            catch ( NotSupportedException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            catch ( ObjectDisposedException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            catch ( NotImplementedException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка загрузки сборки" );
            }
            return null;
        }

        /// <summary>
        /// Подпрограмма выгрузки данных сборки
        /// </summary>
        /// <param name="filename"></param>
        public static void ExtractDllResourceToFile( string filename ) {
            try {
                if ( File.Exists( filename ) ) {
                    return;
                }
                // Получаем сборку, которая содержит выполняемый в текущий момент код.
                var exec_assembly = Assembly.GetExecutingAssembly();
                var resource_name = $"{exec_assembly.GetName().Name}.Models.DLL.{filename}";
                // Запускаем поток, который загружает указанный ресурс манифеста из сборки.
                using ( var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream( resource_name ) ) {
                    if ( stream == null ) {
                        return;
                    }
                    using ( var fstream = new FileStream( filename, FileMode.Create ) ) {
                        var assembly_data = new byte[ stream.Length ];
                        // Чтение данных 
                        var readed = stream.Read( assembly_data, 0, assembly_data.Length );
                        if ( assembly_data.Length == readed ) {
                            // Запись данных
                            fstream.Write( assembly_data, 0, assembly_data.Length );
                        }
                    }
                }
            }
            catch ( ArgumentNullException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
            catch ( ArgumentException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
            catch ( FileLoadException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
            catch ( FileNotFoundException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
            catch ( BadImageFormatException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
            catch ( IOException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
            catch ( NotSupportedException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
            catch ( ObjectDisposedException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
            catch ( NotImplementedException exc ) {
                App.MyWindows.ShowFormErrorCommand.Execute( exc, "Ошибка выгрузки сборки" );
            }
        }

        /// <summary>
        /// Подпрограмма получения списка всех DLL в ресурсах
        /// </summary>
        public static string[] GetAllAssemblyResource() {
            var this_assembly = Assembly.GetExecutingAssembly();
            var all_assembly  = this_assembly.GetManifestResourceNames();
            return all_assembly;
        }
    }
}