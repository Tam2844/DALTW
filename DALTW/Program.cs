using DALTW.Models;
using DALTW.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDistributedMemoryCache(); // Cache để lưu session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian tồn tại session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
.AddDefaultTokenProviders()
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})
.AddCookie()
.AddGoogle(options =>
{
    var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

    options.ClientId = googleAuthNSection["ClientId"];
    options.ClientSecret = googleAuthNSection["ClientSecret"];
});



builder.Services.AddScoped<IDocumentRepository, EFDocumentRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IGradeRepository, EFGradeRepository>();
builder.Services.AddScoped<ITopicRepository, EFTopicRepository>();
builder.Services.AddScoped<ICompetitionRepository, EFCompetitionRepository>();
builder.Services.AddScoped<ISemesterRepository, EFSemesterRepository>();
builder.Services.AddScoped<ITrafficLogRepository, TrafficLogRepository>();


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2147483648; // Tăng lên 2GB
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseMiddleware<TrafficLoggerMiddleware>();


app.MapStaticAssets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "Admin", pattern: "{area:exists}/{controller=DocumentManager}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(name: "User", pattern: "{area:exists}/{controller=DocumentUser}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
});


app.UseStaticFiles();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DocumentUser}/{action=Add}/{id?}")
    .WithStaticAssets();

app.Use(async (context, next) =>
{
    var maxRequestSizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
    if (maxRequestSizeFeature != null)
    {
        maxRequestSizeFeature.MaxRequestBodySize = 2147483648; // 100MB
    }
    await next.Invoke();
});

app.Run();
