

using Practice2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<CustomMiddleware>();
var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.Use(async (context, next) => {
//    await context.Response.WriteAsync("Hello form 1 \n");
//    await next();
//    await context.Response.WriteAsync("Hello form 1 ,1 \n");
//});
//app.Use(async (context, next) => {
//    await context.Response.WriteAsync("HEllo From run 2 \n");
//    await next();
//    await context.Response.WriteAsync("Hello form 2 ,1 \n");
//});
//app.Map("/rajat", rajatCode =>
//{
//    rajatCode.Use(async (context, next) =>
//    {
//        await context.Response.WriteAsync("this is from Rajat end \n");
//        await next();
//    });
//    rajatCode.Run(async context =>
//    {
//        await context.Response.WriteAsync("this is end from rajat end \n");
//    });
//});
//app.UseMiddleware<CustomMiddleware>();

//app.Run(async context => {

//    await context.Response.WriteAsync("HEllo From run 3 \n");

//});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
