using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;

namespace conoceles_api.Utilities
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            // source, destination
            CreateMap<Estado, EstadoDTO>();
            CreateMap<EstadoDTO, Estado>();

            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol))
                .ForMember(dest => dest.Nombres, opt => opt.MapFrom(src => $"{src.Nombres} {src.ApellidoPaterno} {src.ApellidoMaterno}"));      
            CreateMap<UsuarioDTO, Usuario>();


            CreateMap<Rol, RolDTO>();
            CreateMap<RolDTO, Rol>();

            CreateMap<Genero, GeneroDTO>();
            CreateMap<GeneroDTO, Genero>();

            CreateMap<Cargo, CargoDTO>();
            CreateMap<CargoDTO, Cargo>();

            CreateMap<TipoOrganizacionPolitica, TipoOrganizacionPoliticaDTO>();
            CreateMap<TipoOrganizacionPoliticaDTO, TipoOrganizacionPolitica>();

            CreateMap<OrganizacionPolitica, OrganizacionPoliticaDTO>();
            CreateMap<OrganizacionPoliticaDTO, OrganizacionPolitica>();

            CreateMap<Municipio, MunicipioDTO>();
            CreateMap<MunicipioDTO, Municipio>();

            CreateMap<Candidato, CandidatoDTO>();
            CreateMap<CandidatoDTO, Candidato>();

            CreateMap<AsignacionFormulario, AsignacionFormularioDTO>();
            CreateMap<AsignacionFormularioDTO, AsignacionFormulario>();

            CreateMap<ConfigGoogleForm, ConfigGoogleFormDTO>();
            CreateMap<ConfigGoogleFormDTO, ConfigGoogleForm>();

            CreateMap<Formulario, FormularioDTO>();
            CreateMap<FormularioDTO, Formulario>();

            CreateMap<PreguntaFormulario, PreguntaFormularioDTO>();
            CreateMap<PreguntaFormularioDTO, PreguntaFormulario>();

            CreateMap<RespuestaPreguntaFormularioDTO, RespuestaPreguntaFormulario>();
            CreateMap<RespuestaPreguntaFormulario, RespuestaPreguntaFormularioDTO>();

            CreateMap<Claim, ClaimDTO>();
            CreateMap<ClaimDTO, Claim>();

        }
    }
}
