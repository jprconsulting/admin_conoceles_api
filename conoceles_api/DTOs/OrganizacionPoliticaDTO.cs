namespace conoceles_api.DTOs
{
    public class OrganizacionPoliticaDTO
    {
        public int? Id { get; set; }
        public string NombreOrganizacion { get; set; }
        public string Logo { get; set; }
        public bool Estatus { get; set; }
        public TipoOrganizacionPoliticaDTO TipoOrganizacionPolitica { get; set; }
    }
}
