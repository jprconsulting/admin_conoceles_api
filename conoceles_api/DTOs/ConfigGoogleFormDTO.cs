namespace conoceles_api.DTOs
{
    public class ConfigGoogleFormDTO
    {
        public int? Id { get; set; }
        public string GoogleFormId { get; set; }
        public string SpreadsheetId { get; set; }
        public string SheetName { get; set; }
        public string Type { get; set; }
        public string ProjectId { get; set; }
        public string PrivateKeyId { get; set; }
        public string PrivateKey { get; set; }
        public string ClientEmail { get; set; }
        public string ClientId { get; set; }
        public string AuthUri { get; set; }
        public string TokenUri { get; set; }
        public string AuthProviderX509CertUrl { get; set; }
        public string ClientX509CertUrl { get; set; }
        public string UniverseDomain { get; set; }
    }
}
