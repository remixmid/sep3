using API.CoreConnection;
using API.Hubs;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Model;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Swagger gen???


String baseAddress = "http://localhost:8081/api/";

// This needs to be adjusted likely. I believe it is the correct path though.
builder.Services.AddHttpClient<ChatClient>(x => {
    x.BaseAddress = new Uri(baseAddress);
});

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
app.MapHub<UserHub>("/hubs/messages");
app.MapHub<CoreHub>("/coreHub");




app.Run();
