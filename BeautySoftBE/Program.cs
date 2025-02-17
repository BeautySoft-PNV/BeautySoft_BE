using Microsoft.EntityFrameworkCore;
using BeautySoftBE.Data;
using BeautySoftBE.Repositories;
using BeautySoftBE.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IMakeupItemService, MakeupItemService>();
builder.Services.AddScoped<IMakeupItemRepository, MakeupItemRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/ spnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();