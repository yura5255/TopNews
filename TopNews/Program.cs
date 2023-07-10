using TopNews.Infrastructure;
using TopNews.Infrastructure.Initializers;

var builder = WebApplication.CreateBuilder(args);

// Create connection sting
string connStr = builder.Configuration.GetConnectionString("DefaultConnection");
// Database context
builder.Services.AddDbContext(connStr);
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Infastracture services
builder.Services.AddInfrastructureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await UsersAndRolesInitializers.SeedUserAndRole(app);

app.Run();
