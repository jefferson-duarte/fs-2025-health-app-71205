using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HealthApp.Razor.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class MyPrescriptionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyPrescriptionsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor != null)
            {
                Prescriptions = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Where(p => p.DoctorId == doctor.Id)
                    .ToListAsync();
            }
        }
    }
}
