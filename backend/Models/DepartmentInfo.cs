using System;
using System.ComponentModel.DataAnnotations;

namespace Photobox.Models {
    /// <summary>
    /// BrokerInfo object
    /// </summary>
    public class DepartmentInfo {
        private string _installationId;

        private string _departmentId;

        /// <summary>
        /// Installation id
        /// </summary>
        [Required]
        public string installationId { get => _installationId; set => _installationId = value; }

        /// <summary>
        /// Department id
        /// </summary>
        [Required]
        public string departmentId { get => _departmentId; set => _departmentId = value; }
    }
}
