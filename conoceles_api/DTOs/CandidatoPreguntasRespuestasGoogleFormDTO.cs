namespace conoceles_api.DTOs
{
    public class CandidatoPreguntasRespuestasGoogleFormDTO
    {
        public int CandidatoId { get; set; }
        public string NombreCompleto { get; set; } 
        public List<FormularioPreguntasRespuestasGoogleFormDTO> Formularios { get; set; }
    }
}
