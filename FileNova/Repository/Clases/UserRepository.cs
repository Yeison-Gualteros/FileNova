using Contracts.Interface;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;
using System.Drawing;
using System.Linq.Dynamic.Core;

namespace Repository.Clases
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly RepositoryContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(RepositoryContext context, UserManager<User> userManager)
            : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        public void CreateUser(User user)
        {
            Create(user);
        }

        public void DeleteUser(User user)
        {
            Delete(user);
        }


        public async Task<PagedList<User>> GetAllUsers(UserParameters userParameters, bool trackChanges)
        {
            var query = FindByCondition(u => u.Estado != 0, trackChanges)
                .FilteUser()
                .SearchUser(userParameters.Busqueda)
                .SortUser(userParameters.Orden);

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((userParameters.PageNumber - 1) * userParameters.PageSize)
                .Take(userParameters.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedList<User>(users, totalCount, userParameters.PageNumber, userParameters.PageSize);
        }




        public async Task<User> GetById(string userId, bool trackChanges)
        {
            return await FindByCondition(u => u.Id == userId, trackChanges)
                .SingleOrDefaultAsync();
        }


        public async Task<User> GetUserById(string userId, bool trackChanges)
        {
            if (!Guid.TryParse(userId, out Guid guid))
                return null;

            return await FindByCondition(u => u.Id == guid.ToString(), trackChanges)
                  .SingleOrDefaultAsync();
        }

        public async Task<List<Role>> GetRolesByUserId(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return new List<Role>();

            var roles = await _userManager.GetRolesAsync(user);

            return await _context.Roles.Where(r => roles.Contains(r.Name)).ToListAsync();
        }

        public async Task<string> GetUserRoleNombre(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return roles.FirstOrDefault();
        }

        public async Task<Role> GetUserRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var rolesNames = await _userManager.GetRolesAsync(user); // List<string>
            var roleName = rolesNames.FirstOrDefault(); // Tomar solo el primero si es uno por usuario

            if (roleName == null) return null;

            // Traer el objeto Role completo desde la base de datos
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            return role;
        }

        public async Task<User> GetUserByEmailNombreApellido(string email, string userName, string nombre, string apellido, bool trackChanges)
        {
            var query = FindByCondition(u =>
                u.Email == email &&
                u.UserName == userName &&
                u.Nombre == nombre &&
                u.Apellido == apellido &&
                u.Estado != 0,
                trackChanges);

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }



    }
}
