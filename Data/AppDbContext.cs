using api_netcore.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace api_netcore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        // set de entidades
        public DbSet<Empresa> Tbl_Empresa { get; set; }
        public DbSet<Empleado> Tbl_Empleado { get; set; }

        // configurar las entidades
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.ToTable("Tbl_Empresa");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("empresa_id");
                entity.Property(e => e.RutaImagen).HasMaxLength(255).IsRequired(false).HasColumnName("empresa_rutaimagen").HasDefaultValue("default.png");
                entity.Property(e => e.Ruc)
            .HasMaxLength(20)
            .IsRequired(false)
            .HasColumnName("empresa_ruc")
            .HasColumnType("nvarchar(20)");
                entity.Property(e => e.NombreDueno).HasMaxLength(50).IsRequired(false).HasColumnName("empresa_nombredueno");
                entity.Property(e => e.NombreEmpresa).HasMaxLength(50).IsRequired(false).HasColumnName("empresa_nombreempresa");
                entity.Property(e => e.Direccion).HasMaxLength(100).IsRequired(false).HasColumnName("empresa_direccion");
                entity.Property(e => e.Telefono).HasMaxLength(10).IsRequired(false).HasColumnName("empresa_telefono");
                entity.Property(e => e.Email).HasMaxLength(50).IsRequired(false).HasColumnName("empresa_email");
                entity.Property(e => e.FechaCreacion).HasColumnName("empresa_fechacreacion").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Estado).HasColumnName("empresa_estado").IsRequired(false).HasDefaultValue(1);
            });
            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("Tbl_Empleado");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("empleado_id");
                entity.Property(e => e.NombreEmpleado).HasMaxLength(50).IsRequired(false).HasColumnName("empleado_nombre");
                entity.Property(e => e.ApellidoEmpleado).HasMaxLength(50).IsRequired(false).HasColumnName("empleado_apellido");
                entity.Property(e => e.Direccion).HasMaxLength(100).IsRequired(false).HasColumnName("empleado_direccion");
                entity.Property(e => e.Telefono).HasMaxLength(10).IsRequired(false).HasColumnName("empleado_telefono");
                entity.Property(e => e.Correo).HasMaxLength(50).IsRequired(false).HasColumnName("empleado_correo");
                entity.Property(e => e.FechaNacimiento).HasColumnType("date").IsRequired(false).HasColumnName("empleado_fechanacimiento");
                entity.Property(e => e.FechaIngreso).IsRequired().HasColumnName("empleado_fechaingreso").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IdEmpresa).IsRequired(false).HasColumnName("empresa_id");
                entity.Property(e => e.Estado).HasColumnName("empleado_estado").HasDefaultValue(1);

            });

            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Empresa)
                .WithMany(e => e.Empleados)
                .HasForeignKey(e => e.IdEmpresa)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
