using System.IdentityModel.Tokens.Jwt;
using Amatsucozy.PMS.Security.Core.Identity;
using Amatsucozy.PMS.Security.Infrastructure;
using Amatsucozy.PMS.Security.Portal;
using Amatsucozy.PMS.Security.Portal.Services;
using Amatsucozy.PMS.Shared.Helpers.MessageQueues;
using Duende.IdentityServer;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
    options.TokenLifespan = TimeSpan.FromHours(24);
});
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<SecurityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(IdentityServerConstants.LocalApi.PolicyName, policy =>
    {
        policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        // custom requirements
    });
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireClaim("scope", "sts");
    });
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
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var configSection = builder.Configuration.GetSection("IdentityServer");
        options.Authority = configSection.GetValue<string>("Authority");
        options.TokenValidationParameters.ValidateAudience = false;
        // options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
        // options.ForwardDefaultSelector = (o) => "introspection";
    })
    // .AddOAuth2Introspection("introspection", options =>
    // {
    //     var configSection = builder.Configuration.GetSection("IdentityServer");
    //
    //     // options.Authority = configSection.GetValue<string>("Authority");
    //     options.ClientId = "pat.client";
    //     options.ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader;
    //     // options.ClientSecret = "secret";
    // });
    .AddLocalApi();
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
    options.ExpireTimeSpan = TimeSpan.FromHours(48);
    options.SlidingExpiration = true;
    options.Cookie.MaxAge = TimeSpan.FromHours(48);
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
app.UseIdentityServer();
app.UseAuthorization();

app.Run();
