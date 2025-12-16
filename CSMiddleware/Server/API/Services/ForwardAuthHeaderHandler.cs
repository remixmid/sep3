using Microsoft.AspNetCore.Http;

namespace API.Services;

public class ForwardAuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _ctx;
    public ForwardAuthHeaderHandler(IHttpContextAccessor ctx) => _ctx = ctx;

    private static string Mask(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "<empty>";
        var t = s.Trim();
        if (t.Length <= 18) return t;
        return t[..12] + "..." + t[^6..];
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var http = _ctx.HttpContext;

        var reqUri = request.RequestUri?.ToString() ?? "<null>";
        Console.WriteLine($"[GW][FWD] HIT -> {request.Method} {reqUri}");

        if (http is null)
        {
            Console.WriteLine("[GW][FWD] HttpContext is NULL -> cannot forward headers");
            return base.SendAsync(request, cancellationToken);
        }

        var trace = http.TraceIdentifier;
        var incomingAuth = http.Request.Headers["Authorization"].ToString();
        var incomingUa = http.Request.Headers["User-Agent"].ToString();

        Console.WriteLine($"[GW][FWD][{trace}] Incoming Path={http.Request.Path} Auth={Mask(incomingAuth)} UA={Mask(incomingUa)}");

        var beforeOutAuth = request.Headers.Contains("Authorization")
            ? Mask(string.Join(",", request.Headers.GetValues("Authorization")))
            : "<missing>";
        Console.WriteLine($"[GW][FWD][{trace}] Outgoing BEFORE set Authorization={beforeOutAuth}");

        if (http.Request.Headers.TryGetValue("Authorization", out var auth) && !string.IsNullOrWhiteSpace(auth))
        {
            request.Headers.Remove("Authorization");
            request.Headers.TryAddWithoutValidation("Authorization", auth.ToString());
        }

        if (http.Request.Headers.TryGetValue("User-Agent", out var ua) && !string.IsNullOrWhiteSpace(ua))
        {
            request.Headers.Remove("User-Agent");
            request.Headers.TryAddWithoutValidation("User-Agent", ua.ToString());
        }

        var afterOutAuth = request.Headers.Contains("Authorization")
            ? Mask(string.Join(",", request.Headers.GetValues("Authorization")))
            : "<missing>";
        Console.WriteLine($"[GW][FWD][{trace}] Outgoing AFTER  set Authorization={afterOutAuth}");

        return base.SendAsync(request, cancellationToken);
    }
}
