using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Photobox.Models {
    public class Photographer {
        private string _email;
        private string _firstName;
        private string _lastName;
        private string _hashedPassword;
        private string _cellPhoneNumber;
        private string _address;
        private string _company;
        private string _orgNr;
        private string _city;
        private int _postalCode;
        private string _postalCodeArea;
        private List<string> _orderIdList;
        private bool _available = true;

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
        [StringLength (50)]
        public string firstName { get => _firstName; set => _firstName = value; }

        [Required]
        [StringLength (50)]
        public string lastName { get => _lastName; set => _lastName = value; }

        [Required]
        [StringLength (256)]
        public string hashedPassword { get => _hashedPassword; set => _hashedPassword = value; }

        [Required]
        [StringLength (50)]
        public string cellPhoneNumber { get => _cellPhoneNumber; set => _cellPhoneNumber = value; }

        [Required]
        [StringLength (256)]
        public string address { get => _address; set => _address = value; }

        [Required]
        [StringLength (256)]
        public string company { get => _company; set => _company = value; }

        [Required]
        [StringLength (256)]
        public string orgNr { get => _orgNr; set => _orgNr = value; }

        [Required]
        [StringLength (256)]
        public string city { get => _city; set => _city = value; }

        [Required]
        public int postalCode { get => _postalCode; set => _postalCode = value; }

        [Required]
        [StringLength (256)]
        public string postalCodeArea { get => _postalCodeArea; set => _postalCodeArea = value; }

        public List<string> orderIdList { get => _orderIdList; set => _orderIdList = value; }

        public bool available { get => _available; set => _available = value; }
    }
}