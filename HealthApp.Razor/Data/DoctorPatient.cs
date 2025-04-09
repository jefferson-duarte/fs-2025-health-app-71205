using Microsoft.AspNetCore.Identity;

namespace HealthApp.Razor.Data
{
    public class DoctorPatient
    {
        public string DoctorId { get; set; }
        public IdentityUser Doctor { get; set; } // Navigation property

        public string PatientId { get; set; }
        public IdentityUser Patient { get; set; }
    }
}
