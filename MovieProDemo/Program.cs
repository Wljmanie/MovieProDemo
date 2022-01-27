using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieProDemo.Data;
using MovieProDemo.Models.Settings;
using MovieProDemo.Services;
using MovieProDemo.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.




builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(ConnectionService.GetConnectionString(builder.Configuration)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();




builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();



builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddTransient<SeedService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRemoteMovieService, TmDbMovieService>();
builder.Services.AddScoped<IDataMappingService, TmDbMappingService>();
builder.Services.AddSingleton<IImageService, BasicImageService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//var SeedService = app.Services.CreateScope().ServiceProvider.GetRequiredService<SeedService>();
//await SeedService.ManageDataAsync();
await app.Services.CreateScope().ServiceProvider.GetRequiredService<SeedService>().ManageDataAsync();



app.Run();
