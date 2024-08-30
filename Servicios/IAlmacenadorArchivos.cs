namespace AnimalApiPeliculas.Servicios {
    public interface IAlmacenadorArchivos {

        Task Borrar(string? ruta, string contenedor); //contenedor es la carpeta a almacenar

        Task<string> Almacenar(string contenedor, IFormFile archivo); // regresara la URL de donde esta el archivo

        async Task<string> Editar(string? ruta, string contenedor, IFormFile archivo) { //hace ambas borra y almacena la nueva foto
            await Borrar(ruta, contenedor);
            return await Almacenar(contenedor, archivo);
        }

    }
}
