using BlazorApp1.Service;
using BlazorApp1.Service.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<ChatHubService>();

builder.Services.AddScoped<AuthSession>();

builder.Services.AddScoped<ChatStore>();

var proxyBaseUrl =
    builder.Configuration["Proxy:BaseUrl"] ??
    "http://localhost:5294/";

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