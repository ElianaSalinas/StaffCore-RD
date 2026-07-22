using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StaffCore_RD.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios ─────────────────────────────────────────────────────────────────

// 1. DbContext (EF Core)
builder.Services.AddDbContext<StaffDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StaffCore")));

// 2. Identity con opciones de contraseña
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<StaffDbContext>()
.AddDefaultTokenProviders();

// 3. Controladores con vistas (MVC)
builder.Services.AddControllersWithViews();

// Configuración de la cookie de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────────────────

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 4. UseAuthentication ANTES de UseAuthorization (orden crítico — error fatal si se invierte)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ── Seed de roles ─────────────────────────────────────────────────────────────
// Se crean los 3 roles requeridos al iniciar si no existen todavía
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = ["Administrador", "RRHH", "Viewer"];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.Run();
