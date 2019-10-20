using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace AppLibrary.SQLite
{
    /// <summary>
    /// 
    /// </summary>
    public class Device : INotifyPropertyChanged {
        public int Id { get; set; }

        private int _number;

        public int Number {
            get { return _number; }
            set {
                _number = value;
                OnPropertyChanged( "Number" );
            }
        }

        private int _data;

        public int Data {
            get { return _data; }
            set {
                _data = value;
                OnPropertyChanged( "Data" );
            }
        }

        private string _datatime;

        public string DataTime {
            get { return _datatime; }
            set {
                _datatime = value;
                OnPropertyChanged( "DataTime" );
            }
        }

        private string _user;

        public string User {
            get { return _user; }
            set {
                _user = value;
                OnPropertyChanged( "User" );
            }
        }

        private string _agentOtk;

        public string AgentOtk {
            get { return _agentOtk; }
            set {
                _agentOtk = value;
                OnPropertyChanged( "AgentOtk" );
            }
        }

        private string _agentMilitary;

        public string AgentMilitary {
            get { return _agentMilitary; }
            set {
                _agentMilitary = value;
                OnPropertyChanged( "AgentMilitary" );
            }
        }

        private int _temperature;

        public int Temperature {
            get { return _temperature; }
            set {
                _temperature = value;
                OnPropertyChanged( "Temperature" );
            }
        }

        private string _result;

        public string Result {
            get { return _result; }
            set {
                _result = value;
                OnPropertyChanged( "Result" );
            }
        }

        private string _dectription;

        public string Dectription {
            get { return _dectription; }
            set {
                _dectription = value;
                OnPropertyChanged( "Dectription" );
            }
        }

        private string _fileName;

        public string FileName {
            get { return _fileName; }
            set {
                _fileName = value;
                OnPropertyChanged( "FileName" );
            }
        }

        private byte[] _file;

        public byte[] File {
            get { return _file; }
            set {
                _file = value;
                OnPropertyChanged( "File" );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged( [CallerMemberName] string prop = "" ) {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( prop ) );
        }

        public Device() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="time"></param>
        /// <param name="user"></param>
        /// <param name="agentOtk"></param>
        /// <param name="agentMilitary"></param>
        /// <param name="temperature"></param>
        /// <param name="result"></param>
        /// <param name="dectription"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        public Device( int number, DateTime time, string user, string agentOtk, string agentMilitary,
            int temperature, string result, string dectription, string fileName, byte[] file ) {
            _number        = number;
            _data          = ( int ) ( time - new DateTime( 1970, 1, 1, 0, 0, 0 ) ).TotalSeconds;
            _datatime      = time.ToString( CultureInfo.CurrentCulture );
            _user          = user;
            _agentOtk      = agentOtk;
            _agentMilitary = agentMilitary;
            _temperature   = temperature;
            _result        = result;
            _dectription   = dectription;
            _fileName      = fileName;
            _file          = file;
        }
    }


    public class ApplicationContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }

        public ApplicationContext() : base( "DefaultConnection" ) {}
        
        //    //var db = new ApplicationContext();
        //    //db.Phones.Load();

        //    //var ii = db.Phones.Local;

        //    //Phone phone = new Phone();
        //    //phone.Company = "xaomi";
        //    //db.Phones.Add( phone );
        //    //db.SaveChanges();
    }
}
