using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interface
{
    public interface IUserRepository
    {
        Task<User> GetById(string userId, bool trackChanges);
        Task<PagedList<User>> GetAllUsers(UserParameters userParameters, bool trackChanges);
        Task<User> GetUserById  (string userId, bool trackChanges);
        Task<Role> GetUserRole(string userId);
        Task<List<Role>> GetRolesByUserId(string userId);
        Task<string> GetUserRoleNombre(string userId);
        Task<User> GetUserByEmailNombreApellido(string email, string userName, string nombre, string apellido, bool trackChanges);
        void CreateUser(User user);
        void DeleteUser(User user);
    }
}
