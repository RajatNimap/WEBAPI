var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); 

var app = builder.Build();



//Conventional Routing


//app.MapGet("/", () => "Hello World!");
//app.MapControllerRoute(
//    name: "default",
//    pattern:"{controller=Home}/{action=Index}/{id?}"

// );

//app.MapDefaultControllerRoute();

app.MapControllers();
app.Run();
