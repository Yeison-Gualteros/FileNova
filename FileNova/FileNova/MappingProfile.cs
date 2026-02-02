using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Documentos;
using Shared.DataTransferObjects.Permisos;
using Shared.DataTransferObjects.Roles;
using Shared.DataTransferObjects.User;


namespace FileNova
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ==========================
            // Usuarios
            // ==========================
            CreateMap<UserForRegistrationDto, User>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => 1)); // Activo por defecto


            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
                .ForMember(dest => dest.Permisos, opt => opt.Ignore()) // lo llenamos en el service si aplica
                .ForMember(dest => dest.Rol, opt => opt.Ignore());     // lo llenamos en el service


            CreateMap<UserForUpdateDto, User>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) =>
                    {
                        if (srcMember == null)
                            return false;
                        if (srcMember is string str)
                            return !string.IsNullOrEmpty(str);
                        return true;
                    }));


            // ==========================
            // Roles 
            // ==========================
            // Crear rol
            CreateMap<RolForCreationDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()))
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()));

            // Actualizar rol
            CreateMap<RolForUpdateDto, Role>()
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name != null ? src.Name.ToUpper() : null))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DTO para enviar al cliente
            CreateMap<Role, RolDto>();

            // ==========================
            // Documentos
            // ==========================
            // Crear Documento
            CreateMap<DocumentoForCreationDto, Documento>();
            // Mostrar Documentos
            CreateMap<Documento, DocumentoDTO>();
            // Actualizar Documentos
            CreateMap<DocumentoForUpdateDto, Documento>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            // Trazabilidad del Documento
            CreateMap<Trazabilidad_Documento, Trazabilidad_DocumentoDTO>();

            // ==========================
            // Permisos 
            // ==========================
            // Mostrar Permisos
            CreateMap<Permiso, PermisosDto>();
            // Crear Permisos
            CreateMap<PermisosForCreationDto, Permiso>();
            // Actualizar Permisos
            CreateMap<PermisoForUpdateDto, Permiso>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            // Asignar Permisos a un rol
            CreateMap<Rol_Permiso, PermisosDto>()
                .ForMember(dest => dest.Id_Permiso, opt => opt.MapFrom(src => src.Permiso.Id_Permiso))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Permiso.Nombre));

        }
    }
}
