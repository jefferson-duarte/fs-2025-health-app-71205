using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using HealthApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HealthApp.Razor.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class CreatePrescriptionModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreatePrescriptionModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Prescription Prescription { get; set; } = new Prescription();

        public SelectList Patients { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Patients = new SelectList(await _context.Patients.ToListAsync(), "Id", "FirstName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
            {
                ModelState.AddModelError("", "Doctor information is missing.");
                return Page();
            }

            Prescription.DoctorId = doctor.Id;

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            _context.Prescriptions.Add(Prescription);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Prescription created successfully.";

            return RedirectToPage("/Doctor/DoctorDashboard");
        }
    }
}
