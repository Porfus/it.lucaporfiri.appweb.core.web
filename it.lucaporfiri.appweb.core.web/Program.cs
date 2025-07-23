using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Servizi;
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
});

builder.Services.AddDbContext<ContestoApp>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContestoApp") ?? throw new InvalidOperationException("Connection string 'ContestoApp' not found.")));

// Aggiungi i tuoi service come "scoped" o "transient"
builder.Services.AddScoped<ServiziAtleta>();
builder.Services.AddScoped<ServiziAbbonamento>();
builder.Services.AddScoped<ServiziScheda>();

// Add services to the container.
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContestoApp>();
    dbContext.Database.Migrate();
}

app.Run();
