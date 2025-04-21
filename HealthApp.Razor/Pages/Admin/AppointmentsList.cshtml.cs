using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

        public async Task OnGetAsync()
        {
            Appointments = await _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
