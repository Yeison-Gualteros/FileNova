using Contracts.Interface;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Clases
{
    public class PermisosRepository : RepositoryBase<Permiso>, IPermisosRepository 
    {
        private readonly RepositoryContext _context;
        public PermisosRepository(RepositoryContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Permiso>> GetPermisosPorRole( string roleId, bool trackChanges)
        {
            return await _context.Rol_Permisos
                .Include(rp => rp.Permiso)  
                .Where(rp => rp.Id_Rol == roleId)
                .Select(rp => rp.Permiso)
                .ToListAsync();
        }



        public async Task<Permiso?> GetById(int permisoId)
        {
            return await FindByCondition(
                    p => p.Id_Permiso == permisoId,
                    trackChanges: false)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Permiso>> GetPermisosByIds(List<int> permisosIds)
        {
            return await _context.Permisos
                                 .Where(p => permisosIds.Contains(p.Id_Permiso))
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Permiso>> GetAllPermisos(int? id_Permisos, bool trackChanges)
        {
            if (id_Permisos.HasValue)
            {
                return await FindByCondition(
                        p => p.Id_Permiso == id_Permisos.Value,
                        trackChanges)
                    .ToListAsync();
            }

            return await FindAll(trackChanges)
                .ToListAsync();
        }



        public async Task<IEnumerable<Permiso>> GetUserPermisos(
            Guid userId, bool trackChanges)
        {
            return await _context.user_Permisos
                .Where(up => up.UserId == userId.ToString())
                .Select(up => up.Permiso)
                .ToListAsync();
        }


        public async Task<Permiso> CreatePermiso(Permiso permiso)
        {
            await _context.Permisos.AddAsync(permiso);
            await _context.SaveChangesAsync();
            return permiso;
        }

        public async Task<Permiso?> GetPermisoById(int id, bool trackChanges)
        {
            return await FindByCondition(
                p => p.Id_Permiso == id,
                trackChanges)
                .FirstOrDefaultAsync();
        }

        public void DeletePermiso(Permiso permiso)
        {
            Delete(permiso);
        }
    }
}
