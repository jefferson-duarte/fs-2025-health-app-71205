using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Razor.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Appointment> Appointments { get; set; } = default!;
    public DbSet<Doctor> Doctors { get; set; } = default!;
    public DbSet<Patient> Patients { get; set; } = default!;
    public DbSet<Prescription> Prescriptions { get; set; } = default!;


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
