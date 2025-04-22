using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using DoctorModel = HealthApp.Domain.Models.Doctor;

namespace HealthApp.Razor.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AppointmentsListModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<DoctorModel> Doctors { get; set; } = new List<DoctorModel>();
        public string SelectedDoctor { get; set; } = string.Empty;
        public string SelectedStatus { get; set; } = string.Empty;
        public DateTime? SelectedDate { get; set; }

        public async Task OnGetAsync(string doctorId = "", string status = "", string date = "")
        {
            var query = _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient).AsQueryable();

            if (!string.IsNullOrEmpty(doctorId))
            {
                query = query.Where(a => a.Doctor.UserId == doctorId);
                SelectedDoctor = doctorId;
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
                SelectedStatus = status;
            }

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime parsedDate))
            {
                query = query.Where(a => a.AppointmentDate.Date == parsedDate.Date);
                SelectedDate = parsedDate;
            }

            Appointments = await query.ToListAsync();
            Doctors = await _context.Doctors.ToListAsync();
        }
    }
}
