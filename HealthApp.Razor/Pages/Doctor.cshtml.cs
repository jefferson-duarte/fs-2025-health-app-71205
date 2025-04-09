using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthApp.Razor.Pages
{
    [Authorize(Roles = "Doctor")]
    public class DoctorModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
