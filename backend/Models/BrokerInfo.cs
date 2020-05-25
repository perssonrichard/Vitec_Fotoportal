using System;
using System.ComponentModel.DataAnnotations;

namespace Photobox.Models {
    /// <summary>
    /// BrokerInfo object
    /// </summary>
    public class BrokerInfo {
        private string _employeeId;
        private string _installationId;

        /// <summary>
        /// Employee id
        /// </summary>
        [Required]
        public string employeeId { get => _employeeId; set => _employeeId = value; }

        /// <summary>
        /// Broker company id
        /// </summary>
        [Required]
        public string installationId { get => _installationId; set => _installationId = value; }
    }
}
