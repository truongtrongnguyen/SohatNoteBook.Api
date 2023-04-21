using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SohatNoteBook.Authentication.Configuration;
using SohatNoteBook.DataService.Data;
using SohatNoteBook.DataService.IConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(option =>
{
    var connect = builder.Configuration.GetConnectionString("AppDbContext");
    option.UseSqlServer(connect);
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddApiVersioning(opt =>
{
    // Provides to the client the different Api versions that we have
    opt.ReportApiVersions = true;

    // this will allow the api to automatically provides a default version
    opt.AssumeDefaultVersionWhenUnspecified = true;

    opt.DefaultApiVersion = ApiVersion.Default;
});

// Update the JWT config from the settings
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

/*
 * means that we are telling asp.net core that in order for us to utilize any authentication 
 * or any authorization that we want to implement in the future we need to basically build everything on jwt
 */
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    // Get the secret from config
    var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

    // after authorization do you wanna save this token
    jwt.SaveToken = true;

    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,         // ToDo Update
        ValidateAudience = false,       // ToDo Update
        RequireExpirationTime = false,  // ToDo Update
        ValidateLifetime = true
    };
});
builder.Services.AddDefaultIdentity<IdentityUser>(option =>
    option.SignIn.RequireConfirmedAccount = true
).AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Link tutorial :  https://www.youtube.com/playlist?list=PLcvTyQIWJ_ZqVJA4oACh9G_8x7gP5BNoG