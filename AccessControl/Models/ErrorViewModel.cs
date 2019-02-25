using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControl.Models
{
    /// <summary>
    /// Used for error view
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Failing request
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// Show/hive
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    }
}
