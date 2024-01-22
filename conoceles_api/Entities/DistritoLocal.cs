namespace conoceles_api.Entities
{
    public class DistritoLocal
    {
        public int Id { get; set; }
        public string NombreDistritoLocal { get; set; }
        public Estado Estado { get; set; }
        public List<Municipio> Municipios { get; set; }

    }
}
