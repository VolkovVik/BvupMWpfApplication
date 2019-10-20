using System.ComponentModel;

namespace BvupMLinkLibrary.ViewModels {
    public class BaseViewModel : INotifyPropertyChanged {
        /// <inheritdoc />
        /// <summary>
        /// Событие для обработчика изменения свойства 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Подпрограмма обработчик изменения свойства 
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged( string propertyName = "" ) {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Подпрограмма обработчик изменения свойства 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e ) {
            PropertyChanged?.Invoke( this, e );
        }
    }
}