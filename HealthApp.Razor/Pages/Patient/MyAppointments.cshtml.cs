using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Identity;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient != null)
            {
                Appointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Where(a => a.PatientId == patient.Id)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

            if (appointment != null)
            {
                appointment.IsCanceled = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
