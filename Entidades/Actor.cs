namespace AnimalApiPeliculas.Entidades {
    public class Actor {
        public int Id { get; set; } // id minuscula
        public string Nombre { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; }
    }
}