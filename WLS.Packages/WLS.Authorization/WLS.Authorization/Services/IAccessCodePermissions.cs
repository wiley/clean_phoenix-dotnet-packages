using System.Collections.Generic;
using System.Threading.Tasks;

using WLS.Authorization.Models;

namespace WLS.Authorization.Services
{
    public interface IAccessCodePermissions
    {
        Task<bool> CanCreate(int accountID);
    }
}
