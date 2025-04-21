using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HealthApp.Domain.Models;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using HealthApp.Razor.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace HealthApp.Razor.Pages.Patient
{
    [Authorize(Roles = "Patient")]
    public class ScheduleAppointmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ScheduleAppointmentModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment();

        public SelectList Doctors { get; set; } = default!;

        public void OnGet()
        {
            Doctors = new SelectList(_context.Doctors.ToList(), "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);

            var currentPatient = _context.Patients.FirstOrDefault(p => p.UserId == userId);

            if (currentPatient != null)
            {
                Appointment.PatientId = currentPatient.Id;
            }
            else
            {
                ModelState.AddModelError("", "Unable to find the logged-in patient's information.");
                return Page();
            }

            if (Appointment.DoctorId == 0)
            {
                ModelState.AddModelError("DoctorId", "Please select a doctor.");
                OnGet();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            var appointmentService = new AppointmentService(_context);
            bool isSuccess = await appointmentService.ScheduleAppointment(Appointment.PatientId, Appointment.DoctorId, Appointment.AppointmentDate);

            if (!isSuccess)
            {
                ModelState.AddModelError("", "The doctor is not available at the selected time.");
                return Page();
            }

            return RedirectToPage("Success");
        }
    }
}
