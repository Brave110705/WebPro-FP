using Microsoft.EntityFrameworkCore;
using MyFirstAspApp.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var hasEnvVars = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MYSQL_HOST"));

var connectionString = hasEnvVars
    ? $"server={Environment.GetEnvironmentVariable("MYSQL_HOST")};" +
      $"port={Environment.GetEnvironmentVariable("MYSQL_PORT")};" +
      $"database={Environment.GetEnvironmentVariable("MYSQL_DATABASE")};" +
      $"user={Environment.GetEnvironmentVariable("MYSQL_USERNAME")};" +
      $"password={Environment.GetEnvironmentVariable("MYSQL_PASSWORD")};"
    : builder.Configuration.GetConnectionString("DefaultConnection");

// Hardcode MySQL version to avoid AutoDetect crash
var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);

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
