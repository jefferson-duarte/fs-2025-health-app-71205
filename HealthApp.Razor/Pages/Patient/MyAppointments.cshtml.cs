using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoctorModel = HealthApp.Domain.Models.Doctor;

namespace HealthApp.Razor.Pages.Patient
{
    [Authorize(Roles = "Patient")]
    public class MyAppointmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyAppointmentsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;
        public List<DoctorModel> Doctors { get; set; } = new List<DoctorModel>();
        public string SelectedDoctor { get; set; } = string.Empty;
        public string SelectedStatus { get; set; } = string.Empty;
        public DateTime? SelectedDate { get; set; }

        public async Task OnGetAsync(int pageNumber = 1, string doctorId = "", string status = "", string date = "")
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient != null)
            {
                int pageSize = 5;
                var query = _context.Appointments
                    .Include(a => a.Doctor)
                    .Where(a => a.PatientId == patient.Id);

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

                Appointments = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
                CurrentPage = pageNumber;

                Doctors = await _context.Doctors.ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment != null)
            {
                if ((appointment.AppointmentDate - DateTime.Now).TotalHours < 48)
                {
                    ViewData["CancelError"] = "Appointments can only be canceled up to 48 hours before the scheduled time.";
                    await OnGetAsync();

                    return Page();
                }

                appointment.IsCanceled = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }


    }
}
