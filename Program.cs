
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5007); // Allow external devices
});

// Read secret key from config or fallback to default
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// JWT Config

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TherapistOnly", policy => policy.RequireRole("Therapist"));
    options.AddPolicy("PatientOnly", policy => policy.RequireRole("Patient"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddScoped<ITokenService, TokenService>();
var app = builder.Build();

    using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // 1. Ensure roles exist
    string[] roles = new[] { "Admin", "Therapist", "Patient" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 2. Create admin user if not exists
    var email = "admin@test.com";
    var password = "Admin@123";

    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        var newUser = new ApplicationUser
        {
             UserName = email,
    Email = email,
    Role = "Admin",
    EmailConfirmed = true
            // UserName = email,
            // Email = email,
            // Role = "Admin",
            // password = password,
            // EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newUser, password);
        if (result.Succeeded)
        {
            // Assign Admin role
            await userManager.AddToRoleAsync(newUser, "Admin");
            Console.WriteLine("✅ Admin user created and added to Admin role.");
        }
        else
        {
            Console.WriteLine("❌ Failed to create admin:");
            foreach (var error in result.Errors)
                Console.WriteLine($" - {error.Description}");
        }
    }
    else
    {
        Console.WriteLine("ℹ️ Admin already exists.");
    }
}


    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    await app.RunAsync();
