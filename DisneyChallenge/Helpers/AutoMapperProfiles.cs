using AutoMapper;
using DisneyChallenge.DTOs;
using DisneyChallenge.Entidades;

namespace DisneyChallenge.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //Genero
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();
            //Personaje
            CreateMap<Personaje, PersonajeDTO>().ReverseMap();
            CreateMap<PersonajeCreacionDTO, Personaje>();
        }
    }
}
