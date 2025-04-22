using Bogus;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

                string genericPassword = "Letmein01*";

                int existingDoctorsCount = await context.Doctors.CountAsync();

                if (existingDoctorsCount < 5)
                {
                    var faker = new Faker();

                    for (int i = existingDoctorsCount; i < 5; i++)
                    {
                        string firstName = faker.Name.FirstName();
                        string lastName = faker.Name.LastName();
                        string doctorEmail = $"{firstName.ToLower()}.{lastName.ToLower()}@healthapp.com";

                        var existingUser = await userManager.FindByEmailAsync(doctorEmail);
                        if (existingUser == null)
                        {
                            var newDoctorUser = new IdentityUser
                            {
                                UserName = doctorEmail,
                                Email = doctorEmail,
                                EmailConfirmed = true
                            };

                            var result = await userManager.CreateAsync(newDoctorUser, genericPassword);
                            if (result.Succeeded)
                            {
                                await userManager.AddToRoleAsync(newDoctorUser, HealthAppRoles.Doctor);

                                var doctorEntity = new Doctor
                                {
                                    Name = $"Dr. {firstName} {lastName}",
                                    UserId = newDoctorUser.Id
                                };

                                context.Doctors.Add(doctorEntity);
                            }
                        }
                    }

                    await context.SaveChangesAsync();
                }


                int existingPatientsCount = await context.Patients.CountAsync();
                if (existingPatientsCount < 1000)
                {
                    var fakerPatient = new Faker();
                    for (int i = existingPatientsCount; i < 1000; i++)
                    {
                        string firstName = fakerPatient.Name.FirstName();
                        string lastName = fakerPatient.Name.LastName();
                        string patientEmail = $"{firstName.ToLower()}.{lastName.ToLower()}@healthapp.com";

                        var existingUser = await userManager.FindByEmailAsync(patientEmail);
                        if (existingUser == null)
                        {
                            var newPatientUser = new IdentityUser
                            {
                                UserName = patientEmail,
                                Email = patientEmail,
                                EmailConfirmed = true
                            };

                            var result = await userManager.CreateAsync(newPatientUser, genericPassword);
                            if (result.Succeeded)
                            {
                                await userManager.AddToRoleAsync(newPatientUser, HealthAppRoles.Patient);

                                var patientEntity = new Patient
                                {
                                    FirstName = firstName,
                                    LastName = lastName,
                                    Email = patientEmail,
                                    UserId = newPatientUser.Id
                                };

                                context.Patients.Add(patientEntity);
                            }
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
