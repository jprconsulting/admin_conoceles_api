using conoceles_api.Entities;

namespace conoceles_api.DTOs
{
    public class ComunidadDTO
    {
        public int Id { get; set; }
        public string NombreComunidad { get; set; }
        public string Acronimo { get; set; }
        public string Peticion { get; set; }
        public bool Estatus { get; set; }
        public MunicipioDTO Municipio { get; set; }
    }
}
