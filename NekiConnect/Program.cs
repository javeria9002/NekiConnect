using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NekiConnect.Components;
using NekiConnect.Data;
using NekiConnect.Models;
using NekiConnect.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── JWT Bearer authentication (for API endpoints) ──
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Required for Razor Page login handler
builder.Services.AddRazorPages();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

// ── Services (uncomment as you rebuild each file) ──
//builder.Services.AddScoped<NGOService>();
//builder.Services.AddScoped<FundraisingService>();
//builder.Services.AddScoped<CampaignService>();
//builder.Services.AddScoped<DonationService>();
//builder.Services.AddScoped<VolunteerService>();
//builder.Services.AddScoped<BlogService>();
//builder.Services.AddScoped<BeneficiaryService>();
builder.Services.AddScoped<UserService>();
//builder.Services.AddScoped<AdminService>();
//builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AuthService>();

// ── JWT ──
builder.Services.AddScoped<TokenService>();

// ── Email (Brevo) ──
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Seed roles + admin on startup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var role in new[] { "Donor", "NGO", "Admin" })
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    const string adminEmail = "admin@nekiconnect.com";
    const string adminPassword = "Admin@1234";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            FullName = "Super Admin",
            UserName = adminEmail,
            Email = adminEmail,
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(newAdmin, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(newAdmin, "Admin");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();