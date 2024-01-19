using conoceles_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<AsignacionFormulario> AsignacionesFormulario { get; set; }
        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Estado> Estados { get; set; }

        internal Task FirstOrDefaultAsync(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Formulario> Formularios { get; set; }
        public DbSet<AgrupacionPolitica> AgrupacionesPoliticas { get; set; }
        public DbSet<PreguntaFormulario> PreguntasFormulario { get; set; }
        public DbSet<RespuestaPreguntaFormulario> RespuestasPreguntaFormulario { get; set; }
        public DbSet<TipoAgrupacionPolitica> TiposAgrupacionesPoliticas { get; set; }
        public DbSet<DistritoLocal> DistritosLocales { get; set; }
        public DbSet<Ayuntamiento> Ayuntamientos { get; set; }
        public DbSet<Comunidad> Comunidades { get; set; }

    }
}
