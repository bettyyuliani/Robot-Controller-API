using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using robot_controller_api.Persistence;
using System.Security.Claims;
using BCrypt.Net;

namespace robot_controller_api.Authentication
{
  public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
  {
    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      var endpoint = this.Context.GetEndpoint();
      // Check for [AllowAnonymous] present on method
      // this is always null and not present in Metadata?
      if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
      {
        return Task.FromResult(AuthenticateResult.NoResult());
      }

      base.Response.Headers.Add("WWW-Authenticate", @"Basic realm=""Access to the robot controller.""");
      var authHeader = base.Request.Headers["Authorization"].ToString();
      string[] str = authHeader.Split(' ');
      if (str.Length > 1)
      {
        var base64EncodedBytes = System.Convert.FromBase64String(str[1]);
        string plainText = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        string[] plainTextArr = plainText.Split(':');
        string email = plainTextArr[0];
        string password = plainTextArr[1];
        UserRepository repo = new UserRepository();
        UserModel user = repo.GetUsers().Where(i => i.Email == email).FirstOrDefault();
        if (user != null)
        {
          //var hasher = new PasswordHasher<UserModel>();
          var pwVerificationResult = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

          if (pwVerificationResult)
          {
            var claims = new[]
            {
                new Claim("name", $"{user.FirstName}{user.FirstName}"),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
                // any other claims that you think might be useful
            };
            var claims2 = new[] // this part is just to test a set of claimsidentity passed into claimsprincipal
            {
              new Claim(ClaimTypes.Email, user.Email),
              new Claim(ClaimTypes.Surname, user.LastName)
            };
            var identity1 = new ClaimsIdentity(claims, "Basic");
            var identity2 = new ClaimsIdentity(claims2, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity[] { identity1, identity2 });
            var authTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(authTicket));
          }
        }
      }
      // Authentication logic will be here.
      Response.StatusCode = 401;
      return Task.FromResult(AuthenticateResult.Fail($"Incorrect Authentication Details!!!!"));
    }
  }

}
