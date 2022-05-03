using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DisneyChallenge.DTOs;
using DisneyChallenge.Servicios;
using DisneyChallenge.Helpers;

namespace DisneyChallenge.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class CuentasController : CustomBaseCrontoller
    {
        //Es el servicio que me permite registrar un usuario,
        //<IdentityUser> es la clase que identifica a un usuario y debo pasarle por parametro a UserManager
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        //Con este campo puedo acceder a la info que tengo alojada en apssetings
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IMailService mailService;

        public CuentasController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IMapper mapper,
            IMailService mailService) : base(context, mapper)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            this.context = context;
            this.mapper = mapper;
            this.mailService = mailService;
        }

        [HttpPost("register")]
        //UserToken será la respuesta al registro que realiza (el token y la fecha de expiración).
        //User Info contiene las credenciales con las que pretendo crear el nuevo usuario.
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
            //<IdentityUser> es la clase que identifica a un usuario y debo pasarle por parametro a UserManager
            //UserManager es el servicio que me permite registrar un usuario. 
            var user = new IdentityUser {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                EnviarEmailBienvenida(
                                        user.Email, 
                                        "Bienvenido a Disney Challenge", 
                                        "Gracias por registrarse en el sitio web de Disney Challenge \n No olvide su contraseña: "
                                        + model.Password+ "\nNos vemos pronto!"
                                        );
                return await ConstruirToken(model);//Envio "model" que son las credenciales del usuario.
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Login")]
        //UserToken será la respuesta al login que realiza (el token y la fecha de expiración).
        //User Info contiene las credenciales con las que pretendo identificar al nuevo usuario.
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo model)
        {
            //SignInManager es el servicio que me permite realizar el Login y al cual debo pasarle el IdentityUser.
            var resultado = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false, //Logueo mediante cookies.
                lockoutOnFailure: false //Bloqueo de usuario ante un logueo fallido.
                );

            if (resultado.Succeeded)
            {
                //Si la identificación fue exitosa, construimos el token para el usuario. 
                return await ConstruirToken(model);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpPost("RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> Renovar()
        {
            var userInfo = new UserInfo
            {
                Email = HttpContext.User.Identity.Name
            };

            return await ConstruirToken(userInfo);
        }

        private async Task<UserToken> ConstruirToken(UserInfo userInfo)
        //Nota: Para que el web api pueda validar que nuestrojwt es correcto, debe configurarse el servicio en la clase Startup
        //en services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        {
            //Creo un listado de Claims del usuario, éstos son información del usuario en la cual yo puedo confiar.
            //En ellos no debe colocarse información sensible como password o tarjetas de crédito.
            //Un Claim es un campo del tipo llave-valor.
            //Los Claims se añaden al token del usuario para que teniendo el token yo pueda leer esta informacion.
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim(ClaimTypes.Email, userInfo.Email),
            };
            //Traigo el usuario
            var identityUser = await _userManager.FindByEmailAsync(userInfo.Email);

            claims.Add(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));
            //Traigo desde la base los claims que tenga registrado ese usuario.
            var claimsDB = await _userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsDB);

            //Construyo el Jason Web Token

            //Preparo la firma para mi jwt, la clave secreta está alojada en appsetings.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //Tiempo de caducidad de nuestro token.
            var expiracion = DateTime.UtcNow.AddYears(1);

            //Armamos el token
            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims, // Agregamos las cliams.
               expires: expiracion,// Agregamos la expiración.
               signingCredentials: creds // Agregamos la firma para el jwt.
               );
            
            //Devuelvo la respuesta.
            return new UserToken()
            {
                //String que representa al token del usuario
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                //Fecha de expiración.
                Expiracion = expiracion
            };

        }
        
        //Devuelve el listado de usuarios registrados
        [HttpGet("Usuarios")]
        //Notar que la politica de autorizacion me permite acceder a este endpoint sólo si mi usuario es "Admin"
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<UsuarioDTO>>> Get([FromQuery] PaginacionDTO paginationDTO)
        {
            return await Get<IdentityUser, UsuarioDTO>(paginationDTO);
        }

        [HttpGet("Roles")]
        //Notar que la politica de autorizacion me permite acceder a este endpoint sólo si mi usuario es "Admin"
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            return await context.Roles.Select(x => x.Name).ToListAsync();
        }

        [HttpPost("AsignarRol")]
        //Notar que la politica de autorizacion me permite acceder a este endpoint sólo si mi usuario es "Admin"
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> AsignarRol(EditarRolDTO editarRolDTO)
        {
            var user = await _userManager.FindByIdAsync(editarRolDTO.UsuarioId);
            if (user == null)
            {
                return NotFound();
            }
            //new Claim(tipoDeClaim, valorDelClaim)
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editarRolDTO.NombreRol));
            return NoContent();
        }

        [HttpPost("RemoveRol")]
        //Notar que la politica de autorizacion me permite acceder a este endpoint sólo si mi usuario es "Admin"
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> RemoverRol(EditarRolDTO editarRolDTO)
        {
            var user = await _userManager.FindByIdAsync(editarRolDTO.UsuarioId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editarRolDTO.NombreRol));
            return NoContent();
        }

        public async void EnviarEmailBienvenida(string to, string sub, string bod)
        {
            try
            {
                await mailService.SendEmailAsync(
                    new MailRequest()
                    {
                        ToEmail = to,
                        Subject = sub,
                        Body = bod
                    });       
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



    }
}
