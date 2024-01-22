namespace conoceles_api.Entities
{
    public class Comunidad
    {
        public int Id { get; set; }
        public string NombreComunidad { get; set; }
        public Municipio Municipio { get; set; }
    }
}
