using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Identity;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;

namespace HealthApp.Razor.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class DoctorDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DoctorDashboardModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;

        public async Task OnGetAsync(int pageNumber = 1)
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor != null)
            {
                int pageSize = 5;
                var query = _context.Appointments
                    .Include(p => p.Patient)
                    .Where(p => p.DoctorId == doctor.Id);

                Appointments = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
                CurrentPage = pageNumber;
            }
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

            if (appointment != null && !appointment.IsCanceled)
            {
                appointment.Status = "Approved";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

            if (appointment != null && !appointment.IsCanceled)
            {
                appointment.Status = "Rejected";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
