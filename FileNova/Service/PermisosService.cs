using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using Shared.DataTransferObjects.Permisos;

namespace Service
{
    public class PermisosService : IPermisosService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public PermisosService(
            IRepositoryManager repository,
            ILoggerManager logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // =========================
        // CONSULTAS
        // =========================

        public async Task<IEnumerable<PermisosDto>> GetAllPermisos(
            int? id_Permiso,
            bool trackChanges)
        {
            var permisos = await _repository.Permisos
                .GetAllPermisos(id_Permiso, trackChanges);

            return permisos == null
                ? Enumerable.Empty<PermisosDto>()
                : _mapper.Map<IEnumerable<PermisosDto>>(permisos);
        }

        public async Task<IEnumerable<PermisosDto>> GetUserPermisos(
            Guid userId,
            bool trackChanges)
        {
            var rol = await _repository.User.GetUserRole(userId.ToString());
            if (rol == null)
                throw new KeyNotFoundException("El usuario no tiene rol asignado.");

            var permisosRol = await _repository.Permisos
                .GetPermisosPorRole(rol.Id, trackChanges);

            var permisosUsuario = await _repository.Permisos
                .GetUserPermisos(userId, trackChanges);

            var permisos = permisosRol
                .Concat(permisosUsuario)
                .Distinct();

            return _mapper.Map<IEnumerable<PermisosDto>>(permisos);
        }

        public async Task<IEnumerable<PermisosDto>> GetPermissionsByRole(string roleId)
        {
            // Traemos todos los permisos del rol como Rol_Permiso incluyendo Permiso
            var rolPermisos = await _repository.Rol_Permisos
                .GetPermisosByRole(Guid.Parse(roleId));

            // Mapear cada Rol_Permiso → PermisosDto
            return _mapper.Map<IEnumerable<PermisosDto>>(rolPermisos);
        }

        public async Task<IEnumerable<PermisosDto>> GetPermissionsByUser(string Id)
        {
            // Traemos todos los permisos del rol como Rol_Permiso incluyendo Permiso
            var rolPermisos = await _repository.Rol_Permisos
                .GetPermisosByRole(Guid.Parse(Id));

            // Mapear cada Rol_Permiso → PermisosDto
            return _mapper.Map<IEnumerable<PermisosDto>>(rolPermisos);
        }



        // =========================
        // CRUD
        // =========================

        public async Task<PermisosDto> CreatePermiso(PermisosForCreationDto permiso)
        {
            var entity = _mapper.Map<Permiso>(permiso);

            _repository.Permisos.Create(entity);
            await _repository.SaveAsync();

            return _mapper.Map<PermisosDto>(entity);
        }

        public async Task<PermisosDto> GetPermisoById(
            int id_Permiso,
            bool trackChanges)
        {
            var permiso = await _repository.Permisos
                .GetPermisoById(id_Permiso, trackChanges);

            if (permiso == null)
                throw new KeyNotFoundException("Permiso no encontrado.");

            return _mapper.Map<PermisosDto>(permiso);
        }

        public async Task<PermisosDto> UpdatePermiso(int id_Permiso, PermisoForUpdateDto dto)
        {
            var permiso = await _repository.Permisos
                .GetPermisoById(id_Permiso, true);

            if (permiso == null)
                throw new KeyNotFoundException("Permiso no encontrado.");

            _mapper.Map(dto, permiso);
            await _repository.SaveAsync();

            return _mapper.Map<PermisosDto>(permiso);
        }

        public async Task<bool> DeletePermiso(int id_Permiso)
        {
            var permiso = await _repository.Permisos
                .GetPermisoById(id_Permiso, true);

            if (permiso == null)
                return false;

            _repository.Permisos.Delete(permiso);
            await _repository.SaveAsync();

            return true;
        }

        // =========================
        // USUARIOS
        // =========================

        public async Task AddPermissionsToUser(Guid userId, List<int> permisosIds)
        {
            foreach (var permisoId in permisosIds)
            {
                _repository.UserPermisos.Create(new User_Permiso
                {
                    UserId = userId.ToString(),
                    Id_Permiso = permisoId
                });
            }

            await _repository.SaveAsync();
        }

        public async Task RemovePermissionFromUser(Guid userId, int permisoId)
        {
            var entity = _repository.UserPermisos
                .FindByCondition(up =>
                    up.UserId == userId.ToString() &&
                    up.Id_Permiso == permisoId,
                    trackChanges: true)
                .FirstOrDefault();

            if (entity == null)
                throw new KeyNotFoundException("Permiso no asignado al usuario.");

            _repository.UserPermisos.Delete(entity);
            await _repository.SaveAsync();
        }

        // =========================
        // ROLES
        // =========================

        public async Task AddPermissionsToRole(string roleId, List<int> permisosIds)
        {
            if (permisosIds == null || !permisosIds.Any()) return;

            foreach (var permisoId in permisosIds)
            {
                var existe = await _repository.Rol_Permisos.ExistsAsync(roleId, permisoId);
                if (existe) continue;

                _repository.Rol_Permisos.Create(new Rol_Permiso
                {
                    Id_Rol = roleId,
                    Id_Permiso = permisoId
                });
            }

            await _repository.SaveAsync();
        }

        public async Task RemovePermissionFromRole(string roleId, int permisoId)
        {
            var entity = await _repository.Rol_Permisos
                .FindByCondition(rp => rp.Id_Rol == roleId && rp.Id_Permiso == permisoId, trackChanges: true)
                .FirstOrDefaultAsync();

            if (entity == null) return;

            _repository.Rol_Permisos.Delete(entity);
            await _repository.SaveAsync();
        }

        // 🔹 Aquí agregamos el método de sincronización
        public async Task UpdatePermissionsOfRole(string roleId, List<int> permisosIds)
        {
            // Obtener permisos actuales del rol
            var permisosActuales = await _repository.Permisos
                .GetPermisosPorRole(roleId, trackChanges: true);

            var permisosActualesIds = permisosActuales.Select(p => p.Id_Permiso).ToList();

            // Eliminar permisos que ya no están seleccionados
            foreach (var permisoId in permisosActualesIds)
            {
                if (!permisosIds.Contains(permisoId))
                {
                    await RemovePermissionFromRole(roleId, permisoId);
                }
            }

            // Agregar permisos nuevos
            foreach (var permisoId in permisosIds)
            {
                if (!permisosActualesIds.Contains(permisoId))
                {
                    _repository.Rol_Permisos.Create(new Rol_Permiso
                    {
                        Id_Rol = roleId,
                        Id_Permiso = permisoId
                    });
                }
            }

            await _repository.SaveAsync();



        }

        public async Task UpdatePermissionsOfUser(string id, object permisos)
        {
            var permisosActusalesUser = await _repository.Permisos.GetUserPermisos(Guid.Parse(id), trackChanges: true);

            var permisosActualesIds = permisosActusalesUser.Select(p => p.Id_Permiso).ToList();
            foreach (var permisoId in permisosActualesIds)
            {
                if (!((List<int>)permisos).Contains(permisoId))
                {
                    await RemovePermissionFromUser(Guid.Parse(id), permisoId);
                }
            }

            foreach (var permisoId in (List<int>)permisos)
            {
                if (!permisosActualesIds.Contains(permisoId))
                {
                    _repository.UserPermisos.Create(new User_Permiso
                    {
                        UserId = id,
                        Id_Permiso = permisoId
                    });
                }
            }
            await _repository.SaveAsync();
        }
    }
}
