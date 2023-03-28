using Amatsucozy.PMS.Security.Infrastructure;
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
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    // options.User.AllowedUserNameCharacters =
    //     "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
    options.User.RequireUniqueEmail = true;
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<SecurityDbContext>();

builder.Services.AddIdentityServer()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = dbOptions => dbOptions.UseNpgsql(connectionString,
            sqlBuilder => { sqlBuilder.MigrationsAssembly(typeof(InfrastructureMarker).Assembly.GetName().Name); });
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = dbOptions => dbOptions.UseNpgsql(connectionString,
            sqlBuilder => { sqlBuilder.MigrationsAssembly(typeof(InfrastructureMarker).Assembly.GetName().Name); });
    })
    .AddAspNetIdentity<IdentityUser>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200")
            .AllowAnyHeader();
    });
});
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = false;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});
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
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.MapRazorPages();

app.Run();