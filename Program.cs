using Microsoft.EntityFrameworkCore;
using MyFirstAspApp.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Check if Zeabur env vars exist
var hasEnvVars = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQL_HOST"));

// If in Zeabur, build full connection string
var connectionString = hasEnvVars
    ? $"server={Environment.GetEnvironmentVariable("MYSQL_HOST")};" +
      $"port={Environment.GetEnvironmentVariable("MYSQL_PORT")};" +
      $"database={Environment.GetEnvironmentVariable("MYSQL_DATABASE")};" +
      $"user={Environment.GetEnvironmentVariable("MYSQL_USERNAME")};" +  // ← FIXED
      $"password={Environment.GetEnvironmentVariable("MYSQL_PASSWORD")};"
    : builder.Configuration.GetConnectionString("DefaultConnection");

// Safety check
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("❗ Database connection string missing.");
}

// Add DbContext
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();
app.Run();
