using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Data;
using PatientAppointmentSchedulingSystem.Pages.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();

//add authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/DoctorLogin";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session timeout
        options.SlidingExpiration = true;
    });

//1. PatientDetails.cs
builder.Services.AddDbContext<PatientDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SystemDatabaseDbConnString"));
});

//2. HospitalDetails.cs
builder.Services.AddDbContext<HospitalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SystemDatabaseDbConnString"));
});

//3. AvailabilitySlots.cs
builder.Services.AddDbContext<AvailabilitySlotDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SystemDatabaseDbConnString"));
});

//4. Doctor.cs
builder.Services.AddDbContext<DoctorDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SystemDatabaseDbConnString"));
});



builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); ;
} 

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
