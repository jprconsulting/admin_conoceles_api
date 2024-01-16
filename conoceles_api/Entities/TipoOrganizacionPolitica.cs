namespace conoceles_api.Entities
{
    public class TipoOrganizacionPolitica
    {
        public int Id { get; set; }
        public string TipoOrganizacion { get; set; }
        public List<OrganizacionPolitica> OrganizacionesPoliticas { get; set; }
    }
}
