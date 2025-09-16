using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Data;
using PatientAppointmentSchedulingSystem.Pages.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using PatientAppointmentSchedulingSystem.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddTransient<EmailService>();

//add authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/DoctorLogin";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session timeout
        options.SlidingExpiration = true;
    });

// (Optional) If you also use Session anywhere:
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(o => {
//    o.IdleTimeout = TimeSpan.FromHours(8);
//    o.Cookie.HttpOnly = true;
//    o.Cookie.IsEssential = true;
//});

builder.Services.AddAuthentication("PatientCookie")
    .AddCookie("PatientCookie", options =>
    {
        options.LoginPath = "/PatientLogin";
        options.AccessDeniedPath = "/AccessDenied";
        options.Cookie.Name = "Patient.Auth";
    });

//1. PatientDetails.cs
builder.Services.AddDbContext<PatientDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SystemDatabaseDbConnString"));
});

//2. HospitalDetails.cs
builder.Services.AddDbContext<ProviderDbContext>(options =>
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

app.UseSession(); //must before UseEndPoint/MapRazorPage

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
