using Shared.DataTransferObjects.User;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IUserService
    {
        Task<(IEnumerable<UserDto> users, MetaData metaData)> GetAllUsers(UserParameters userParameters, bool trackChanges);
        Task<UserDto> GetUserById(string id, bool trackChanges);
        Task<UserDto> ActualizarUser(string id, UserForUpdateDto userForUpdate, bool trackChanges);
        Task<UserDto> CreateUser(UserForRegistrationDto userForRegistration, bool trackChanges);
        Task DeleteUser(string id, bool trackChanges);
        
        //Task AssignRoleToUser(string userId, string roleName);

    }
}
