using Lift.Data;
using Lift.Simulator;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Baza de date
builder.Services.AddDbContext<LiftDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Transient);

// Simulatorul - un singur obiect pentru toata aplicatia
builder.Services.AddSingleton<LiftProcess>();

// MVC
builder.Services.AddControllersWithViews();

// Sesiuni pentru login
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

// Cream baza de date automat la pornire
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LiftDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Pornim simulatorul
var lift = app.Services.GetRequiredService<LiftProcess>();
lift.Start();

app.Run();