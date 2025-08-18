using Hospital_Management.Data;
using Hospital_Management.Exeption;
using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.EntitiesDto;
using Hospital_Management.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audiance"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
            });

builder.Services.AddScoped<IAuthentication, AuthenticationImplementation>();
builder.Services.AddScoped<TokenImplementation>();
builder.Services.AddScoped<RefreshTokenImplementation>();
builder.Services.AddScoped<PatientsImplementation>();
builder.Services.AddScoped<DepartmentImplement>();
builder.Services.AddScoped<DoctorImplementation>();
builder.Services.AddScoped<SlotGenerator>();
builder.Services.AddScoped<AvailabilityRepository>();
builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<DoctorSlotService>();
builder.Services.AddScoped<AppointmentImplementation>();
builder.Services.AddScoped<IMedicalRecord, MedicalRecordImplementation>();
builder.Services.AddScoped<IReportDashboard, DashboardImplementation>();
builder.Services.AddScoped<IDoctorAvaliability, DoctorAvailabilityService>();
builder.Services.AddExceptionHandler<GlobalException>();
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailService>();


builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
