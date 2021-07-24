using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CCMS.Pages
{
    public class SignoutModel : PageModel
    {
        public async Task OnGet()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Response.Redirect("/");
        }
    }
}