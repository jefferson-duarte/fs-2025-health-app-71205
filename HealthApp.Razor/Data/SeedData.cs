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

                string adminUserName = "admin@healthapp.com";
                string adminUserEmail = "admin@healthapp.com";
                string genericPassword = "Letmein01*";

                if (await userManager.FindByEmailAsync(adminUserEmail) == null)
                {

                    var user = new IdentityUser { UserName = adminUserName, Email = adminUserEmail, EmailConfirmed = true };
                    await userManager.CreateAsync(user, genericPassword);
                    await userManager.AddToRoleAsync(user, HealthAppRoles.Admin);
                }

                string doctor = "doctor01@healthapp.com";
                if (await userManager.FindByEmailAsync(doctor) == null)
                {

                    var userDoctor = new IdentityUser { UserName = doctor, Email = doctor, EmailConfirmed = true };
                    await userManager.CreateAsync(userDoctor, genericPassword);
                    await userManager.AddToRoleAsync(userDoctor, HealthAppRoles.Doctor);



                    string patient = "patient01@healthapp.com";
                    if (await userManager.FindByEmailAsync(patient) == null)
                    {

                        var userPatient = new IdentityUser { UserName = patient, Email = patient, EmailConfirmed = true };
                        await userManager.CreateAsync(userPatient, genericPassword);
                        await userManager.AddToRoleAsync(userPatient, HealthAppRoles.Patient);


                        var doctorPatient = new DoctorPatient
                        {
                            DoctorId = userDoctor.Id,
                            PatientId = userPatient.Id
                        };

                        context.DoctorPatient.Add(doctorPatient);
                        await context.SaveChangesAsync();
                    }
                }

                string doctor2 = "doctor02@healthapp.com";
                if (await userManager.FindByEmailAsync(doctor2) == null)
                {

                    var userDoctor = new IdentityUser { UserName = doctor2, Email = doctor2, EmailConfirmed = true };
                    await userManager.CreateAsync(userDoctor, genericPassword);
                    await userManager.AddToRoleAsync(userDoctor, HealthAppRoles.Doctor);



                    string patient2 = "patient02@healthapp.com";
                    if (await userManager.FindByEmailAsync(patient2) == null)
                    {

                        var userPatient = new IdentityUser { UserName = patient2, Email = patient2, EmailConfirmed = true };
                        await userManager.CreateAsync(userPatient, genericPassword);
                        await userManager.AddToRoleAsync(userPatient, HealthAppRoles.Patient);


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
}
