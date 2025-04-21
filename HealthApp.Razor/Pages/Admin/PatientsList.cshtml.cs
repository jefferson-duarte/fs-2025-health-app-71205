using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using PatientModel = HealthApp.Domain.Models.Patient;
using Microsoft.AspNetCore.Authorization;


namespace HealthApp.Razor.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class PatientsListModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PatientsListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PatientModel> Patients { get; set; } = new List<PatientModel>();

        public async Task OnGetAsync()
        {
            Patients = await _context.Patients.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
