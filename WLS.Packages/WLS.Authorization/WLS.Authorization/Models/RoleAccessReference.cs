using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WLS.Authorization.Models
{
    public class RoleAccessReference
    {
        public RoleType RoleType { get; set; }

        public AccessType AccessType { get; set; }

        public List<UserRoleAccess> UserRoleAccessList { get; set; }
    }
}
