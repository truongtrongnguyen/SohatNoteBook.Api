using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Link tutorial :  https://www.youtube.com/playlist?list=PLcvTyQIWJ_ZqVJA4oACh9G_8x7gP5BNoG