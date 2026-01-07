using Microsoft.AspNetCore.Identity;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interface
{
    public interface IRoleRepository
    {
        Task<PagedList<IdentityRole>> GetAllRoles(RoleParameters roleParameters, bool trackChanges);
        Task<IdentityRole> GetRoleById(string roleId, bool trackChanges);

        //Task<IdentityRole> ActualizarRol(string id, bool)

        void CreateRol(IdentityRole role);
        void DeleteRol(IdentityRole role);
    }
}
