namespace HealthApp.Domain.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; } = default!;

        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; } = default!;

        public string Medication { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public DateTime DateIssued { get; set; } = DateTime.Now;
    }
}
