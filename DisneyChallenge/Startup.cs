using DisneyChallenge.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DisneyChallenge
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            //Servicio para el envío de correos.
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, Servicios.MailService>();

            //Servicio para poder alojar imagenes localmente en nuestro servidor.
            services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
            services.AddHttpContextAccessor();

            services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();            
            services.AddEndpointsApiExplorer();

            //Agregamos el servicio de Identity para el manejo de usuarios.
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    //Parámetros de validación del token
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true, //Valida la expiración del token
                    ValidateIssuerSigningKey = true,//Valida la firma del token.
                    //Configuramos la firma.
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                    ClockSkew = TimeSpan.Zero
                });
            
            services.AddSwaggerGen();
            

            //Para activar la autorizacion basada en Claims.
            //Me permitirá manejar distintos niveles de autorizacion ( Por ej. usuario administrador y usuario normal
            services.AddAuthorization(opciones =>
            {
                //Si el usuario tiene un Claim llamado "esAdmin" se entenderá en mi web api que es un Administrador.
                opciones.AddPolicy("Admin", politica => politica.RequireClaim("esAdmin"));
                //opciones.AddPolicy("Vendedor", politica => politica.RequireClaim("esVendedor"));
                //opciones.AddPolicy("Proveedor", politica => politica.RequireClaim("esProveedor"));
                //...
            });

            //Servicio de creacion de hashes.
            services.AddTransient<HashService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //Para poder mostrar contenido estático y podamos ver en el browser las imagenes de nuestra web api localmente.
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

    }
}
