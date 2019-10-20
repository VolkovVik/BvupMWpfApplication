using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace AppLibrary.SQLite {

    /// <summary>
    /// База данных
    /// </summary>
    public class DatabaseClass : SqLiteClass {

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DatabaseClass() { }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        public DatabaseClass( string database ) : base( database ) {}

        /// <summary>
        /// Создание таблицы базы данных
        /// </summary>
        /// <param name="table"></param>
        public void Create( string table ) =>
            Set( $"CREATE TABLE IF NOT EXISTS {table} ( "            +
                 "[Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                 "[Number] INTEGER, "                                +
                 "[Data] INTEGER, "                                  +
                 "[DataTime] TEXT, "                                 +
                 "[User] TEXT, "                                     +
                 "[AgentOtk] TEXT, "                                 +
                 "[AgentMilitary] TEXT, "                            +
                 "[Temperature] INTEGER, "                           +
                 "[Result] TEXT, "                                   +
                 "[Dectription] TEXT,"                               +
                 "[FileName] TEXT,"                                  +
                 "[File] BLOW )", new SQLiteParameter[ 0 ] );

        /// <summary>
        /// Добавление данных
        /// </summary>
        /// <param name="table"></param>
        /// <param name="device"></param>
        public void Insert( string table, Device device ) {
            //sq_lite_command.Parameters.AddWithValue( "@first_name", "Sergey" );
            //sq_lite_command.Parameters.Add( "@birth_date", DbType.DateTime ).Value = DateTime.Parse( "2000-01-15" );

            // SQLite не имеет специального типа данных для хранения даты и времени. 
            // Наиболее распространенным способом хранения данных значений является 
            // целое число секунд, прошедших от точки отчета 1970-01-01 00:00:00 UTC.
            // преобразования даты в целое число strftime('%s', value)
            // обратное преобразования datetime( Data, 'unixepoch' )

            var command =
                $"INSERT INTO {table} ( Number, Data, DataTime, User, AgentOtk, AgentMilitary, Temperature, Result, Dectription, FileName, File ) " +
                "VALUES ( @number, strftime('%s', @data), @datatime, @user, @otk, @pz, @temperature, @result, @dectription, @filename, @file );";
            var param = new[] {
                new SQLiteParameter( "@number", device.Number ),
                new SQLiteParameter( "@data", DateTime.Parse( device.DataTime ) ),
                new SQLiteParameter( "@datatime", device.DataTime ),
                new SQLiteParameter( "@user", device.User ),
                new SQLiteParameter( "@otk", device.AgentOtk ),
                new SQLiteParameter( "@pz", device.AgentMilitary ),
                new SQLiteParameter( "@temperature", device.Temperature ),
                new SQLiteParameter( "@result", device.Result ),
                new SQLiteParameter( "@dectription", device.Dectription ),
                new SQLiteParameter( "@filename", device.FileName ),
                new SQLiteParameter( "@file", device.File )
            };
            Set( command, param );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public IEnumerable< Device > Select( string table ) {

            var data = GetTable( $"SELECT * FROM {table}" );
            var list = new List< Device >();
            for ( var i = 0; i < data.Rows.Count; i++ ) {

                var item = data.Rows[ i ].ItemArray;
                //var data_time = new DateTime( 1970, 1, 1, 0, 0, 0 ).AddSeconds( ( int ) ( long ) j[ 2 ] );
                list.Add( new Device {
                    Id     = ( int ) ( long ) item[ 0 ],
                    Number = ( int ) ( long ) item[ 1 ],
                    // ReSharper disable once PossibleInvalidCastException
                    Data          = ( int ) ( long ) item[ 2 ],
                    DataTime      = ( string ) item[ 3 ],
                    User          = ( string ) item[ 4 ],
                    AgentOtk      = ( string ) item[ 5 ],
                    AgentMilitary = ( string ) item[ 6 ],
                    Temperature   = ( int ) ( long ) item[ 7 ],
                    Result        = ( string ) item[ 8 ],
                    Dectription   = ( string ) item[ 9 ],
                    FileName      = ( string ) item[ 10 ],
                    File          = ( byte[] ) item[ 11 ]
                } );
            }
            return list;
        }

        ///// <summary>
        ///// Изменение данных
        ///// </summary>
        ///// <param name="table"></param>
        ///// <param name="id"></param>
        ///// <param name="number"></param>
        //public void Update( string table, int id, int number ) {
        //    var command = $"UPDATE {table} SET 'number' = @number WHERE id = @id";
        //    var param = new[] {
        //        new SQLiteParameter( "@number", number ),
        //        new SQLiteParameter( "@id", id )
        //    };
        //    Set( command, param );
        //}

        /// <summary>
        /// Удаление данных
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        public void Delete( string table, int id ) {
            var command = $"DELETE FROM {table} WHERE id = @id";
            var param   = new[] { new SQLiteParameter( "@id", id ) };
            Set( command, param );
        }

        /// <summary>
        /// Удаление данных из таблицы
        /// </summary>
        /// <param name="table"></param>
        public void Delete( string table ) => Set( $"DELETE FROM {table}" );

        /// <summary>
        /// Удаление таблицы
        /// </summary>
        /// <param name="table"></param>
        public void Drop( string table ) => Set( $"DROP TABLE IF EXISTS {table}" );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void GetCount( string table ) => GetOne( $"SELECT COUNT( id ) FROM {table}" );

        /// <summary>
        /// Конвертация файл в байты byte[]
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetByte( string path ) {
            byte[] file_bytes;
            using ( var file_stream = new FileStream( path, FileMode.Open, FileAccess.Read ) ) {
                // откроем файл на чтение
                using ( var bin_reader = new BinaryReader( file_stream ) ) {
                    var count = new FileInfo( path ).Length;
                    file_bytes = bin_reader.ReadBytes( ( int ) count ); // файл в байтах
                }
            }
            return file_bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="name"></param>
        public static void GetFile( byte[] data, string name ) {
            using ( var file_stream = new FileStream( name, FileMode.Create, FileAccess.ReadWrite ) ) {
                var bin_writer = new BinaryWriter( file_stream );
                bin_writer.Write( data );
                bin_writer.Close();
            }
        }
    }
}
