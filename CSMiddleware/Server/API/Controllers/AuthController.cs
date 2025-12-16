using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly HttpClient _coreAuth;
    private const string RefreshCookieName = "refresh_token";

    public AuthController(IHttpClientFactory factory)
    {
        _coreAuth = factory.CreateClient("CoreAuth");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] object req)
    {
        var resp = await _coreAuth.PostAsJsonAsync("/auth/register", req);
        return await ProxyRaw(resp);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] object req)
    {
        var msg = new HttpRequestMessage(HttpMethod.Post, "/auth/login")
        {
            Content = JsonContent.Create(req)
        };

        if (Request.Headers.TryGetValue("User-Agent", out var ua))
            msg.Headers.TryAddWithoutValidation("User-Agent", ua.ToString());

        var resp = await _coreAuth.SendAsync(msg);
        ForwardSetCookie(resp);
        return await ProxyJson(resp);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var refresh) || string.IsNullOrWhiteSpace(refresh))
            return Unauthorized();

        var msg = new HttpRequestMessage(HttpMethod.Post, "/auth/refresh");
        msg.Headers.Add("Cookie", $"{RefreshCookieName}={refresh}");

        if (Request.Headers.TryGetValue("User-Agent", out var ua))
            msg.Headers.TryAddWithoutValidation("User-Agent", ua.ToString());

        var resp = await _coreAuth.SendAsync(msg);
        ForwardSetCookie(resp);
        return await ProxyJson(resp);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        Request.Cookies.TryGetValue(RefreshCookieName, out var refresh);
        var msg = new HttpRequestMessage(HttpMethod.Post, "/auth/logout");
        if (!string.IsNullOrWhiteSpace(refresh))
            msg.Headers.Add("Cookie", $"{RefreshCookieName}={refresh}");

        var resp = await _coreAuth.SendAsync(msg);
        Response.Cookies.Delete(RefreshCookieName, new CookieOptions { Path = "/auth" });
        ForwardSetCookie(resp);
        return StatusCode((int)resp.StatusCode);
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var msg = new HttpRequestMessage(HttpMethod.Get, "/auth/me");
        if (Request.Headers.TryGetValue("Authorization", out var auth))
            msg.Headers.TryAddWithoutValidation("Authorization", auth.ToString());

        var resp = await _coreAuth.SendAsync(msg);
        return await ProxyJson(resp);
    }

    private void ForwardSetCookie(HttpResponseMessage resp)
    {
        if (!resp.Headers.TryGetValues("Set-Cookie", out var cookies)) return;
        foreach (var c in cookies)
            Response.Headers.Append("Set-Cookie", c);
    }

    private async Task<IActionResult> ProxyJson(HttpResponseMessage resp)
    {
        var body = await resp.Content.ReadAsStringAsync();
        return new ContentResult
        {
            StatusCode = (int)resp.StatusCode,
            ContentType = "application/json",
            Content = body
        };
    }

    private async Task<IActionResult> ProxyRaw(HttpResponseMessage resp)
    {
        var body = await resp.Content.ReadAsStringAsync();
        return StatusCode((int)resp.StatusCode, body);
    }
}
