using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Photobox.Models {
    public class UpdatedPassword {
        private string _email;
        private string _oldPassword;
        private string _newPassword;

        /// <summary>
        /// Photographer email
        /// <exception cref="System.FormatException">Will be thrown on invalid email</exception>
        /// </summary>
        [Required]
        [StringLength (256)]
        public string email {
            get => _email;

            set {
                // Will throw a FormatException if not valid
                var isValid = new MailAddress (value);
                _email = value;
            }
        }

        [Required]
        [StringLength (256)]
        public string oldPassword { get => _oldPassword; set => _oldPassword = value; }

        [Required]
        [StringLength (256)]
        public string newPassword { get => _newPassword; set => _newPassword = value; }
    }
}