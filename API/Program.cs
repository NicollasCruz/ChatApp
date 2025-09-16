using API.Data;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Endpoints;
using API.Services;
using API.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy =>
{
    policy.WithOrigins("http://localhost:4200", "https:localhost:4200")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
}));

var JwtSetting = builder.Configuration.GetSection("JWTSetting");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=chatapp.db"));

builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.SaveToken = true;
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSetting.GetSection("SecurityKey").Value!))
    };

    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hubs/chat")))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
}
);

builder.Services.AddOpenApi();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapHub<ChatHub>("/hubs/chat");
app.MapAccountEndpoint();

app.Run();

