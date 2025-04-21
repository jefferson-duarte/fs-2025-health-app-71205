using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using DoctorModel = HealthApp.Domain.Models.Doctor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HealthApp.Razor.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DoctorsListModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DoctorsListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<DoctorModel> Doctors { get; set; } = new List<DoctorModel>();

        public async Task OnGetAsync()
        {
            Doctors = await _context.Doctors.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
