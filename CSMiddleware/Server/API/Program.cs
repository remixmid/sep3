using System.Text.Json.Serialization;
using API.CoreConnection;
using API.Hubs;
using API.Services; // ForwardAuthHeaderHandler

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSignalR();
builder.Services.AddScoped<IClientProviderService, ClientProviderService>();


builder.Services.AddScoped<RealtimePublisher>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ForwardAuthHeaderHandler>();

const string coreBase = "http://localhost:8081";
const string coreApi = coreBase + "/api/";

Console.WriteLine($"[GW] Starting. coreBase={coreBase}, coreApi={coreApi}");

builder.Services.AddHttpClient("CoreAuth", x =>
{
    x.BaseAddress = new Uri(coreBase);
    Console.WriteLine($"[GW] HttpClient 'CoreAuth' BaseAddress={x.BaseAddress}");
}).AddHttpMessageHandler<ForwardAuthHeaderHandler>();

builder.Services.AddHttpClient<ChatClient>(x =>
{
    x.BaseAddress = new Uri(coreApi + "chats");
    Console.WriteLine($"[GW] Typed HttpClient <ChatClient> BaseAddress={x.BaseAddress}");
}).AddHttpMessageHandler<ForwardAuthHeaderHandler>();

builder.Services.AddHttpClient<ChatMemberClient>(x =>
{
    x.BaseAddress = new Uri(coreApi + "chats/");
    Console.WriteLine($"[GW] Typed HttpClient <ChatMemberClient> BaseAddress={x.BaseAddress}");
}).AddHttpMessageHandler<ForwardAuthHeaderHandler>();

builder.Services.AddHttpClient<MessageClient>(x =>
{
    x.BaseAddress = new Uri(coreApi);
    Console.WriteLine($"[GW] Typed HttpClient <MessageClient> BaseAddress={x.BaseAddress}");
}).AddHttpMessageHandler<ForwardAuthHeaderHandler>();

builder.Services.AddHttpClient<UserClient>(x =>
{
    x.BaseAddress = new Uri(coreApi + "users/");
    Console.WriteLine($"[GW] Typed HttpClient <UserClient> BaseAddress={x.BaseAddress}");
}).AddHttpMessageHandler<ForwardAuthHeaderHandler>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

Console.WriteLine("[GW] Gateway mode: Authentication/Authorization middleware is DISABLED in C#");

app.MapControllers();

app.MapHub<UserHub>("/hubs/user");
app.MapHub<CoreHub>("/coreHub");

app.MapHub<RealtimeHub>("/hubs/realtime");

app.Run();
