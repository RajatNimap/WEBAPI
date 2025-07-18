using System.Text;
using System.Text.Json.Serialization;
using E_Commerce;
using E_Commerce.Data;
using E_Commerce.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ResourceFilter>();
builder.Services.AddScoped<AuditLog>();
builder.Services.AddScoped<ResultFIlter>();

builder.Services.AddExceptionHandler<AppExceptionalHandler>();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew=TimeSpan.Zero
    };
});
    builder.Services.AddMemoryCache();
var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

app.UseHttpsRedirection();
app.UseMiddleware<CustomMiddlewarebyDelegate>();
//app.Use(async (context, next) =>
//{
//    Console.WriteLine("Console 1");
//    await next();
//    Console.WriteLine("Console 2");
//});

//app.Use(async (context, next) =>
//{
//    Console.WriteLine("Console 3");
//    await next();
//    Console.WriteLine("Console 4");
//});

app.UseAuthentication();

    //app.UseExceptionHandler(_ => { });

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
