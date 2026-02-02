using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;
using Shared.DataTransferObjects.User;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        


        public UserService(IRepositoryManager repository, IMapper mapper, ILoggerManager logger, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserDto> ActualizarUser(string id, UserForUpdateDto dto, bool trackChanges)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("Usuario no encontrado");

            _mapper.Map(dto, user);

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (dto.RoleIds != null && dto.RoleIds.Any())
            {
                var roleName = dto.RoleIds.First();
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                await _userManager.AddToRoleAsync(user, roleName);
            }

            if (dto.Estado.HasValue)
                user.Estado = dto.Estado.Value;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                UserName = user.UserName,
                Estado = user.Estado,
                Rol = roles.FirstOrDefault() ?? "Sin rol"
            };
        }


        public async Task<UserDto> CreateUser(UserForRegistrationDto userForRegistration, bool trackChanges)
        {
            // 🔹 Verificar si ya existe un usuario con el mismo nombre, apellido y correo
            var usuarioExistente = await _repository.User.GetUserByEmailNombreApellido(
                userForRegistration.Email,
                userForRegistration.UserName,
                userForRegistration.Nombre,
                userForRegistration.Apellido,
                trackChanges
            );

            if (usuarioExistente != null)
            {
                if (usuarioExistente.Estado == 0)
                    throw new Exception("Ya existe un usuario con este nombre, apellido y correo que fue eliminado.");
                else
                    throw new Exception("Ya existe un usuario con este nombre, apellido y correo.");
            }

            // 🔹 Crear el usuario
            var user = new User
            {
                UserName = userForRegistration.UserName,
                Email = userForRegistration.Email,
                Nombre = userForRegistration.Nombre,
                Apellido = userForRegistration.Apellido,
                Estado = userForRegistration.Estado == 3 ? 3 : 1,
                MustChangePassword = true
            };

            var password = string.IsNullOrWhiteSpace(userForRegistration.Password)
                ? $"Temp@{Guid.NewGuid():N}".Substring(0, 12)
                : userForRegistration.Password;

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // 🔹 Asignar rol
            if (userForRegistration.RoleIds != null && userForRegistration.RoleIds.Any())
            {
                var roleId = userForRegistration.RoleIds.First();
                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null)
                    throw new Exception("Rol no existe");

                await _userManager.AddToRoleAsync(user, role.Name);
            }

            // 🔹 Devolver DTO
            return new UserDto
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                UserName = user.UserName,
                Estado = user.Estado,
                Rol = userForRegistration.RoleIds?.FirstOrDefault()
            };
        }



        public async Task DeleteUser(string id, bool trackChanges)
         {
            var userDelete = await _repository.User.GetUserById(id, trackChanges);

            if (userDelete == null)
                throw new UserNotFoundException(id);

            userDelete.Estado = 0;
            await _repository.SaveAsync();

        }

        public async Task<(IEnumerable<UserDto> users, MetaData metaData)> GetAllUsers(UserParameters userParameters, bool trackChanges)
        {
            var usersPaged = await _repository.User.GetAllUsers(userParameters, trackChanges);

            var usersDto = _mapper.Map<List<UserDto>>(usersPaged);

            foreach (var dto in usersDto)
            {
                var roleName = await _repository.User.GetUserRoleNombre(dto.Id);
                dto.Rol = roleName ?? "Sin rol";
            }

            return (usersDto, usersPaged.MetaData);
        }

        public async Task<UserDto> GetUserById(string id, bool trackChanges)
        {
            var userFromDb = await _repository.User.GetUserById(id.ToString(), trackChanges);
            if (userFromDb is null)
            {
                _logger.LogError($"User with id: {id} doesn't exist in the database.");
                throw new KeyNotFoundException($"User with id: {id} doesn't exist in the database.");
            }
            return _mapper.Map<UserDto>(userFromDb);
        }

        //public async Task AssignRoleToUser(string userId, string roleName)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null) throw new KeyNotFoundException("Usuario no encontrado");

        //    var roles = await _userManager.GetRolesAsync(user);
        //    if (roles.Any())
        //    {
        //        // Remueve roles existentes si solo quieres uno por defecto
        //        await _userManager.RemoveFromRolesAsync(user, roles);
        //    }

        //    await _userManager.AddToRoleAsync(user, roleName);
        //}

    }
}
