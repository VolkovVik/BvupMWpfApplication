using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// Класс работы с электронной почтой
    /// </summary>
    internal class MailClass {
        /// <summary>
        /// Логин отправителя
        /// </summary>
        private string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        private string Password { get; set; }

        /// <summary>
        /// Подпрограмма выдачи сообщения об ошибке
        /// </summary>
        private readonly Action< Exception, string > _showErrorMessage; //App.MyWindows.ShowFormErrorCommand.Execute;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="action"></param>
        public MailClass( string login = "", string password = "", Action< Exception, string > action = null  ) {
            _showErrorMessage = action;
            Login             = string.IsNullOrWhiteSpace( login ) ? "mieakta312@gmail.com" : login;
            Password          = string.IsNullOrWhiteSpace( password ) ? "asLKrtNY1937mAdF" : password;
        }

        /// <summary>
        /// Подпрограмма отправки электронного письма
        /// </summary>
        /// <returns></returns>
        public async void SendEmailAsync( string address, string path ) {
            // TODO По хорошему нужно сделать 2-шаговую верификацию
            // Разблокировать небезопасные приложения
            // https://myaccount.google.com/lesssecureapps?pli=1
            try {
                var time = File.GetLastWriteTime( path );
                // Установка отправителя сообщения
                // Установка адрес и отображаемое в письме имя
                var from = new MailAddress( Login, App.TaskManager.ConfigProgram.NameDevice );
                // Установка получателя сообщения
                var to = new MailAddress( address );
                // Создание сообщения
                using ( var message = new MailMessage( from, to ) ) {
                    // Задание темы письма
                    message.Subject = "Тест";
                    // Задание текста письма
                    message.Body = $"<h2>Лог-файл теста от {time:HH:mm:ss dd MMMM yyyy года}</h2>";
                    // Письмо представляет код html
                    message.IsBodyHtml = true;
                    // Добавление файла
                    message.Attachments.Add( new Attachment( path ) );
                    // Создание smtp-клиента
                    // Задание адреса smtp-сервера и порта, с которого будем отправлять письмо
                    using ( var client = new SmtpClient( "smtp.gmail.com", 587 ) ) {
                        // Задание логина и пароля
                        client.Credentials = new NetworkCredential( Login, Password );
                        client.EnableSsl   = true;
                        // Выдача
                        await client.SendMailAsync( message );
                        // Выдача результата пользователю
                        App.MyWindows.TextLine += "Письмо отправлено";
                    }
                }
            }
            catch ( UnauthorizedAccessException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( PathTooLongException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( NotSupportedException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( ArgumentNullException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( ArgumentOutOfRangeException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( ArgumentException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( FormatException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( ObjectDisposedException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( InvalidOperationException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( SmtpFailedRecipientException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
            catch ( SmtpException exc ) {
                _showErrorMessage?.Invoke( exc, "Ошибка выдачи электронного письма" );
            }
        }
    }
}