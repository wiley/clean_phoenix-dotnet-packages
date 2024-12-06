using System.ComponentModel.DataAnnotations;

namespace WLS.Authorization.Models
{
    public class AccessType
    {
        [Required]
        public int AccessTypeID { get; set; }

        [Required]
        [MaxLength(245)]
        public string AccessTypeName { get; set; }
    }
}