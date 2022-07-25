using Microsoft.EntityFrameworkCore;
using ShopTemplate;
using ShopTemplate.Data;
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
services.AddControllersWithViews();
services.AddDbContext<AppDBContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultCon")
    ));
services.AddSingleton<DataRepository>(new DataRepository(new AppDBContext(new DbContextOptions<AppDBContext>())));

services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
services.AddHttpContextAccessor();

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
app.UseCookiePolicy();
app.UseSession();
app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(name: "Home",
                pattern: "Home/{action}",
                defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}"
    
);



app.Run();
