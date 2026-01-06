using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Filters;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
});

builder.Services.AddDbContext<ContestoApp>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContestoApp") ?? throw new InvalidOperationException("Connection string 'ContestoApp' not found.")
        , o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

// Aggiungi i tuoi service come "scoped" o "transient"
builder.Services.AddScoped<ServiziAtleta>();
builder.Services.AddScoped<ServiziAbbonamento>();
builder.Services.AddScoped<ServiziScheda>();
builder.Services.AddScoped<ServiziEvento>();
builder.Services.AddScoped<ServiziAccount>();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ControlloPrimoAccessoFilter>();
});

//Activate Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ContestoApp>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; 
    options.AccessDeniedPath = "/Account/AccessDenied"; 
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ContestoApp>();
    dbContext.Database.Migrate();
    await DbInitializer.Initialize(services);
}

app.Run();
