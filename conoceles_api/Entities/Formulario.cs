namespace conoceles_api.Entities
{
    public class Formulario
    {
        public int Id { get; set; }
        public string NombreFormulario { get; set; }
        public string GoogleFormId { get; set; }
        public string SpreadsheetId { get; set; }
        public string SheetName { get; set; }
        public string EndPointEditLinks { get; set; }
        public bool EstadoOperativo { get; set; }
        public int ConfigGoogleFormId { get; set; }
        public ConfigGoogleForm ConfigGoogleForm { get; set; } = null!;
        public List<AsignacionFormulario> AsignacionesFormulario { get; set; }
        public List<PreguntaFormulario> PreguntasFormulario { get; set; }
    }
}
