using Contracts.Interface;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;
using System.Data;

namespace Repository.Clases
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<PagedList<Role>> GetAllRoles(RoleParameters roleParameters, bool trackChanges)
        {
            var query = FindByCondition(r => true, trackChanges)
                        .SearchRole(roleParameters.Busqueda);

            var orderBy = $"{roleParameters.Orden} {roleParameters.Direccion}";
            query = query.SortRole(orderBy);

            var rolesList = await query
                .Select(r => new Role
                {
                    Id = r.Id,
                    Name = r.Name,
                    NormalizedName = r.NormalizedName
                })
                .ToListAsync();

            return PagedList<Role>.ToPageList(
                rolesList,
                roleParameters.PageNumber,
                roleParameters.PageSize
            );
        }


        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await RepositoryContext.Roles
                .Include(r => r.Permisos) // Incluye permisos
                .ToListAsync();
        }


        public async Task<Role> GetByIdAsync(int roleId)
        {
            return await RepositoryContext.Roles
                                 .Include(r => r.Permisos)  // Asegúrate de incluir la relación con Permisos
                                 .FirstOrDefaultAsync(r => r.Id == roleId.ToString());
        }


        public async Task<Role> GetRoleById(string roleId, bool trackChanges) =>
            await FindByCondition(r => r.Id.Equals(roleId), trackChanges)
                  .SingleOrDefaultAsync();

        public async Task<Role> GetById(int roleId, bool trackChanges)
        {
            return await RepositoryContext.Roles
                .AsNoTracking() // Si trackChanges es false, usamos AsNoTracking()
                .FirstOrDefaultAsync(role => role.Id == roleId.ToString());
        }

        public void CreateRol(Role role) => Create(role);

        public void DeleteRol(Role role) => Delete(role);

    }
}
