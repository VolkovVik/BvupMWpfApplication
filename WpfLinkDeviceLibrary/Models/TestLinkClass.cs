using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows.Media;
using BvupMLinkLibrary.Views;

namespace BvupMLinkLibrary.Models {
    internal class TestLinkClass {
        /// <summary>
        /// Признак выхода
        /// </summary>
        private bool SignExit { get; set; }

        /// <summary>
        /// Команда
        /// </summary>
        private readonly byte[] _command = new ASCIIEncoding().GetBytes( "\r\n" );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="timePause"></param>
        public void Start( SerialPort port = null, int timePause = 100 ) {
            if ( port == null ) {
                BvupMLinkWindow.ScreenMain.SetText( "СОМ-порт не задан" );
                return;
            }
            var com_port = new ComPortClass( port );
            // Цикл теста
            SignExit = false;
            do {
                // Контроль
                var i = Test( com_port );
                BvupMLinkWindow.ScreenMain.SetText( i == 0 ? "Связь установлена" : "Нет связи" );
                Brush brush = new SolidColorBrush( i == 0 ? Colors.LightGreen : Colors.Red );
                brush.Freeze();
                BvupMLinkWindow.ScreenMain.SetColor( brush );
                Thread.Sleep( timePause );
            } while ( !SignExit );
            // Очистка буферов
            com_port.Erase();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop() {
            SignExit = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private int Test( ComPortClass port ) {
            int count_err;
            //Буфер приема данных из com-порта
            var buffer_in = new byte[ 32 ];

            try {
                // Сброс данных порта
                port.Erase();
                // Запись команды в порт
                port.Write( _command, 0, _command.Length );
                // Ожидаем ответа 
                Thread.Sleep( 10 );
                // Чтение данных из порта
                var readed = port.Read( buffer_in, 0, buffer_in.Length );
                //var str = new ASCIIEncoding().GetString(buffer_in, 0, readed);
                count_err = readed > 0 && buffer_in[ 0 ] != 0xFF ? 0 : 1;
            }
            catch ( Exception< Rs232ExceptionArgs > ) {
                count_err = 2;
            }
            return count_err;
        }
    }
}