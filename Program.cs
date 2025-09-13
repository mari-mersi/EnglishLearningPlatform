using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EnglishLearningPlatform.Data;
using EnglishLearningPlatform.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� Identity � ���������� �������������
builder.Services.AddIdentity<User, IdentityRole>(options => {
    // ��������� ������
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // ��������� ������������
    options.User.RequireUniqueEmail = true;

    // ��������� �����
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;

    // ��������� ����������
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ��������� cookies ��� ��������������
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LogoutPath = "/Account/Logout";
    options.SlidingExpiration = true;
});

// ��������� ������
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ��������� �������������� � �����������
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// ���� ���������� �������� � �������� �����
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); // ������������� ��������� ��� pending ��������

        // ������� ����, ���� �� ���
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        await CreateRoles(roleManager);
        await CreateAdminUser(userManager);
    }
    catch (Exception ex) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations.");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ����� ��� �������� �����
async Task CreateRoles(RoleManager<IdentityRole> roleManager) {
    string[] roleNames = { "Admin", "User" };

    foreach (var roleName in roleNames) {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist) {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

// ����� ��� �������� �������������� �� ���������
async Task CreateAdminUser(UserManager<User> userManager) {
    var adminEmail = "admin@englishplatform.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null) {
        var user = new User {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "System",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createPowerUser = await userManager.CreateAsync(user, "Admin123!");
        if (createPowerUser.Succeeded) {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}