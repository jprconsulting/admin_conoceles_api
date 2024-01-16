namespace conoceles_api.Entities
{
    public class Estado
    {
        public int Id { get; set; }
        public string NombreEstado { get; set; }
        public List<Candidato> Candidatos { get; set; }
    }
}
