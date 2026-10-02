var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// IHttpContextAccessor (dùng Session trong Views)
builder.Services.AddHttpContextAccessor();

// =====================================================
// Session
// =====================================================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =====================================================
// HttpClient gọi tới BackEnd API
// =====================================================
builder.Services.AddHttpClient("BackEndApi", client =>
{
    var backEndUrl = builder.Configuration["BackEndUrl"] ?? "https://localhost:7198";
    client.BaseAddress = new Uri(backEndUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// =====================================================
// Routing: /Admin, /Staff, default
// =====================================================
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Index}/{id?}",
    defaults: new { controller = "Admin" });

app.MapControllerRoute(
    name: "staff",
    pattern: "Staff/{action=Index}/{id?}",
    defaults: new { controller = "Staff" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
