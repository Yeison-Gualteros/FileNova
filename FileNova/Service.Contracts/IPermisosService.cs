using Shared.DataTransferObjects.Permisos;

namespace Service.Contracts
{
    public interface IPermisosService
    {
        // =========================
        // CONSULTAS
        // =========================
        Task<IEnumerable<PermisosDto>> GetAllPermisos(int? id_Permiso, bool trackChanges );

        Task<IEnumerable<PermisosDto>> GetUserPermisos(Guid userId, bool trackChanges );

        Task<IEnumerable<PermisosDto>> GetPermissionsByRole( string roleId);
        Task<IEnumerable<PermisosDto>>  GetPermissionsByUser(string id);

        // =========================
        // CRUD
        // =========================
        Task<PermisosDto> CreatePermiso(PermisosForCreationDto permiso);

        Task<PermisosDto> GetPermisoById( int id_Permiso, bool trackChanges);

        Task<PermisosDto> UpdatePermiso( int id_Permiso, PermisoForUpdateDto permisoForUpdate);

        Task<bool> DeletePermiso( int id_Permiso);

        // =========================
        // ASIGNACIÓN USUARIO
        // =========================
        Task AddPermissionsToUser(Guid userId, List<int> permisosIds);

        Task RemovePermissionFromUser( Guid userId,int permisoId);

        // =========================
        // ASIGNACIÓN ROL
        // =========================
        Task AddPermissionsToRole(string roleId, List<int> permisosIds);

        Task RemovePermissionFromRole( string roleId, int permisoId );
        Task UpdatePermissionsOfRole(string roleId, List<int> permisosIds);
        Task UpdatePermissionsOfUser(string id, object permisos);
        
    }
}
