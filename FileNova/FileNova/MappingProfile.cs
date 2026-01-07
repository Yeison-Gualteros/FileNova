using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Documentos;
using Shared.DataTransferObjects.Roles;


namespace FileNova
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ==========================
            // Usuarios
            // ==========================
            CreateMap<UserForRegistrationDto, User>();

            //CreateMap<User, UserDTO>();

            //CreateMap<UserForUpdateDto, User>()
            //.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ==========================
            // Roles (IdentityRole)
            // ==========================
            // Crear rol
            CreateMap<RolForCreationDto, IdentityRole>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()))
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()));




            // Actualizar rol
            CreateMap<RolForUpdateDto, IdentityRole>()
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name != null ? src.Name.ToUpper() : null))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DTO para enviar al cliente
            CreateMap<IdentityRole, RolDto>();

            // ==========================
            // Documentos
            // ==========================
            CreateMap<DocumentoForCreationDto, Documento>();
            CreateMap<Documento, DocumentoDTO>();

            CreateMap<DocumentoForUpdateDto, Documento>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Trazabilidad_Documento, Trazabilidad_DocumentoDTO>();
        }
    }
}
