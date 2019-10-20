using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppLibrary.Learn {

    public class Photo {

        private void SortFoto() {

            var watch = System.Diagnostics.Stopwatch.StartNew();

            GetFiles( @"d:\Volkov\_projects\MC21\MUP\_changes\2018_12_01_mv\" );
            watch.Stop();
            //App.MyWindows.TextLine += $"Время: {watch.Elapsed}";
            return;

            const string path        = @"c:\Users\User\Downloads\takeout-20190724T110812Z-001\Takeout\Google Фото\";
            var          month_names = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

            // Получаем все директории
            var current_directory = new DirectoryInfo( path );
            var directories       = current_directory.GetDirectories( "*", SearchOption.AllDirectories );
            var files = current_directory.GetFiles( "*.*", SearchOption.AllDirectories )
                                         .Where( o => o.Extension == ".jpg" ).ToList();

            //App.MyWindows.TextLine += $"Каталогов : {directories.Length}";
            //App.MyWindows.TextLine += $"Файлов    : {files.Count}";

            // Partition the entire source array.
            var range_partitioner = Partitioner.Create( 0, directories.Length );
            // Loop over the partitions in parallel.
            Parallel.ForEach( range_partitioner, ( range, loopState ) => {
                // Loop over each range element without a delegate invocation.
                for ( var i = range.Item1; i < range.Item2; i++ ) {
                    var files_list = directories[ i ].GetFiles( "*.*", SearchOption.TopDirectoryOnly )
                                                     .Where( o => o.Extension == ".jpg" ).ToList();

                    if ( files_list.Count == 0 ) continue;
                    foreach ( var file in files_list ) {
                        ////App.MyWindows.TextLine += $"{file,-40}{file.FullName}";
                        string path_directory;
                        var    date_created = GetTimeFoto( file );
                        if ( date_created != null ) {
                            var data = ( DateTime ) date_created;
                            // Создание директории
                            path_directory =
                                $"C:\\Users\\User\\Downloads\\Takeout2\\{data.Year}\\{data.Month - 1:D2}. " +
                                $"{month_names[ data.Month - 1 ]}\\{data:dd-MMMM-yyyy}";
                        } else {
                            path_directory = @"C:\Users\User\Downloads\Takeout2\";

                            int year, month, day;
                            if ( int.TryParse( directories[ i ].Name.Substring( 0, 4 ), out year )  &&
                                 int.TryParse( directories[ i ].Name.Substring( 5, 2 ), out month ) &&
                                 int.TryParse( directories[ i ].Name.Substring( 8, 2 ), out day ) ) {
                                var data = new DateTime( year, month, day );
                                path_directory =
                                    $"C:\\Users\\User\\Downloads\\Takeout2\\{data.Year}\\{data.Month - 1:D2}. " +
                                    $"{month_names[ data.Month - 1 ]}\\{data:dd-MMMM-yyyy}";
                            } else {
                                path_directory += "C:\\Users\\User\\Downloads\\Takeout2\\NotSort\\" +
                                                  directories[ i ].Name;
                            }
                        }
                        if ( !Directory.Exists( path_directory ) ) Directory.CreateDirectory( path_directory );
                        // Копирование файлов .jpg
                        var path_file = path_directory + "\\" + file.Name;
                        if ( File.Exists( path_file ) ) {
                            ////App.MyWindows.TextLine += $"{file,-40}    NOT COPY";
                            continue;
                        }
                        file.CopyTo( path_file );
                    }
                }
            } );

            //App.MyWindows.TextLine +=
            //    "Copy" + new DirectoryInfo( @"C:\Users\User\Downloads\Takeout2" )
            //             .GetFiles( "*.*", SearchOption.AllDirectories ).Length;
            watch.Stop();
            //App.MyWindows.TextLine += $"Время: {watch.Elapsed}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void GetFiles( string path ) {
            //App.MyWindows.TextLine += $"{path}";
            // Получаем все директории
            var current_directory = new DirectoryInfo( path );
            var top_directories   = current_directory.GetDirectories( "*", SearchOption.TopDirectoryOnly );
            var top_files         = current_directory.GetFiles( "*.*", SearchOption.TopDirectoryOnly );
            //var files = current_directory.GetFiles( "*.*", SearchOption.AllDirectories ).Where( o=> o.Extension == ".jpg" ).ToList();
            //App.MyWindows.TextLine += $"Каталогов = {top_directories.Length:D5}  Файлов = {top_files.Length:D5}";

            foreach ( var files in top_files ) //App.MyWindows.TextLine += $"    {files}";

            if ( top_directories.Length == 0 ) return;
            // Цикл всех директорий
            foreach ( var dir in top_directories ) GetFiles( dir.FullName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private DateTime? GetTimeFoto( FileSystemInfo file ) {
            DateTime? time;

            // Create an Image object. 
            using (Image image = new System.Drawing.Bitmap( file.FullName ) ) {
                // Get the PropertyItems property from image.
                if ( image.PropertyIdList.Length <= 0 || image.PropertyIdList.All( o => o != 0x132 ) ) return null;
                var property = image.GetPropertyItem( 0x132 );
                var text     = new System.Text.ASCIIEncoding().GetString( property.Value, 0, property.Len - 1 );
                time = DateTime.ParseExact( text, "yyyy:MM:d H:m:s", CultureInfo.InvariantCulture );
            }
            return time;
        }
    }
}