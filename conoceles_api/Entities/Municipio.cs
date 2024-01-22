namespace conoceles_api.Entities
{
    public class Municipio
    {
        public int Id { get; set; }
        public string NombreMunicipio { get; set; }
        public DistritoLocal DistritoLocal { get; set; }
        public List<Comunidad> Comunidades { get; set; }
    }
}
