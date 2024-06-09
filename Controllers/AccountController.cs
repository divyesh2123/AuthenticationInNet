using AuthenticationInNet.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthenticationInNet.Controllers
{
    public class AccountController : Controller
    {
        public List<UserModel> users = null;

        public AccountController()
        {
            users = new List<UserModel>();
            users.Add(new UserModel()
            {
                UserId = 1,
                Username = "Anoop",
                Password = "123",
                Role = "Admin"
            });
            users.Add(new UserModel()
            {
                UserId = 2,
                Username = "Other",
                Password = "123",
                Role = "User"
            });
        }

        public IActionResult Index(string ReturnUrl = "/")
        {
            LoginModel d = new LoginModel();
            d.ReturnUrl = ReturnUrl;
            return View(d);
        }

        [HttpPost]
        public IActionResult Index(LoginModel objLoginModel)
        {
            var user = users.Where(x => x.Username == objLoginModel.UserName && x.Password == objLoginModel.Password).FirstOrDefault();
            if (user == null)
            {
                //Add logic here to display some message to user    
                ViewBag.Message = "Invalid Credential";
                return View(objLoginModel);
            }
            else
            {
                var claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.UserId)),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("FavoriteDrink", "Tea")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                 HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                {
                    IsPersistent = objLoginModel.RememberMe
                });

                return LocalRedirect(objLoginModel.ReturnUrl);  
            }
        }
    }
}
