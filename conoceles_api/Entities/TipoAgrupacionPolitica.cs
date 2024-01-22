namespace conoceles_api.Entities
{
    public class TipoAgrupacionPolitica
    {
        public int Id { get; set; }
        public string TipoAgrupacion { get; set; }
        public List<AgrupacionPolitica> AgrupacionesPoliticas { get; set; }
    }
}
