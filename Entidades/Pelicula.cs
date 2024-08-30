namespace AnimalApiPeliculas.Entidades {
    public class Pelicula {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaDeLanzamiento { get; set; }
        public string? Poster { get; set; }

        // Lista de comentarios de la tabla comenntarios 
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<GeneroPelicula> GenerosPeliculas { get; set; } = new List<GeneroPelicula>();
        public List<ActorPelicula> ActoresPeliculas { get; set; } = new List<ActorPelicula>();
    }
}
