using System.Data;
using System.Data.SQLite;
using System.IO;

namespace AppLibrary.SQLite {

    /// <summary>
    /// База данных
    /// </summary>
    public class SqLiteClass
    {

        ///<remarks>
        /// Типы данных
        /// NULL – null, просто null
        /// INTEGER – целое число
        /// REAL – вещественное число
        /// TEXT – текст
        /// BLOB – блок данных
        ///</remarks>

        /// <summary>
        /// Имя файла базы данных SQLite
        /// </summary>
        // ReSharper disable once MemberInitializerValueIgnored
        private string DatabaseName { get; set; } = "database.sqlite";

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        protected SqLiteClass() : this( "device.db" ) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected SqLiteClass( string name ) {
            DatabaseName = name;
            // Создание базы данных
            if ( !File.Exists( DatabaseName ) ) SQLiteConnection.CreateFile( DatabaseName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        protected object Set( string command ) => Set( command, new SQLiteParameter[ 0 ] );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        protected object Set( string command, SQLiteParameter[] param ) {
            object            result              = null;
            SQLiteTransaction sq_lite_transaction = null;

            if ( !File.Exists( DatabaseName ) ) return null;
            // Подключение
            using ( var sq_lite_connect = new SQLiteConnection( $"Data Source={DatabaseName}; Version=3;" ) ) {
                try {
                    // Открытие соединения
                    sq_lite_connect.Open();
                    // Проверка открытия соединения
                    // ReSharper disable once InvertIf
                    if ( sq_lite_connect.State == ConnectionState.Open ) {
                        // Создание запроса
                        using ( var sq_lite_command = new SQLiteCommand() ) {
                            sq_lite_command.Connection  = sq_lite_connect;
                            sq_lite_command.CommandText = command;
                            sq_lite_command.Parameters.AddRange( param );
                            // Начало транзакции
                            sq_lite_transaction         = sq_lite_connect.BeginTransaction();
                            sq_lite_command.Transaction = sq_lite_transaction;
                            // Выполнения запроса
                            // Выполнение SQL-команды, не предполагающей возвращения данных,
                            // выполняется вызовом метода ExecuteNonQuery().
                            result = sq_lite_command.ExecuteNonQuery();
                            // Успешное завершение транзакции
                            sq_lite_transaction.Commit();
                        }
                    }
                }
                catch ( SQLiteException exc ) {
                    // Откат транзакции
                    sq_lite_transaction?.Rollback();
                    
                }
                finally {
                    // Закрытие соединения
                    if ( sq_lite_connect.State == ConnectionState.Open ) sq_lite_connect.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// SQL-команда с возвращением единственного значения
        /// </summary>
        /// <param name="command"></param>
        protected object GetOne( string command ) {
            object result = null;

            if ( !File.Exists( DatabaseName ) ) return null;
            // Подключение
            using ( var sq_lite_connect = new SQLiteConnection( $"Data Source={DatabaseName}; Version=3;" ) ) {
                try {
                    // Открытие соединения
                    sq_lite_connect.Open();
                    // Проверка открытия соединения
                    if ( sq_lite_connect.State == ConnectionState.Open ) {
                        // Создание запроса
                        using ( var sq_lite_command = new SQLiteCommand() ) {
                            sq_lite_command.Connection  = sq_lite_connect;
                            sq_lite_command.CommandText = command;
                            // Выполнения запроса
                            // Выполнение SQL-запроса, предполагающего возвращение единственного значения,
                            // выполняется вызовом метода ExecuteScalar():
                            result = sq_lite_command.ExecuteScalar();
                        }
                    }
                }
                catch ( SQLiteException exc ) {
                    
                }
                finally {
                    // Закрытие соединения
                    if ( sq_lite_connect.State == ConnectionState.Open ) sq_lite_connect.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// SQL-команда с возвращением множественных значений
        /// </summary>
        /// <param name="command"></param>
        protected void GetRow( string command ) {
            if ( !File.Exists( DatabaseName ) ) return;

            // Подключение
            using ( var sq_lite_connect = new SQLiteConnection( $"Data Source={DatabaseName}; Version=3;" ) ) {
                try {
                    // Открытие соединения
                    sq_lite_connect.Open();
                    // Проверка открытия соединения
                    if ( sq_lite_connect.State == ConnectionState.Open ) {
                        // Создание запроса
                        using ( var sq_lite_command = new SQLiteCommand() ) {
                            sq_lite_command.Connection  = sq_lite_connect;
                            sq_lite_command.CommandText = command;
                            // Выполнения запроса
                            // Выполнение SQL-запроса, предполагающей возврат множества данных. 
                            // Построчное считывание результатов выполнения команды с помощью объекта класса SQLiteDataReader
                            using ( var reader = sq_lite_command.ExecuteReader() ) {
                                while ( reader.Read() ) {
                                    // Вывод пользователю
                                    //App.MyWindows.Text += $"{reader.FieldCount}    ";
                                    //for ( var i = 0; i < reader.FieldCount; i++ ) {
                                    //    App.MyWindows.Text += $"{reader[ i ]}  ";
                                    //}
                                    //App.MyWindows.TextLine += "";
                                }
                                reader.Close();
                            }
                        }
                    }
                }
                catch ( SQLiteException exc ) {
                    
                }
                finally {
                    // Закрытие соединения
                    if ( sq_lite_connect.State == ConnectionState.Open ) sq_lite_connect.Close();
                }
            }
        }

        /// <summary>
        /// SQL-команда с возвращением множественных значений
        /// </summary>
        /// <param name="command"></param>
        protected DataTable GetTable( string command ) {
            var table = new DataTable();
            table.Reset();

            if ( !File.Exists( DatabaseName ) ) return table;
            // Подключение
            using ( var sq_lite_connect = new SQLiteConnection( $"Data Source={DatabaseName}; Version=3;" ) ) {
                try {
                    // Открытие соединения
                    sq_lite_connect.Open();
                    // Проверка открытия соединения
                    // ReSharper disable once InvertIf
                    if ( sq_lite_connect.State == ConnectionState.Open ) {
                        // Выполнения запроса
                        // Выполнение SQL-запроса, предполагающей возврат множества данных. 
                        // Cчитывание результатов выполнения команды с помощью объекта класса DataTable, DataSet
                        using ( var adapter = new SQLiteDataAdapter( command, sq_lite_connect ) ) {
                            adapter.Fill( table );
                        }
                    }
                }
                catch ( SQLiteException exc ) {
                    
                }
                finally {
                    // Закрытие соединения
                    if ( sq_lite_connect.State == ConnectionState.Open ) sq_lite_connect.Close();
                }
                return table;
            }
        }
    }
}
