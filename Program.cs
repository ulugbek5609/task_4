using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using UserManagementApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();





builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddAuthorization();



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

app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var userId = context.User.FindFirst("UserId")?.Value;
        var db = context.RequestServices.GetRequiredService<AppDbContext>();
        var user = await db.Users.FindAsync(int.Parse(userId!));

        if (user == null || user.Status == "Blocked")
        {
            await context.SignOutAsync();
            context.Response.Redirect("/Account/Login");
            return;
        }
    }

    await next();
});

app.Use(async (context, next) =>
{
    var userIdClaim = context.User.FindFirst("UserId");

    if (userIdClaim != null)
    {
        var db = context.RequestServices.GetRequiredService<AppDbContext>();
        var id = int.Parse(userIdClaim.Value);
        var user = await db.Users.FindAsync(id);

        if (user == null || user.Status == "Blocked")
        {
            await context.SignOutAsync();
            context.Response.Redirect("/Account/Login");
            return;
        }
    }

    await next();
});


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();