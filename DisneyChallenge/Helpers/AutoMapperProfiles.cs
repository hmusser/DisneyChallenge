using AutoMapper;
using DisneyChallenge.DTOs;
using DisneyChallenge.Entidades;
using Microsoft.AspNetCore.Identity;

namespace DisneyChallenge.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            /*Nota: La imagen en BD es una URL, pero la imagen que viene en los DTOs de creación son IFormFile.
            por ello se hace la excepción de ignorarlos en el mapeo*/

            //Usuarios
            CreateMap<IdentityUser, UsuarioDTO>();

            //Genero
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>()
                .ForMember( x => x.Imagen, options => options.Ignore())
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapGenerosPeliculas));

            //Personaje      
            CreateMap<Personaje, PersonajeDTO>()
               .ForMember(PersonajeDTO => PersonajeDTO.Peliculas, opciones => opciones.MapFrom(MapPersonajeDTOPeliculas));

            CreateMap<Personaje, PersonajesListadoDTO>();

            CreateMap<PersonajeCreacionDTO, Personaje>()
                .ForMember(x => x.Imagen, options => options.Ignore())
                .ForMember(x => x.PeliculasPersonajes, options => options.MapFrom(MapPersonajesPeliculas));

            //Pelicula
            
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(PeliculaDTO => PeliculaDTO.Personajes, opciones => opciones.MapFrom(MapPeliculaDTOPersonajes))
                .ForMember(PeliculaDTO => PeliculaDTO.Generos, opciones => opciones.MapFrom(MapPeliculaDTOGeneros));
            CreateMap<Pelicula, PeliculaListadoDTO>();
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.Imagen, options => options.Ignore())
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.PeliculasPersonajes, options => options.MapFrom(MapPeliculasPersonajes));

            
        }



        public List<GeneroListadoDTO> MapPeliculaDTOGeneros(Pelicula pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<GeneroListadoDTO>();

            if (pelicula.PeliculasGeneros == null)
            {
                return resultado;
            }

            foreach (var peliculaGenero in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroListadoDTO()
                {
                Nombre = peliculaGenero.Genero.Nombre
                });
            }
            return resultado;
        }
        public List<PersonajesListadoDTO> MapPeliculaDTOPersonajes(Pelicula pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<PersonajesListadoDTO>();

            if (pelicula.PeliculasPersonajes == null)
            {
                return resultado;
            }

            foreach (var peliculaPersonaje in pelicula.PeliculasPersonajes)
            {
                resultado.Add(new PersonajesListadoDTO()
                {
                    Imagen = peliculaPersonaje.Personaje.Imagen,
                    Nombre = peliculaPersonaje.Personaje.Nombre
                });
            }
            return resultado;
        }
        public List<PeliculaDTO> MapPersonajeDTOPeliculas(Personaje personaje, PersonajeDTO personajeDTO)
        {
            var resultado = new List<PeliculaDTO>();

            if(personaje.PeliculasPersonajes == null)
            {
                return resultado;
            }

            foreach(var peliculaPersonaje in personaje.PeliculasPersonajes)
            {
                resultado.Add(new PeliculaDTO()
                {
                    Titulo = peliculaPersonaje.Pelicula.Titulo,
                    FechaCreacion = peliculaPersonaje.Pelicula.FechaCreacion,
                    Imagen = peliculaPersonaje.Pelicula.Imagen
                });
            }
            return resultado;
        }
        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();

            //Si la pelicula vino sin ids de generos devuelvo una lista vacía.
            if (peliculaCreacionDTO.GenerosIds == null)
            {
                return resultado;
            }
            
            foreach (var id in peliculaCreacionDTO.GenerosIds)
            {
                resultado.Add(new PeliculasGeneros()
                {
                    GeneroId = id
                });
            }
            return resultado;
        }

        private List<PeliculasPersonajes> MapPeliculasPersonajes(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasPersonajes>();

            //Si la pelicula vino sin ids de personajes devuelvo una lista vacía.
            if (peliculaCreacionDTO.PersonajesIds == null)
            {
                return resultado;
            }
            
            foreach (var id in peliculaCreacionDTO.PersonajesIds)
            {
                resultado.Add(new PeliculasPersonajes()
                {
                    PersonajeId = id
                });
            }
            return resultado;
        }

        private List<PeliculasPersonajes> MapPersonajesPeliculas(PersonajeCreacionDTO personajeCreacionDTO, Personaje personaje)
        {
            var resultado = new List<PeliculasPersonajes>();

            //Si el personaje vino sin ids de peliculas devuelvo una lista vacía.
            if (personajeCreacionDTO.PeliculasIds == null)
            {
                return resultado;
            }

            foreach (var id in personajeCreacionDTO.PeliculasIds)
            {
                resultado.Add(new PeliculasPersonajes()
                {
                    PeliculaId = id
                }
                );
            }
            return resultado;
        }

        private List<PeliculasPersonajes> MapGenerosPeliculas(GeneroCreacionDTO generoCreacionDTO, Genero genero)
        {
            var resultado = new List<PeliculasPersonajes>();

            //Si el personaje vino sin ids de peliculas devuelvo una lista vacía.
            if (generoCreacionDTO.PeliculasIds == null)
            {
                return resultado;
            }

            foreach (var id in generoCreacionDTO.PeliculasIds)
            {
                resultado.Add(new PeliculasPersonajes()
                {
                    PeliculaId = id
                }
                );
            }
            return resultado;
        }

    }
}
