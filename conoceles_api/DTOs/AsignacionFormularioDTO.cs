using System.ComponentModel.DataAnnotations;

namespace conoceles_api.DTOs
{
    public class AsignacionFormularioDTO
    {
        public int? Id { get; set; }
        public string EditLink { get; set; }
        public bool Estatus { get; set; }
        public DateTime FechaHoraAsignacion { get; set; }
        public string StrFechaHoraAsignacion { get; set; }
        [Required]
        public FormularioDTO Formulario { get; set; }
        public CandidatoDTO Candidato { get; set; }
        public List<int> CandidatosIds { get; set; }
    }
}
