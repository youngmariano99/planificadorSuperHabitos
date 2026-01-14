using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Planificador.Domain.Entities;

namespace Planificador.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext // Heredamos de Identity para Login/Registro
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AnnualPlan> AnnualPlans { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Necesario para Identity

        builder.Entity<AnnualPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            // Asegura que un usuario solo tenga un plan por año
            entity.HasIndex(e => new { e.UserId, e.Year }).IsUnique(); 
        });
    }
}