namespace conoceles_api.Entities
{
    public class DistritoLocal
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Acronimo { get; set; }
        public string Peticion { get; set; }
        public bool Estatus { get; set; }
        public Estado Estado { get; set; }
        public List<Ayuntamiento> Ayuntamientos { get; set; }

    }
}
