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
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;

        public async Task OnGetAsync(int pageNumber = 1)
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient != null)
            {
                int pageSize = 5;
                var query = _context.Prescriptions
                    .Include(p => p.Doctor)
                    .Where(p => p.PatientId == patient.Id);

                Prescriptions = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
                CurrentPage = pageNumber;
            }
        }
    }
}
