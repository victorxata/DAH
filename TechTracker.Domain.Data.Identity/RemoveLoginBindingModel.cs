using System.ComponentModel.DataAnnotations;

namespace TechTracker.Domain.Data.Identity
{
    /// <summary>
    /// Model to remove an external login
    /// </summary>
    public class RemoveLoginModel
    {
        /// <summary>
        /// The id of the external provider item
        /// </summary>
        [Required]
        public string LoginProvider { get; set; }

        /// <summary>
        /// The key of the external provider item
        /// </summary>
        [Required]
        public string ProviderKey { get; set; }
    }
}