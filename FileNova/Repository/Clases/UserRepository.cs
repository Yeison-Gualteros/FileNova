using Contracts.Interface;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository.Clases
{
    public class UserRepository : IUserRepository
    {
        private readonly RepositoryContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(
            RepositoryContext context,
            UserManager<User> userManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<PagedList<User>> GetUsersAsync(UserParameters parameters)
        {
            var query = _context.Users
                .Include(u => u.User_Permisos)
                    .ThenInclude(up => up.Permiso)
                .FilteUser()
                .SearchUser(parameters.Busqueda)
                .SortUser(parameters.Orden);


            var count = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedList<User>(
                items,
                count,
                parameters.PageNumber,
                parameters.PageSize
            );
        }

        public async Task<User?> GetByIdAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.User_Permisos)
                    .ThenInclude(up => up.Permiso)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }



        public async Task<Role?> GetUserRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault();

            if (roleName == null)
                return null;

            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }

        

    }
}