using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace HealthApp.Razor.Data
{
    public static class HealthAppRoles
    {
        public const string Admin = "Admin";
        public const string Doctor = "Doctor";
        public const string Patient = "Patient";
    }
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new string[] { HealthAppRoles.Patient, HealthAppRoles.Doctor, HealthAppRoles.Admin };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        Console.WriteLine($"Creating role {role}");
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                string doctor = "doctor@healthapp.com";
                var userDoctor = await userManager.FindByEmailAsync(doctor);

                if (userDoctor == null)
                {
                    var newDoctor = new IdentityUser
                    {
                        UserName = doctor,
                        Email = doctor,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(newDoctor, "Letmein01*");
                    await userManager.AddToRoleAsync(newDoctor, HealthAppRoles.Doctor);

                    userDoctor = newDoctor;
                }

                if (!context.Doctors.Any(d => d.UserId == userDoctor.Id))
                {
                    var doctorEntity = new Doctor
                    {
                        Name = "Dr. Carlos Pereira",
                        UserId = userDoctor.Id
                    };

                    context.Doctors.Add(doctorEntity);
                    await context.SaveChangesAsync();
                }

                string patientEmail = "patient@healthapp.com";
                var userPatient = await userManager.FindByEmailAsync(patientEmail);

                if (userPatient == null)
                {
                    var newPatient = new IdentityUser
                    {
                        UserName = patientEmail,
                        Email = patientEmail,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(newPatient, "Letmein01*");
                    await userManager.AddToRoleAsync(newPatient, HealthAppRoles.Patient);

                    userPatient = newPatient;
                }

                if (!context.Patients.Any(p => p.UserId == userPatient.Id))
                {
                    var patientEntity = new Patient
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Email = userPatient.Email,
                        UserId = userPatient.Id
                    };

                    context.Patients.Add(patientEntity);
                    await context.SaveChangesAsync();
                }



                if (!context.DoctorPatient.Any(dp => dp.DoctorId == userDoctor.Id && dp.PatientId == userPatient.Id))
                {
                    var doctorPatient = new DoctorPatient
                    {
                        DoctorId = userDoctor.Id,
                        PatientId = userPatient.Id
                    };

                    context.DoctorPatient.Add(doctorPatient);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
