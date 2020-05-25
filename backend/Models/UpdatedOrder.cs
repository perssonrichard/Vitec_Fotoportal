using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Photobox.Models
    {
        /// <summary>
        /// Order object
        /// </summary>
        public class UpdatedOrder
        {
            private string _orderId;
            private string _installationId;
            private string _estateId;
            private string _deptId;
            private string _userId;
            private string _userEmail;
            private string _username;
            private string _description;
            private string _address;
            private string _city;
            private int _postalCode;
            private StatusType _status;
            private DateTime _regDate;
            private string _photographerEmail;
            /// <summary>
            /// Unique id
            /// </summary>
            // [Required]
            public string orderId { get => _orderId; set => _orderId = value; }
            /// <summary>
            /// Broker company id
            /// </summary>
            // [Required]
            public string installationId { get => _installationId; set => _installationId = value; }
            /// <summary>
            /// Estate id
            /// </summary>
            // [Required]
            public string estateId { get => _estateId; set => _estateId = value; }
            /// <summary>
            /// Reference to broker department
            /// </summary>
            //[Required]
            public string deptId { get => _deptId; set => _deptId = value; }
            /// <summary>
            /// Broker id
            /// </summary>
            // [Required]
            public string userId { get => _userId; set => _userId = value; }
            /// <summary>
            /// Broker's full name
            /// </summary>
            // [Required]
            public string username { get => _username; set => _username = value; }
            /// <summary>
            /// Order description
            /// </summary>
            // [Required]
            public string description { get => _description; set => _description = value; }
            // [Required]
            [StringLength (256)]
            public string address { get => _address; set => _address = value; }
            // [Required]
            [StringLength (256)]
            public string city { get => _city; set => _city = value; }
            // [Required]
            public int postalCode { get => _postalCode; set => _postalCode = value; }
            /// <summary>
            /// Status of order
            /// </summary>
            // [Required]
            public StatusType status { get => _status; set => _status = value; }
            /// <summary>
            /// Timestamp when order was created
            /// </summary>
            // [Required]
            public DateTime regDate { get => _regDate; set => _regDate = value; }
            /// <summary>
            /// Photographer currently working on the order
            /// </summary>
            // [Required]
            public string photographerEmail { get => _photographerEmail; set => _photographerEmail = value; }
            /// <summary>
            /// User email
            /// <exception cref="System.FormatException">Will be thrown on invalid email</exception>
            /// </summary>
            // [Required]
            public string userEmail
            {
                get => _userEmail;
                set
                {
                    // Will throw a FormatException if not valid
                    // var isValid = new MailAddress (value);
                    _userEmail = value;
                }
            }
        }
    }
