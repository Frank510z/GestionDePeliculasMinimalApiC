
namespace AnimalApiPeliculas.Servicios {
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) { //Obtener la direccion de la carpeta donde yo quiero obtener las imgenes, y el acceso
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Almacenar(string contenedor, IFormFile archivo) {

            var extencion = Path.GetExtension(archivo.FileName); //Obtiene la extencion del archivo
            var nombreArchivo = $"{Guid.NewGuid()}{extencion}"; //Le agrega un nombre unico al archivo
            string folder = Path.Combine(env.WebRootPath, contenedor); //contenedor es

            if (!Directory.Exists(folder)) {  // si el folder no existe lo crea         
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo); //combinamos folder con el nombre del archivo
            using (var ms = new MemoryStream()) {
                await archivo.CopyToAsync(ms);
                var contenido = ms.ToArray();
                await File.WriteAllBytesAsync(ruta, contenido);
            }
            var url = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var urlArchivo = Path.Combine(url, contenedor, nombreArchivo).Replace("\\", "/");
            return urlArchivo;
        }

        public Task Borrar(string? ruta, string contenedor) {

            if (string.IsNullOrEmpty(ruta)) {
                return Task.CompletedTask;
            }

            var nombreArchivo = Path.GetFileName(ruta);
            var directorioArchivo = Path.Combine(env.WebRootPath, contenedor, nombreArchivo);

            if (File.Exists(directorioArchivo)) {
                File.Delete(directorioArchivo);
            }
            return Task.CompletedTask;
        }
    }
}
