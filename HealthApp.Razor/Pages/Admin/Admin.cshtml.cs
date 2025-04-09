using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using PatientModel = HealthApp.Domain.Models.Patient;


namespace HealthApp.Razor.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        public string UserId { get; set; }
        public List<PatientModel> Patients { get; set; }
        public void OnGet()
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Patients = MockData.Patients();
        }
    }
}
