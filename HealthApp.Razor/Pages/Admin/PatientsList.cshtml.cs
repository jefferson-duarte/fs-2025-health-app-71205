using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using PatientModel = HealthApp.Domain.Models.Patient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;

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

        public async Task OnGetAsync(int pageNumber = 1)
        {
            int pageSize = 10;

            var query = _context.Patients.AsQueryable();

            Patients = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
            CurrentPage = pageNumber;
        }
    }
}
