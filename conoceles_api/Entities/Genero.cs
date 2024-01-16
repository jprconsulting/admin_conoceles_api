namespace conoceles_api.Entities
{
    public class Genero
    {
        public int Id { get; set; }
        public string NombreGenero { get; set; }
        public List<Candidato> Candidatos { get; set; }
    }
}
