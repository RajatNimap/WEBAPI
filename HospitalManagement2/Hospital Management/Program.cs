using System;
using System.Text;
using System.Text.Json.Serialization;
using Hospital_Management.Data;
using Hospital_Management.Exeption;
using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
builder.Services.AddExceptionHandler<GlobalException>();
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailService>();


builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hospital Management",
        Version = "v1",
        Description = "API documentation for the Hospital Outpatient Management System"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
