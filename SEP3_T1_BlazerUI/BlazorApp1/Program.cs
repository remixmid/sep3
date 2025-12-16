using BlazorApp1.Service;
using BlazorApp1.Service.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Existing realtime service (keep)
builder.Services.AddSingleton<ChatHubService>();

// per-circuit state
builder.Services.AddScoped<AuthSession>();

// chats list state
builder.Services.AddScoped<ChatStore>();

var proxyBaseUrl =
    builder.Configuration["Proxy:BaseUrl"] ??
    "http://localhost:5294/";

// REAL HTTP APIs to your C# proxy
builder.Services.AddHttpClient<IAuthApi, HttpAuthApi>(c => c.BaseAddress = new Uri(proxyBaseUrl));
builder.Services.AddHttpClient<IUserApi, HttpUserApi>(c => c.BaseAddress = new Uri(proxyBaseUrl));
builder.Services.AddHttpClient<IChatApi, HttpChatApi>(c => c.BaseAddress = new Uri(proxyBaseUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();