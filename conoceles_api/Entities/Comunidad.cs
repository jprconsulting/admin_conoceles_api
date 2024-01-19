namespace conoceles_api.Entities
{
    public class Comunidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Acronimo { get; set; }
        public string Peticion { get; set; }
        public bool Estatus { get; set; }
        public Ayuntamiento Ayuntamiento { get; set; }
    }
}
