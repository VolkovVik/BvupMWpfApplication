using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

// ReSharper disable InheritdocConsiderUsage
namespace BvupMLinkLibrary.Models {
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class Rs232ExceptionArgs : ExceptionArgs {
        /// <summary>
        /// 
        /// </summary>
        private string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Rs232ExceptionArgs( string name = "" ) {
            Name = name;
        }

        /// <summary>
        /// Override the Message property to include our field (if set)
        /// </summary>
        public override string Message => $"Exception of the protocol of exchange on port RS232 {Name}" +
                                          Environment.NewLine                                           +
                                          base.Message;
    }

    /// <summary>
    /// A base class that a custom exception would derive from in order to add its own exception arguments.
    /// </summary>
    [Serializable]
    public abstract class ExceptionArgs {
        /// <summary>
        /// The string message associated with this exception.
        /// </summary>
        public virtual string Message => string.Empty;
    }

    /// <summary>Represents errors that occur during application execution.</summary>
    /// <typeparam name="TExceptionArgs">The type of exception and any additional arguments associated with it.</typeparam>
    [Serializable]
    public sealed class Exception< TExceptionArgs > : Exception, ISerializable where TExceptionArgs : ExceptionArgs {
        // For (de)serialization
        private const    string         CArgs = "Args";
        private readonly TExceptionArgs _mArgs;

        /// <summary>
        /// Returns a reference to this exception's additional arguments.
        /// </summary>
        //public TExceptionArgs Args => _mArgs;
        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception. 
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public Exception( string message = null, Exception innerException = null )
            : this( null, message, innerException ) { }

        // The fourth public constructor because there is a field
        /// <summary>
        /// Initializes a new instance of the Exception class with additional arguments, 
        /// a specified error message, and a reference to the inner exception 
        /// that is the cause of this exception. 
        /// </summary>
        /// <param name="args">The exception's additional arguments.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public Exception( TExceptionArgs args, string message = null, Exception innerException = null )
            : base( message, innerException ) {
            _mArgs = args;
        }

        /// <summary>
        /// Конструктор для десериализации; так как класс запечатан, конструктор
        /// закрыт. Для незапечатанного класса конструктор должен быть защищенным
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter )]
        private Exception( SerializationInfo info, StreamingContext context )
            : base( info, context ) {
            // Let the base deserialize its fields
            _mArgs = ( TExceptionArgs ) info.GetValue( CArgs, typeof( TExceptionArgs ) );
        }

        /// <summary>
        /// When overridden in a derived class, sets the SerializationInfo with information about the exception.
        /// Метод для сериализации; он открыт из-за интерфейса ISerializable
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        [SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter )]
        public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
            info.AddValue( CArgs, _mArgs );
            base.GetObjectData( info, context );
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message {
            get {
                var base_msg = base.Message;
                return _mArgs == null ? base_msg : "(" + _mArgs.Message + ")" + Environment.NewLine + base_msg;
            }
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current Object.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object. </param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals( object obj ) {
            var other = obj as Exception< TExceptionArgs >;
            if ( other == null ) return false;
            return Equals( _mArgs, other._mArgs ) && base.Equals( obj );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}