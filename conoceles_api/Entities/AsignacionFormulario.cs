namespace conoceles_api.Entities
{
    public class AsignacionFormulario
    {
        public int Id { get; set; }
        public string EditLink { get; set; }
        public bool Estatus { get; set; }
        public DateTime FechaHoraAsignacion { get; set; }
        public Formulario Formulario { get; set; }
        public Candidato Candidato { get; set; }
        public List<RespuestaPreguntaFormulario> RespuestasPreguntasFormulario { get; set; }
    }
}
