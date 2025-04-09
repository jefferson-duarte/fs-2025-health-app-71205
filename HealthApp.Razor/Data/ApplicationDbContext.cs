using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Razor.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<DoctorPatient> DoctorPatient { get; set; } = default!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Many-to-many relationship configuration
        builder.Entity<DoctorPatient>()
            .HasKey(dp => new { dp.DoctorId, dp.PatientId }); // Composite primary key

        builder.Entity<DoctorPatient>()
            .HasOne(dp => dp.Doctor)
            .WithMany()
            .HasForeignKey(dp => dp.DoctorId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

        builder.Entity<DoctorPatient>()
            .HasOne(dp => dp.Patient)
            .WithMany()
            .HasForeignKey(dp => dp.PatientId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
    }
}
