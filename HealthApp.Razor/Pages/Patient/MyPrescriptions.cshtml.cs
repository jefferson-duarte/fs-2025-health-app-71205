using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HealthApp.Razor.Pages.Patient
{
    [Authorize(Roles = "Patient")]
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
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient != null)
            {
                Prescriptions = await _context.Prescriptions
                    .Include(p => p.Doctor)
                    .Where(p => p.PatientId == patient.Id)
                    .ToListAsync();
            }
        }
    }
}
