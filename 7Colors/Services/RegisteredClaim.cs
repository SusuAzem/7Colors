using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace _7Colors.Services
{
    public class RegisteredClaim : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var claimType = "Registered";
            if (!principal.HasClaim(claim => claim.Type == claimType))
            {
                claimsIdentity.AddClaim(new Claim(claimType, "true"));
            }
            principal.AddIdentity(claimsIdentity);
            return Task.FromResult(principal);
        }
    }
}
