using AutoMapper;
using DisneyChallenge.DTOs;
using DisneyChallenge.Entidades;
using DisneyChallenge.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisneyChallenge.Controllers
{
    [ApiController]
    [Route("api/genres")]
    [Authorize(AuthenticationSchemes =  JwtBearerDefaults.AuthenticationScheme)]
    public class GeneroController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "generos";

        public GeneroController(ApplicationDbContext context, 
                                IMapper mapper,
                                IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            var entidades = await context.Generos.ToListAsync();
            var dtos = mapper.Map<List<GeneroDTO>>(entidades);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>>Get(int id)
        {
            var entidad = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if(entidad == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<GeneroDTO>(entidad);
            return dto;
        }


        [HttpPost]
        //Utilizamos [FromForm] para poder recibir el archivo de imagen del Género a través de form-data desde Postman.
        public async Task<ActionResult>Post([FromForm] GeneroCreacionDTO generoCreacionDTO)
        {
            var entidad = mapper.Map<Genero>(generoCreacionDTO);

            if(generoCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await generoCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();//arreglo de bytes.
                    var extension = Path.GetExtension(generoCreacionDTO.Imagen.FileName);
                    entidad.Imagen = await almacenadorArchivos.GuardarArchivo(contenido,
                                                                              extension,
                                                                              contenedor,
                                                                              generoCreacionDTO.Imagen.ContentType);
                }

            }
            context.Add(entidad);
            await context.SaveChangesAsync();

            var generoDTO = mapper.Map<GeneroDTO>(entidad);
            return new CreatedAtRouteResult("obtenerGenero", new {id = entidad.Id}, generoDTO);
        }

        [HttpPut("{id:int}")]
        //Utilizamos [FromForm] para poder recibir el archivo de imagen del Género a través de form-data desde Postman.
        public async Task<ActionResult> Put(int id, [FromForm] GeneroCreacionDTO generoCreacionDTO)
        {
            var generoDB = await context.Generos
                .Include(x => x.PeliculasGeneros) //Incluyo las peliculas donde se asignó el genero.
                .FirstOrDefaultAsync(x => x.Id == id);

            if(generoDB == null)
            {
                return NotFound();//404
            }

            //Lo siguiente lo que hará es mapear lo que traigo de generoCreacionDTO en generoDB.
            //es decir, los campos que son distintos van a ser actualizados.
            generoDB = mapper.Map(generoCreacionDTO, generoDB);

            if (generoCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await generoCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();//arreglo de bytes.
                    var extension = Path.GetExtension(generoCreacionDTO.Imagen.FileName);
                    generoDB.Imagen = await almacenadorArchivos.EditarArchivo(contenido,
                                                                              extension,
                                                                              contenedor,
                                                                              generoDB.Imagen,
                                                                              generoCreacionDTO.Imagen.ContentType);
                }

            }
            /*SaveChangesAsync() por la forma en que funciona EF solo guardará en la BD aquellos campos
            que sean diferentes entre generoCreacionDTO y generoDB.*/
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult>Delete(int id)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Genero() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }


    }
}
