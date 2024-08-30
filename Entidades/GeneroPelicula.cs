namespace AnimalApiPeliculas.Entidades {
    public class GeneroPelicula {
        public int PeliculaId;
        public int GeneroId;
        public Genero Genero { get; set; } = null!;
        public Pelicula Pelicula { get; set; } = null!;
    }
}
