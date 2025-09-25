using FFCORP_CASHFLOW.Model;
using FFCCorp.Services;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IReportService, ReportService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
ExcelPackage.License.SetNonCommercialOrganization("My NonCommercial Organization");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<CorrelationIdMiddleware>();

app.MapControllers();

app.Run();
