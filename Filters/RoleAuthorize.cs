using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PetStoreProject.Repositories.Accounts;

namespace PetStoreProject.Filters
{
    public class RoleAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorize(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var userEmail = filterContext.HttpContext.Session.GetString("userEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                filterContext.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var account = filterContext.HttpContext.RequestServices.GetService<IAccountRepository>();
            var userRole = account.GetUserRoles(userEmail);
            bool isAuthorized = false;
            if (userRole != null)
            {
                if (_roles.Contains(userRole))
                {

                    isAuthorized = true;

                }
            }

            if (isAuthorized == false)
            {
                filterContext.Result = new RedirectToActionResult("AccessDenied", "Account", new { allowedRoles = _roles });
                return;
            }
        }
    }
}
