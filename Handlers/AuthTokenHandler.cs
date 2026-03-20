using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace NoticeBoard_frontend.Handlers;

public class AuthTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User.Identity?.IsAuthenticated == true)
        {
            var idToken = await httpContext.GetTokenAsync("id_token");
            if (!string.IsNullOrEmpty(idToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
