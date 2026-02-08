using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Permisos;
using Shared.DataTransferObjects.User;
using Shared.RequestFeatures;
using System.Security.Cryptography;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly RepositoryContext _context;
        private readonly IPermisosService _permisosService;
        private readonly IEmailService _emailService;


        public UserService(
            IRepositoryManager repository,
            IMapper mapper,
            ILoggerManager logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            RepositoryContext repositoryContext,
            IPermisosService permisosService,
            IEmailService emailService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = repositoryContext;
            _permisosService = permisosService;
            _emailService = emailService;
        }

        public async Task<PagedList<UserDto>> GetAllAsync(UserParameters parameters)
        {
            var users = await _repository.User.GetUsersAsync(parameters);

            var usersDto = new List<UserDto>();
            foreach (var user in users)
            {
                usersDto.Add(await MapUserAsync(user));
            }

            return new PagedList<UserDto>(
                usersDto,
                users.MetaData.TotalCount,
                users.MetaData.CurrentPage,
                users.MetaData.PageSize
            );
        }

        public async Task<UserDto> GetByIdAsync(string id)
        {
            var user = await _repository.User.GetByIdAsync(id)
                ?? throw new Exception("Usuario no existe");

            return await MapUserAsync(user);
        }

        public async Task<ServiceResultDto<UserDto>> CreateAsync(UserForRegistrationDto dto)
        {
            if (dto.RoleIds == null || !dto.RoleIds.Any())
            {
                return new ServiceResultDto<UserDto>
                {
                    Success = false,
                    Error = "Debe seleccionar un rol"
                };
            }

            var roleId = dto.RoleIds.First();
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return new ServiceResultDto<UserDto>
                {
                    Success = false,
                    Error = "El rol seleccionado no existe"
                };
            }

            // Crear usuario
            var user = _mapper.Map<User>(dto);
            user.UserName = dto.UserName!.ToLower();
            user.Estado = 1;
            user.MustChangePassword = true;

            var password = GenerarPasswordSegura();

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return new ServiceResultDto<UserDto>
                {
                    Success = false,
                    Error = string.Join(" | ", result.Errors.Select(e => e.Description))
                };
            }

            // 🔑 ASIGNAR ROL (BIEN HECHO)
            await _userManager.AddToRoleAsync(user, role.Name);

            // Permisos extra
            if (dto.Permisos != null)
            {
                await _permisosService.SaveUserPermisos(user.Id, dto.Permisos);
            }

            // Email
            try
            {
                await _emailService.SendPasswordAsync(
                    user.Email!,
                    user.UserName!,
                    password
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarn($"No se pudo enviar correo: {ex.Message}");
                // ❌ NO lanzar excepción
            }


            return new ServiceResultDto<UserDto>
            {
                Success = true,
                Data = await MapUserAsync(user)
            };
        }



        public async Task<UserDto> UpdateAsync(string id, UserForUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(id)
                ?? throw new Exception("Usuario no existe");

            _mapper.Map(dto, user);
            await _userManager.UpdateAsync(user);

            return await GetByIdAsync(user.Id);
        }


        // 🔑 MÉTODO CLAVE
        private async Task<UserDto> MapUserAsync(User user)
        {
            // Cargar permisos extra del usuario
            await _context.Entry(user)
                .Collection(u => u.User_Permisos)
                .Query()
                .Include(up => up.Permiso)
                .LoadAsync();

            // Obtener rol del usuario
            var roles = await _userManager.GetRolesAsync(user);
            var rolNombre = roles.FirstOrDefault();

            var roleEntity = string.IsNullOrEmpty(rolNombre)
                ? null
                : await _roleManager.FindByNameAsync(rolNombre);

            // Permisos del rol
            var permisosRol = roleEntity == null
                ? new List<Permiso>()
                : await _repository.Permisos
                    .GetPermisosPorRoleId(roleEntity.Id, false);

            // Mapear permisos
            var permisosRolDto = permisosRol.Select(p => new PermisosDto
            {
                Id_Permiso = p.Id_Permiso,
                Nombre = p.Nombre,
                Source = "role"
            });

            var permisosUsuarioDto = user.User_Permisos
                .Select(up => new PermisosDto
                {
                    Id_Permiso = up.Permiso.Id_Permiso,
                    Nombre = up.Permiso.Nombre,
                    Source = "user"
                });

            // 🔑 UnionBy por Id_Permiso para evitar duplicados
            var permisosFinales = permisosRolDto
                .UnionBy(permisosUsuarioDto, p => p.Id_Permiso)
                .ToList();

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Estado = user.Estado,
                Rol = rolNombre,
                Permisos = permisosFinales
            };
        }



        private static string GenerarPasswordSegura()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(9)) + "!";
        }


        public async Task UpdateFullAsync(string userId, UserForUpdateFullDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("Usuario no encontrado");

            // Actualizar campos básicos
            _mapper.Map(dto, user);
            await _userManager.UpdateAsync(user); // 👈 ÚNICO SAVE

            if (!string.IsNullOrWhiteSpace(dto.RoleId))
            {
                var role = await _roleManager.FindByIdAsync(dto.RoleId)
                    ?? throw new Exception("Rol no válido");

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                await _userManager.AddToRoleAsync(user, role.Name);

                // 🔹 Sincronizar permisos extra: eliminar los que ahora son parte del rol
                if (dto.PermisosIds != null)
                {
                    var permisosDelRol = await _repository.Permisos
                        .GetPermisosPorRoleId(dto.RoleId, false);

                    var permisosRolIds = permisosDelRol.Select(p => p.Id_Permiso).ToHashSet();

                    // Solo mantener permisos extra que NO estén en el rol
                    var permisosExtraFiltrados = dto.PermisosIds
                        .Where(p => !permisosRolIds.Contains(p))
                        .ToList();

                    await _permisosService.SaveUserPermisos(userId, permisosExtraFiltrados);
                }
            }
            else
            {
                // Si no se cambió de rol, simplemente actualizar permisos extra
                if (dto.PermisosIds != null && dto.PermisosIds.Any())
                {
                    await _permisosService.SaveUserPermisos(userId, dto.PermisosIds);
                }
                else if (dto.PermisosIds != null && !dto.PermisosIds.Any())
                {
                    // Eliminar TODOS los permisos extra
                    await _permisosService.SaveUserPermisos(userId, new List<int>());
                }

            }
        }

        public async Task<object> GetPermisosEdicionAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("Usuario no existe"); 
            // Rol
            var roles = await _userManager.GetRolesAsync(user);
            var rolNombre = roles.FirstOrDefault();
            var role = string.IsNullOrEmpty(rolNombre)
                ? null : await _roleManager.FindByNameAsync(rolNombre);
            // Permisos del rol
            var permisosRol = role == null
                ? new List<Permiso>() :
                await _repository.Permisos.GetPermisosPorRoleId(role.Id, false);

            var permisosRolIds = permisosRol.Select(p => p.Id_Permiso).ToHashSet(); 
            // Permisos extra del usuario
            await _context.Entry(user).Collection(u => u.User_Permisos).Query().Include(up => up.Permiso).LoadAsync();
            var permisosExtra = user.User_Permisos.Select(up => up.Permiso).ToList(); 
            var permisosExtraIds = permisosExtra.Select(p => p.Id_Permiso).ToHashSet();
            // TODOS los permisos
            var todos = await _repository.Permisos.GetAllPermisos(null, false);
            // DISPONIBLES = no rol y no extra
            var disponibles = todos.Where(p => !permisosRolIds.Contains(p.Id_Permiso) && !permisosExtraIds.Contains(p.Id_Permiso)).Select(p => new PermisosDto { Id_Permiso = p.Id_Permiso, Nombre = p.Nombre }).ToList();
            return new { permisosRol = permisosRol.Select(p => new PermisosDto { Id_Permiso = p.Id_Permiso, Nombre = p.Nombre, Source = "role" }), permisosExtra = permisosExtra.Select(p => new PermisosDto { Id_Permiso = p.Id_Permiso, Nombre = p.Nombre, Source = "user" }), permisosDisponibles = disponibles };
        }
    }
}
