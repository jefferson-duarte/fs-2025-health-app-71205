using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Authorization;

namespace HealthApp.Razor.Pages
{
    [Authorize(Roles = "Admin")]
    public class DoctorPatientListingModel : PageModel
    {
        private readonly HealthApp.Razor.Data.ApplicationDbContext _context;

        public DoctorPatientListingModel(HealthApp.Razor.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<DoctorPatient> DoctorPatient { get;set; } = default!;

        public async Task OnGetAsync()
        {
            DoctorPatient = await _context.DoctorPatient
                .Include(d => d.Doctor)
                .Include(d => d.Patient).ToListAsync();
        }
    }
}
