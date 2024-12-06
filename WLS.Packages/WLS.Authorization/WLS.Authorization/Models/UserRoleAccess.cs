using System;
using System.ComponentModel.DataAnnotations;

namespace WLS.Authorization.Models
{
    public class UserRoleAccess
    {
        [Required]
        public int UserRoleID { get; set; }

        [Required]
        public int AccessTypeID { get; set; }

        [Required]
        public int AccessRefID { get; set; }

        [Required]
        public int GrantedBy { get; set; }

        [Required]
        public DateTime Created { get; set; }
    }
}