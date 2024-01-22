namespace conoceles_api.DTOs
{
    public class FormularioDTO
    {
        public int? Id { get; set; }
        public string NombreFormulario { get; set; }
        public string GoogleFormId { get; set; }
        public string SpreadsheetId { get; set; }
        public string SheetName { get; set; }
        public string EndPointEditLinks { get; set; }
        public ConfigGoogleFormDTO ConfigGoogleForm { get; set; }
    }
}
