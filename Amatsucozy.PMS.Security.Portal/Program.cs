using Amatsucozy.PMS.Security.Core;
using Amatsucozy.PMS.Security.Core.Identity;
using Amatsucozy.PMS.Security.Infrastructure;
using Amatsucozy.PMS.Security.Portal;
using Amatsucozy.PMS.Security.Portal.Services;
using Amatsucozy.PMS.Shared.Helpers.MessageQueues;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Default") ??
                       throw new InvalidOperationException("Connection string 'Default' not found.");
builder.Services.AddDbContext<SecurityDbContext>(
    options => options.UseNpgsql(
        connectionString,
        sqlBuilder => { sqlBuilder.MigrationsAssembly(typeof(InfrastructureMarker).Assembly.GetName().Name); }
    ));

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;

    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.RequireUniqueEmail = true;
});
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<SecurityDbContext>()
    .AddDefaultTokenProviders();
var configSection = builder.Configuration.GetSection("IdentityServer");
builder.Services
    .AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = configSection.GetValue<string>("Authority");
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
    })
    .AddOAuth2Introspection("Introspection", options =>
    {
        options.Authority = configSection.GetValue<string>("Authority");
        options.ClientId = "external";
        options.ClientSecret = "442B632E-1341-4643-9883-BC4C24395582";
        options.TokenRetriever = TokenRetrieval.FromAuthorizationHeader("Introspection");
    })
    .AddOAuth2Introspection("Introspection1", options =>
    {
        options.Authority = configSection.GetValue<string>("Authority");
        options.ClientId = "sts";
        options.ClientSecret = "442B632E-1341-4643-9883-BC4C24395582";
        options.TokenRetriever = TokenRetrieval.FromAuthorizationHeader("Introspection1");
    });
builder.Services.AddIdentityServer()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = dbOptions => dbOptions.UseNpgsql(connectionString,
            sqlBuilder => { sqlBuilder.MigrationsAssembly(typeof(InfrastructureMarker).Assembly.GetName().Name); });
        options.DefaultSchema = "security";
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = dbOptions => dbOptions.UseNpgsql(connectionString,
            sqlBuilder => { sqlBuilder.MigrationsAssembly(typeof(InfrastructureMarker).Assembly.GetName().Name); });
        options.DefaultSchema = "security";
    })
    .AddAspNetIdentity<User>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "sts");
    });
    options.AddPolicy("Introspection", policy =>
    {
        policy.AuthenticationSchemes.Add("Introspection");
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "accounts");
        policy.RequireClaim("scope", "pms");
    });
    options.AddPolicy("Introspection1", policy =>
    {
        policy.AuthenticationSchemes.Add("Introspection1");
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "sts");
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://localhost:4200")
            .AllowAnyHeader();
    });
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
builder.Services.AddControllers();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
});
builder.Services.AddMessageQueue(builder.Configuration, typeof(PortalMarker));
builder.Services.AddScoped<IEmailSendRequestBuilder, EmailSendRequestBuilder>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.DbStart();
app.UseHttpsRedirection();
app.UseCors("default");
app.UseStaticFiles();
app.MapRazorPages();
app.MapControllers();
app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.Run();
