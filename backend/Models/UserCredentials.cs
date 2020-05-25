using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Photobox.Models {
    public class UserCredentials {
        private string _email;
        private string _password;

        [Required]
        public string email {
            get => _email;

            set {
                // Will throw a FormatException if not valid
                var isValid = new MailAddress (value);
                _email = value;
            }
        }

        [Required]
        public string password { get => _password; set => _password = value; }
    }
}