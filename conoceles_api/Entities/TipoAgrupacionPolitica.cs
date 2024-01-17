namespace conoceles_api.Entities
{
    public class TipoAgrupacionPolitica
    {
        public int Id { get; set; }
        public string TipoOrganizacion { get; set; }
        public List<AgrupacionPolitica> OrganizacionesPoliticas { get; set; }
    }
}
