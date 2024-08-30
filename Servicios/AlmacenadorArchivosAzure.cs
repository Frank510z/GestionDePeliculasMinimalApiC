
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AnimalApiPeliculas.Servicios {
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos {
        private readonly string? connectionString;

        // Conectarse A Azure desde el AzureStorage String (de appsetting.json )
        public AlmacenadorArchivosAzure(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("AzureStorage")!;
        }


        // Guardar Archivo en Azure
        public async Task<string> Almacenar(string contenedor, IFormFile archivo) {
            var cliente = new BlobContainerClient(connectionString,contenedor);
            await cliente.CreateIfNotExistsAsync(); //Crear carpeta si no existe
            cliente.SetAccessPolicy(PublicAccessType.Blob);
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}"; // se crea un nombre aleatorio con extencion obtenida de extencion
            var blob = cliente.GetBlobClient(nombreArchivo);

            //Para darle un tipo al archivo para que se pueda abrir en el navegador:
            var blobHttpHeaders = new BlobHttpHeaders(); 
            blobHttpHeaders.ContentType = archivo.ContentType;
            await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeaders);
            return blob.Uri.ToString(); // Retorna el url de donde esta almacenada la imagen

        }


        //Eliminar Archivo De Azure
        public async Task Borrar(string? ruta, string contenedor) {
            if (string.IsNullOrEmpty(ruta)) {
                return;
            }
            var cliente = new BlobContainerClient(connectionString,contenedor);
            await cliente.CreateIfNotExistsAsync();
            var nombreArchivo = Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(nombreArchivo);
            await blob.DeleteIfExistsAsync();
        }

    }
}
