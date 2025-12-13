using System.Text;
using API.CoreConnection;
using API.Hubs;
using DTOs.UserActionRequests;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

var config = builder.Configuration;

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        // This is not null, it's in the appsetings.json
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
    };

    options.Events = new JwtBearerEvents {
        OnMessageReceived = context => {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if(!String.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub/user")) {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// New
builder.Services.AddAuthorization();

var app = builder.Build();

// New
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.MapHub<UserHub>("/hubs/user");
app.MapHub<CoreHub>("/coreHub");




app.Run();
