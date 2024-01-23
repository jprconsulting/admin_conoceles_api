using conoceles_api.Entities;

namespace conoceles_api.DTOs
{
    public class MunicipioDTO
    {
        public int Id { get; set; }
        public string NombreMunicipio { get; set; }
        public DistritoLocalDTO DistritoLocal { get; set; }
    }
}
