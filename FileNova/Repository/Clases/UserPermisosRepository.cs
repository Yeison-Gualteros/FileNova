using Contracts.Interface;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Clases
{
    public class UserPermisosRepository : IUserPermisosRepository
    {
        private readonly RepositoryContext _context;

        public UserPermisosRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<User_Permiso> GetByUserIdAndPermisoId(Guid userId, int permisoId)
        {
            return await _context.user_Permisos
                .FirstOrDefaultAsync(up => up.UserId == userId.ToString() && up.Id_Permiso == permisoId);
        }

        public async Task<IEnumerable<User_Permiso>> GetPermissionsForUser(Guid userId)
        {
            return await _context.user_Permisos
                .Where(up => up.UserId == userId.ToString())
                .ToListAsync();
        }

        public async Task Create(User_Permiso userPermiso)
        {
            await _context.user_Permisos.AddAsync(userPermiso);  // Agregar la relación
            await _context.SaveChangesAsync();
        }

        public IQueryable<User_Permiso> FindByCondition(Expression<Func<User_Permiso, bool>> expression, bool trackChanges)
        {
            return trackChanges
                ? _context.user_Permisos.Where(expression)  // Si hay seguimiento de cambios
                : _context.user_Permisos.AsNoTracking().Where(expression);  // Si no hay seguimiento de cambios
        }


        public async Task Delete(User_Permiso userPermiso)
        {
            _context.user_Permisos.Remove(userPermiso);  // Elimina la relación
            await _context.SaveChangesAsync();
        }
    }

}
