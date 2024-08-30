namespace AnimalApiPeliculas.DTOs {
    public class ActorDTO {
        public int id { get; set; }
        public string Nombre { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; }
    }
}