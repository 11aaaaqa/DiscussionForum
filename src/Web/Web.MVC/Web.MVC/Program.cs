using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Web;
using DNTCaptcha.Core;
using Web.MVC.Middlewares;
using Web.MVC.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(x =>
{
    x.Cookie.Name = "afrgry";
});
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.RequireHttpsMetadata = false;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
});
builder.Services.AddDNTCaptcha(options =>
{
    options.UseCookieStorageProvider()
        .UseCustomFont(Path.Combine(builder.Environment.WebRootPath, "fonts", "movistar.ttf"))
        .WithEncryptionKey(builder.Configuration["Captcha:EncryptionKey"]);
});

builder.Services.AddTransient<IReportService, SuggestionsService>();
builder.Services.AddTransient<ISuggestionService, SuggestionsService>();
builder.Services.AddTransient<ICheckUserService, CheckUserService>();

builder.Services.AddHttpClient();
builder.Services.AddCors();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseMetricServer();
app.UseHttpMetrics();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseRateLimiter();

app.UseMiddleware<JwtTokenMiddleware>();

app.UseAuthentication();
app.UseStatusCodePages(context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
    {
        string returnUrl;
        if (context.HttpContext.Request.QueryString.HasValue)
        {
            returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
        }
        else
        {
            returnUrl = context.HttpContext.Request.Path;
        }
        var encodedReturnUrl = HttpUtility.UrlEncode(returnUrl);
        response.Redirect($"/Auth/Login?returnUrl={encodedReturnUrl}");
    }

    if (response.StatusCode == (int)HttpStatusCode.Forbidden)
    {
        response.Redirect("/info/forbidden");
    }
    
    return Task.CompletedTask;
});
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
