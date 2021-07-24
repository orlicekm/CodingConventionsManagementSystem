using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CCMS.Pages
{
    public class SigninModel : PageModel
    {
        public async Task OnGet()
        {
            await HttpContext.ChallengeAsync("GitHub", new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        }
    }
}