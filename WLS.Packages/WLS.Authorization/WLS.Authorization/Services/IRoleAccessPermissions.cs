﻿using System.Collections.Generic;
using System.Threading.Tasks;

using WLS.Authorization.Models;

namespace WLS.Authorization.Services
{
    public interface IRoleAccessPermissions
    {
        Task<List<RoleAccessReference>> GetRoleAccessReferences();
        Task<int> HasEPICAccountAccess(int epicAccountID);
    }
}