using TopNews.Core;
using TopNews.Infrastructure;
using TopNews.Infrastructure.Initializers;

var builder = WebApplication.CreateBuilder(args);

// Create connection sting
string connStr = builder.Configuration.GetConnectionString("DefaultConnection");
// Database context
builder.Services.AddDbContext(connStr);
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add core services
builder.Services.AddCoreServices();

// Add Infastracture services
builder.Services.AddInfrastructureServices();

// Add mapping services
builder.Services.AddMapping();

// Add repositories
builder.Services.AddRepositories();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithRedirects("/Error/{0}");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await UsersAndRolesInitializers.SeedUserAndRole(app);

app.Run();
