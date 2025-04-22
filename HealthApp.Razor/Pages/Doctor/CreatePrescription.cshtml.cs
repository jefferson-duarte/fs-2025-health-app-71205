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
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
            {
                ErrorMessage = "Doctor information is missing.";
                return;
            }

            var patientIds = await _context.Appointments
                .Where(a => a.DoctorId == doctor.Id && !a.IsCanceled)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            Patients = new SelectList(await _context.Patients.Where(p => patientIds.Contains(p.Id)).ToListAsync(), "Id", "FirstName");
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

            bool hasAppointment = await _context.Appointments.AnyAsync(a => a.DoctorId == doctor.Id && a.PatientId == Prescription.PatientId && !a.IsCanceled);
            if (!hasAppointment)
            {
                ModelState.AddModelError("", "Prescription can only be created for patients with a scheduled appointment.");
                await OnGetAsync();
                return Page();
            }

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
