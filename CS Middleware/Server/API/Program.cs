using API.CoreConnection;
using API.Hubs;
using DTOs.MessageActionDTOs;
using DTOs.UserDTOs;
using Model;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Swagger gen???


String baseAddress = "http://localhost:8081/api/";

builder.Services.AddHttpClient<ChatClient>(x => {
    x.BaseAddress = new Uri(baseAddress + "chats/");
});

// This needs to be adjusted likely
builder.Services.AddHttpClient<ChatMemberClient>(x => {
    x.BaseAddress = new Uri(baseAddress + "chats/");
});
builder.Services.AddHttpClient<MessageClient>(x => {
    x.BaseAddress = new Uri(baseAddress + "messages/");
});
builder.Services.AddHttpClient<UserClient>(x => {
    x.BaseAddress = new Uri(baseAddress + "users/");
});


var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.MapHub<MessagesHub>("/hubs/messages");
app.MapHub<CoreHub>("/coreHub");

var userGroup = app.MapGroup("users");
userGroup.MapGet("{id}", async (long id, UserClient userClient) => {
    var user = await userClient.GetUserById(id);
    return Results.Ok(user);
});

userGroup.MapGet("by-username", async (String username, UserClient userClient) => {
    var user = await userClient.GetUserByUsername(username);
    return Results.Ok(user);
});

userGroup.MapPost("register", async (RegisterUserRequest req, UserClient userClient) => {
    var user = await userClient.Register(req);
    return Results.Ok(user);
});

var chatGroup = app.MapGroup("chats");
var messageGroup = app.MapGroup("messages");

messageGroup.MapPost(String.Empty, async (SendMessageRequest req, MessageClient messageClient) => {
    var message = await messageClient.SendMessage(req);
    return Results.Ok(message);
});

messageGroup.MapPatch("{id}", async (String id, EditMessageRequest req, MessageClient messageClient) => {
   var message = await messageClient.EditMessage(id, req);
   return Results.Ok(message);
});


app.Run();
