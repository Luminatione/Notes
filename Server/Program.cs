using Common.DTO;
using Common.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Services;
using Server.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Host.UseContentRoot(Directory.GetCurrentDirectory());

var connectionString = builder.Configuration.GetSection("LocalConnectionString").Value;

builder.Services.AddDbContext<DataBaseContext>(e => e.UseNpgsql(connectionString));

builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
builder.Services.AddScoped<Ganss.Xss.IHtmlSanitizer, Ganss.Xss.HtmlSanitizer>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/Auth/Login";
               options.AccessDeniedPath = "/Index";
           });

builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/", "Pages/");
    options.Conventions.AddPageRoute("/", "Views/");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataBaseContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
