namespace conoceles_api.Entities
{
    public class Cargo
    {
        public int Id { get; set; }
        public string NombreCargo { get; set; }
        public List<Candidato> Candidatos { get; set; }
    }
}
