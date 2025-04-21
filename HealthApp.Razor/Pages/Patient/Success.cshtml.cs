using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthApp.Razor.Pages.Patient
{
    [Authorize(Roles = "Patient")]
    public class SuccessModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
