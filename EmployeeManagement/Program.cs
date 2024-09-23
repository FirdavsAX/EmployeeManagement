using EmployeeManagement.Data;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Mappings;
using EmployeeManagement.Middlewares;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Authorization;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EmployeeDbContext>();
builder.Services.AddIdentityCore<User>((options) =>
    {
        options.Password.RequireDigit = false;           // Allow no digits
        options.Password.RequireLowercase = false;       // Allow no lowercase letters
        options.Password.RequireUppercase = false;       // Allow no uppercase letters
        options.Password.RequireNonAlphanumeric = false; // Allow no special characters
        options.Password.RequiredLength = 1;             // Set minimum length to 1
        options.Password.RequiredUniqueChars = 0;
    })
    .AddEntityFrameworkStores<EmployeeDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

//Add mappings
builder.Services.AddAutoMapper(typeof(UserMappings).Assembly);
// Add JwtBearer authentication with event handlers for 401 and 403
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secret = builder.Configuration["JwtConfig:Secret"];
    var issuer = builder.Configuration["JwtConfig:ValidIssuer"];
    var audience = builder.Configuration["JwtConfig:ValidAudiences"];
    if (secret is null || issuer is null || audience is null)
    {
        throw new ApplicationException("Jwt is not set in the Configuration");
    }
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Read token from the AuthToken cookie
            context.Token = context.Request.Cookies["AuthToken"];
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            // Handle 401 Unauthorized
            if (context.Response.StatusCode == 401)
            {
                context.Response.Redirect("/Auth/Login");
                context.HandleResponse(); // Skip the default response
            }
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            // Handle 403 Forbidden
            context.Response.Redirect("/Auth/Login");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AppAuthorizationPolicies.RequireAccountActive,
        policy => policy.RequireClaim(AppClaimTypes.Status, Status.UnBlocked.ToString()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseMiddleware<ExceptionMiddleware>();

// Add middleware for handling 403 and 401 status codes globally
app.UseStatusCodePages(context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode == 401 || response.StatusCode == 403)
    {
        response.Redirect("/Auth/Login");
    }
    return Task.CompletedTask;
});
app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CheckActiveUsersMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
