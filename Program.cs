using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using BookStore.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreContext")
        ?? throw new InvalidOperationException("Connection string 'BookStoreContext' not found.")));

builder.Services.AddRazorPages();

// Register Identity with DefaultUser and the Entity Framework store
builder.Services.AddDefaultIdentity<DefaultUser>(op => op.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>() .AddEntityFrameworkStores<BookStoreContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Register the cart as scoped service
builder.Services.AddScoped(sp => Cart.GetCart(sp));

// Configure session middleware
builder.Services.AddSession(op =>
{
    op.Cookie.HttpOnly = true;
    op.Cookie.IsEssential = true;
    op.IdleTimeout = TimeSpan.FromMinutes(20); // Set a reasonable timeout duration
});

// Add controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BookStoreContext>();
    try
    {
        //SeedData.Initialize(services);
        UserRoleInitializer.InitializeAsync(services).Wait();

    }
    catch (Exception ex)
    {
        // Log error here as needed
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, $"An error occurred seeding the DB: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Ensure session middleware is registered here

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();
