using HealthApp.Domain.Models;
using HealthApp.Razor.Data;

namespace HealthApp.Razor.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ScheduleAppointment(int patientId, int doctorId, DateTime appointmentDate)
        {
            bool isDoctorAvailable = !_context.Appointments.Any(a => a.DoctorId == doctorId && a.AppointmentDate == appointmentDate);
            if (!isDoctorAvailable)
            {
                return false;
            }

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDate = appointmentDate
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return true;
        }

    }

}
