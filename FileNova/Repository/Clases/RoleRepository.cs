using Contracts.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Clases
{
    public class RoleRepository : RepositoryBase<IdentityRole>, IRoleRepository
    {
        public RoleRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<PagedList<IdentityRole>> GetAllRoles( RoleParameters roleParameters, bool trackChanges)
        {
            var query = FindByCondition(r => true, trackChanges)
                .SearchRole(roleParameters.Busqueda)
                .SortRole(roleParameters.Orden ?? "Name");

            var count = await query.CountAsync();

            var items = await query
                .Skip((roleParameters.PageNumber - 1) * roleParameters.PageSize)
                .Take(roleParameters.PageSize)
                .ToListAsync();

            return new PagedList<IdentityRole>(
                items,
                count,
                roleParameters.PageNumber,
                roleParameters.PageSize
            );
        }



        public async Task<IdentityRole> GetRoleById(string roleId, bool trackChanges) =>
            await FindByCondition(r => r.Id.Equals(roleId), trackChanges)
                .SingleOrDefaultAsync();

        public void CreateRol(IdentityRole role) => Create(role);
        public void DeleteRol(IdentityRole role) => Delete(role);

    }
}
