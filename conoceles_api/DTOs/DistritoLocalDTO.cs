using conoceles_api.Entities;

namespace conoceles_api.DTOs
{
    public class DistritoLocalDTO
    {
        public int Id { get; set; }
        public string NombreDistritoLocal { get; set; }
        public EstadoDTO Estado { get; set; }
    }
}
