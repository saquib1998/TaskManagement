using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();
builder.Services.AddAuthentication();

//builder.Services.AddIdentityCore<AppUser>()
//    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

//app.MapIdentityApi<AppUser>();

app.MapControllers();

app.Run();
