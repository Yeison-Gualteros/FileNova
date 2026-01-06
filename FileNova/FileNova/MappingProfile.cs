using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;


namespace FileNova
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Usuarios
            CreateMap<UserForRegistrationDto, User>();

            //Roles




            //Documentos 
            CreateMap<DocumentoForCreationDto, Documento>();
            CreateMap<Documento, DocumentoDTO>();

            CreateMap<DocumentoForUpdateDto, Documento>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Trazabilidad_Documento, Trazabilidad_DocumentoDTO>();
        }
    }
}
