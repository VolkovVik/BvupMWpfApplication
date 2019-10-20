using System;
using System.Windows.Input;

namespace WpfApplication.ViewModels {
    /// <summary>
    /// 
    /// </summary>
    public class RelayCommand : ICommand {
        private readonly Action< object >     _execute;
        private readonly Func< object, bool > _canExecute;

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested    += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand( Action< object > execute, Func< object, bool > canExecute = null ) {
            _execute    = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute( object parameter ) {
            return _canExecute == null || _canExecute( parameter );
        }

        public void Execute( object parameter ) {
            _execute( parameter );
        }

        public void Execute( Exception exc, string text = "" ) {
            _execute( new StructConvTo2Parameters { Exc = exc, Text = text } );
        }
    }

    public struct StructConvTo2Parameters {
        public Exception Exc;
        public string    Text;
    }
}