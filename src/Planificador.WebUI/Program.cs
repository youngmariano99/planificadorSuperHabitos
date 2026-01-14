using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planificador.Infrastructure.Persistence;
using Planificador.WebUI.Components;
using Planificador.Application.Interfaces;     // <--- Agregar using
using Planificador.Infrastructure.Services;    // <--- Agregar using

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient();

// Inyección de Dependencia de nuestro servicio
builder.Services.AddScoped<IPlanningService, PlanningService>();

// 2. Configuración de Identity (Auth)
builder.Services.AddIdentityCore<IdentityUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 4;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// --- CORRECCIÓN: Registramos el enviador de emails "falso" para que no falle ---
builder.Services.AddSingleton<IEmailSender<IdentityUser>, IdentityNoOpEmailSender>();
// -----------------------------------------------------------------------------

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies(options => 
    {
        options.ApplicationCookie?.Configure(c => 
        {
            // Esto es vital para que el login funcione en local (http)
            c.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            c.LoginPath = "/login";
            c.LogoutPath = "/logout";
        });
    });

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// app.UseHttpsRedirection(); 
app.UseStaticFiles();
app.UseAntiforgery();

// Endpoints de API de Identity
app.MapGroup("/account").MapIdentityApi<IdentityUser>();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// --- CLASE EXTRA: Email Sender que no hace nada (necesario para MapIdentityApi) ---
internal sealed class IdentityNoOpEmailSender : IEmailSender<IdentityUser>
{
    public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink) => Task.CompletedTask;
    public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink) => Task.CompletedTask;
    public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode) => Task.CompletedTask;
}