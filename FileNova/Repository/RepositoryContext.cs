using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;

namespace Repository
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1️⃣ Importante: Identity necesita llamar base
            base.OnModelCreating(modelBuilder);

            // 2️⃣ Mapear la entidad Usuario a la tabla "Usuarios"
            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("User"); // Cambia AspNetUsers a Usuarios
            });

            // 3️⃣ Relaciones Many-to-Many: Rol - Permiso
            modelBuilder.Entity<Rol_Permiso>()
                .HasKey(rp => new { rp.Id_Rol, rp.Id_Permiso });

            modelBuilder.Entity<Rol_Permiso>()
                .HasOne(rp => rp.Permiso)
                .WithMany(p => p.Rol_Permiso)
                .HasForeignKey(rp => rp.Id_Permiso);

            // 4️⃣ Usuario - Documentos
            modelBuilder.Entity<Documento>()
                .HasOne(d => d.User)
                .WithMany(u => u.Documentos)
                .HasForeignKey(d => d.Id_Usuario)
                .OnDelete(DeleteBehavior.Restrict);

            // 5️⃣ Documento - Trazabilidad
            modelBuilder.Entity<Trazabilidad_Documento>()
                .HasOne(t => t.Documento)
                .WithMany(d => d.Trazabilidad_Documentos)
                .HasForeignKey(t => t.Id_Documento)
                .OnDelete(DeleteBehavior.Cascade);

            // 6️⃣ Usuario - Trazabilidad
            modelBuilder.Entity<Trazabilidad_Documento>()
                .HasOne(t => t.User)
                .WithMany(u => u.Trazabilidad_Documentos)
                .HasForeignKey(t => t.Id_Usuario)
                .OnDelete(DeleteBehavior.Restrict);

            // 7️⃣ Usuario - Solicitud
            modelBuilder.Entity<Solicitud>()
                .HasOne(s => s.User)
                .WithMany(u => u.Solicitudes)
                .HasForeignKey(s => s.Id_Usuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryContext).Assembly);
        }

        // 9️⃣ DbSets
        
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Trazabilidad_Documento> Trazabilidad_Documentos { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Rol_Permiso> Rol_Permisos { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
    }
}
