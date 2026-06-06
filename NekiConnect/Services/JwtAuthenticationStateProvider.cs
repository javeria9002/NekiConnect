using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NekiConnect.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _http;
        private AuthenticationState? _cachedState;

        public JwtAuthenticationStateProvider(IHttpContextAccessor http)
        {
            _http = http;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // ✅ Return cached state for SignalR calls where HttpContext is null
            if (_cachedState != null)
                return Task.FromResult(_cachedState);

            var token = _http.HttpContext?.Request.Cookies["jwt"];

            if (string.IsNullOrWhiteSpace(token))
            {
                _cachedState = Unauthenticated();
                return Task.FromResult(_cachedState);
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                // ✅ Reject expired tokens
                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    _cachedState = Unauthenticated();
                    return Task.FromResult(_cachedState);
                }

                var identity = new ClaimsIdentity(jwt.Claims, "jwt");
                _cachedState = new AuthenticationState(new ClaimsPrincipal(identity));
                return Task.FromResult(_cachedState);
            }
            catch
            {
                _cachedState = Unauthenticated();
                return Task.FromResult(_cachedState);
            }
        }

        // ✅ Call this on logout to clear cached state
        public void ClearState()
        {
            _cachedState = null;
            NotifyAuthenticationStateChanged(Task.FromResult(Unauthenticated()));
        }

        private static AuthenticationState Unauthenticated() =>
            new(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}