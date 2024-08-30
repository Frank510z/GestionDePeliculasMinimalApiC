using AnimalApiPeliculas.Endpoints;
using AnimalApiPeliculas.Entidades;
using AnimalApiPeliculas.Repositorios;
using AnimalApiPeliculas.Servicios;
using AnimalApiPeliculas.Swagger;
using AnimalApiPeliculas.Utilidades;
using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var origenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!;
//Inicio de area de los servicios

builder.Services.AddCors(opciones => {
    opciones.AddDefaultPolicy(configuracion => {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod(); // solo para localhost
    });

    opciones.AddPolicy("libre", configuracion => {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();  //para cualquier ruta (dominio)
    });
});

//builder.Services.AddOutputCache(); // Servicio de Cache

builder.Services.AddStackExchangeRedisOutputCache(opciones => {
    opciones.Configuration = builder.Configuration.GetConnectionString("redis");
});



builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Peliculas Api",
        Description = "Este es un web API de peliculas",
        Contact = new OpenApiContact {
            Email = "Francis.fjro@gmail.com",
            Name = "Francisco Javier",
            Url = new Uri("https://Ejemplo.com")
        },
        License = new OpenApiLicense {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")

        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {

        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.OperationFilter<FiltroAutorizacion>();

    /*
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
            }, new string[] { }

        }
    });
    
     */

});


builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositoriosComentarios, RepositoriosComentarios>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();
builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();



//kbuilder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosAzure>(); //Almacenado De Imagenes con Azure
builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>(); //Almacenado De Imagenes Local
builder.Services.AddHttpContextAccessor(); //Disponivilidad a servicio utilizado para almacenamiento Local


builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();


builder.Services.AddAutoMapper(typeof(Program)); //AutoMapper
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails(); // Modificar errores y expeciones

//Autenticacion servicios:
builder.Services.AddAuthentication().AddJwtBearer(opciones => {

    opciones.MapInboundClaims = false;

    opciones.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = false,  // Validar Iussuer
        ValidateAudience = false, // Validar Audencia
        ValidateLifetime = true, // Validar tiempo de vida del token
        ValidateIssuerSigningKey = true, // validar si esta firmado el token

        //IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First()
        IssuerSigningKeys = Llaves.ObtenerTodasLasLlaves(builder.Configuration),
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddAuthorization(opciones => {
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin")); // agregamos una politica llamada esadmin que requiere que el usuario tenga un claim llamda esadmin 
});

builder.Services.AddTransient<IUserStore<IdentityUser>, UsuarioStore>(); // User Store
builder.Services.AddIdentityCore<IdentityUser>(); // Servicios de Identity para que pueda usar IdentityUser
builder.Services.AddTransient<SignInManager<IdentityUser>>(); // Logea Usuarios

//Fin de area de los servicios

var app = builder.Build();

//Inicio de area de los middleware
//app.UseAntiforgery();
/*
 * app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context => {

    var exceptionHandlerFeacture = context.Features.Get<IExceptionHandlerFeature>();  //Esto nos permite traer la informacion del error 

    var excepcion = exceptionHandlerFeacture?.Error!;

    var error = new Error();
    error.Fecha = DateTime.UtcNow;
    error.MensajeDeError = excepcion.Message;
    error.StackTrace = excepcion.StackTrace;

    var repositorio = context.RequestServices.GetRequiredService<IRepositorioErrores>();
    await repositorio.Crear(error);

    await TypedResults.BadRequest(new { tipo = "error", mensaje = "Haocurrido un error", status = 500 }).ExecuteAsync(context);

})); // visualizar errores de pagina

*/
app.UseStatusCodePages(); //Modificar errores en pagina

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseOutputCache();

app.UseStaticFiles(); //Usuario puedan acceder a los archivos estaticos Localmente

app.UseAuthorization();

// Grupos de endpoints
app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();
app.MapGroup("/usuarios").MapUsuarios();



app.MapGet("/", [EnableCors(policyName: "libre")] () => "Hola mundo");

app.MapGet("/error", () => {

    throw new InvalidDataException("Eroro de ejemplo");

});

app.MapPost("modelBinding", ([FromHeader(Name = "Nombre2")] string? nombre) => {

    if (nombre is null) {
        return "vacio";
    }
    return nombre;

});

//Fin de area de los middleware

app.Run();
